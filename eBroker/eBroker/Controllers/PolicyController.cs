using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;

namespace eBroker.Controllers
{
    public class PolicyController : BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListPolicy(string query, string ExpStartDate, string ExpEndDate, string EffStartDate, string EffEndDate)
        {
            this.PolicyInfo(0);
            ViewBag.renewed = true;
            var policy = new List<Vw_Policy_Report>();
            try
            {
                DateTime _ExpStartDate = new DateTime();
                DateTime _ExpEndDate = new DateTime();
                DateTime _EffStartDate = new DateTime();
                DateTime _EffEndDate = new DateTime();

                DateTime.TryParse(ExpStartDate, out _ExpStartDate);
                DateTime.TryParse(ExpEndDate, out _ExpEndDate);
                DateTime.TryParse(EffStartDate, out _EffStartDate);
                DateTime.TryParse(EffEndDate, out _EffEndDate);

                if (string.IsNullOrEmpty(query))
                {
                    policy = dc.Vw_Policy_Report.OrderByDescending(x => x.Id).ToList();
                    if (policy == null || policy.Count == 0)
                        return View(policy);
                }
                else
                    policy = dc.Vw_Policy_Report.Where(x => x.client_name.Contains(query) || x.product_name.Contains(query) || x.insurer.Contains(query) || x.policy_no.Contains(query)).ToList();
                if (!string.IsNullOrEmpty(ExpStartDate))
                    policy = policy.Where(x => x.expiry_dt >= _ExpStartDate).ToList();
                //else if (!string.IsNullOrEmpty(ExpStartDate) && string.IsNullOrEmpty(query))
                //    policy = dc.Vw_Policy_Report.Where(x => x.expiry_dt >= _ExpStartDate).ToList();
                if (!string.IsNullOrEmpty(ExpEndDate))
                    policy = policy.Where(x => x.expiry_dt <= _ExpEndDate).ToList();
                //else if (!string.IsNullOrEmpty(ExpEndDate) && string.IsNullOrEmpty(query))
                //    policy = dc.Vw_Policy_Report.Where(x => x.expiry_dt <= _ExpEndDate).ToList();
                if (!string.IsNullOrEmpty(EffStartDate))
                    policy = policy.Where(x => x.expiry_dt >= _EffStartDate).ToList();
                //else if (!string.IsNullOrEmpty(EffStartDate) && string.IsNullOrEmpty(query))
                //    policy = dc.Vw_Policy_Report.Where(x => x.expiry_dt >= _EffStartDate).ToList();
                if (!string.IsNullOrEmpty(EffEndDate))
                    policy = policy.Where(x => x.expiry_dt <= _EffEndDate).ToList();
                //else if (!string.IsNullOrEmpty(EffEndDate) && string.IsNullOrEmpty(query))
                //    policy = dc.Vw_Policy_Report.Where(x => x.expiry_dt <= _EffEndDate).ToList();
                return View(policy.OrderByDescending(x=>x.Id));
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult CustomerPolicy(int ccode)
        {
            var policy = new List<InsurancePolicy>();
            try
            {
                policy = dc.InsurancePolicy.Where(x => x.client_id == ccode).OrderByDescending(x => x.Id).ToList();
                ViewBag.ClientInfo = dc.Client.Where(x => x.Id == ccode).FirstOrDefault();
                return View(policy);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }


        public ActionResult PolicyItems(int CId, int Dis = 0)
        {
            try
            {
                var policy = dc.InsurancePolicy.Where(x => x.Id == CId).FirstOrDefault();
                ViewBag.DisableAddNew = Dis;
                if (policy.InsuranceProducts.category_id == 1)//Motor
                {
                    return RedirectToAction("ListPolicyVehicle", new { CId = CId });
                }
                else if (policy.InsuranceProducts.category_id == 2)//Fire
                {
                    return RedirectToAction("ListPolicyProperty", new { CId = CId });
                }
                else
                {
                    Danger(policy.InsuranceProducts.product_name + " Insurance doesn't have this option", true);
                    return RedirectToAction("ListPolicy");//No action required
                }
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return RedirectToAction("ListPolicy");//No action required
        }

        public ActionResult ListPolicyVehicle(int CId)
        {

            this.PolicyVehicleInfo(CId, 0);
            var policyVehicle = new List<Policy_Vehicle>();
            try
            {
                policyVehicle = dc.Policy_Vehicle.Where(x => x.contract_id == CId).ToList();
                ViewBag.PolicyId = CId;
                return View(policyVehicle);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }
        public ActionResult ListPolicyProperty(int CId)
        {
            this.PolicyPropertyInfo(CId, 0);
            var policyProperty = new List<Policy_Property>();
            try
            {
                policyProperty = dc.Policy_Property.Where(x => x.contract_id == CId).ToList();
                ViewBag.PolicyId = CId;
                return View(policyProperty);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreatePolicy2(eBroker.InsurancePolicy ip)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    string Resp = "";
                    //Data Validation
                    if (ip.renewable && ip.renewal_basis == "")
                    {
                        Danger("Select Renewable Basis", true);
                        //Reading Customer Recruiter
                        var recruiter = (from r in dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                        var rec = (from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
                        recruiter.AddRange(rec);
                        ViewBag.Recruiters = recruiter;
                        var insurers = (from a in dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Insurers = insurers;
                        var products = (from a in dc.Insurance_Product.OrderBy(x => x.product_name).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Products = products;
                        var clients = (from a in dc.Client.OrderBy(x => x.client_name).ToList() select new SelectListItem { Text = a.client_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Clients = clients;
                        var banks = (from a in dc.Bank.OrderBy(x => x.BankName).ToList() select new SelectListItem { Text = a.BankName, Value = a.Id.ToString() }).ToList();
                        ViewBag.Banks = banks;
                        return View("PolicyInfo", ip);
                    }
                    if (ip.interest_transfer && ip.interest_bank_id == 0)
                    {
                        Danger("Select Interest Transfer Bank", true);
                        //Reading Customer Recruiter
                        var recruiter = (from r in dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                        var rec = (from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
                        recruiter.AddRange(rec);
                        ViewBag.Recruiters = recruiter;
                        var insurers = (from a in dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Insurers = insurers;
                        var products = (from a in dc.Insurance_Product.OrderBy(x => x.product_name).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Products = products;
                        var clients = (from a in dc.Client.OrderBy(x => x.client_name).ToList() select new SelectListItem { Text = a.client_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Clients = clients;
                        var banks = (from a in dc.Bank.OrderBy(x => x.BankName).ToList() select new SelectListItem { Text = a.BankName, Value = a.Id.ToString() }).ToList();
                        ViewBag.Banks = banks;
                        return View("PolicyInfo", ip);
                    }
                    if (ip.interest_transfer == false)
                        ip.interest_bank_id = null;
                    if (ip.renewable == false)
                        ip.renewal_basis = "";
                    dc.InsurancePolicy.Add(ip);

                    if (ip.Id == 0)
                    {
                        int res = dc.SaveChanges();
                        if (res > 0)
                        {
                            Success("Record Saved Successfully", true);
                            var policy = dc.InsurancePolicy.Where(x => x.guid == ip.guid).FirstOrDefault();
                            if (ip.product_id == 1 || ip.product_id == 2)//Motor
                            {
                                return RedirectToAction("PolicyVehicleInfo", new { CId = policy.Id, Id = 0 });
                            }
                            else if (ip.product_id == 3)//Fire Insurance
                            {
                                return RedirectToAction("PolicyPropertyInfo", new { CId = policy.Id, Id = 0 });
                            }

                        }
                        else
                            throw new Exception(Resp);
                    }
                    else//Update
                    {
                        dc.Entry(ip).State = EntityState.Modified;
                        dc.SaveChanges();
                    }

                    return RedirectToAction("ListPolicy");

                }
                else
                {
                    Danger("Input validation errors! Kindly check if all fields are populated properly", true);
                    //Reading Customer Recruiter
                    var recruiter = (from r in dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                    var rec = (from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
                    recruiter.AddRange(rec);
                    ViewBag.Recruiters = recruiter;
                    var insurers = (from a in dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                    ViewBag.Insurers = insurers;
                    var products = (from a in dc.Insurance_Product.OrderBy(x => x.product_name).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
                    ViewBag.Products = products;
                    var clients = (from a in dc.Client.OrderBy(x => x.client_name).ToList() select new SelectListItem { Text = a.client_name, Value = a.Id.ToString() }).ToList();
                    ViewBag.Clients = clients;
                    var banks = (from a in dc.Bank.OrderBy(x => x.BankName).ToList() select new SelectListItem { Text = a.BankName, Value = a.Id.ToString() }).ToList();
                    ViewBag.Banks = banks;
                    return View("PolicyInfo",ip);
                }
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View(ip);
        }

        public ActionResult Renew(int Id = 0)
        {
            InsurancePolicy insurancePolicy = this.dc.InsurancePolicy.Where<InsurancePolicy>(x => x.Id == Id).FirstOrDefault<InsurancePolicy>();
            if (insurancePolicy == null)
                return (ActionResult)this.Content("1");
            this.PolicyBag(0);
            insurancePolicy.renewed = false;
            insurancePolicy.Id = 0;
            insurancePolicy.interest_transfer = false;
            insurancePolicy.invoiceable = false;
            insurancePolicy.renewable = true;
            insurancePolicy.renewal_basis = "";
            insurancePolicy.interest_bank_id = 0;
            insurancePolicy.receipt_no = (string)null;
            insurancePolicy.payment_date = DateTime.Now;
            insurancePolicy.policy_no = "";
            insurancePolicy.amendment_no = (string)null;
            insurancePolicy.payment_mode = (string)null;
            insurancePolicy.total_paid = new int?();
            insurancePolicy.net_premium = 0;
            insurancePolicy.renewal_policy_id = new int?(Id);
            insurancePolicy.policy_type = "Renewal";
            // ISSUE: reference to a compiler-generated field
            ViewBag.renewed = true;
 return (ActionResult)this.PartialView("PolicyInfo", (object)insurancePolicy);
        }


        public ActionResult CreatePolicy(InsurancePolicy ip)
        {
            bool flag1 = ip.Id == 0;
            bool flag2 = ip.renewal_policy_id != null & flag1;
            try
            {
                if (this.ModelState.IsValid)
                {
                    ip.entry_user = this.AppUserData.Login;
                    if (ip.renewable && ip.renewal_basis == "")
                    {
                        this.Danger("Select Renewable Basis", true);
                        this.PolicyBag(ip.Id);
                        return (ActionResult)this.PartialView("PolicyInfo", (object)ip);
                    }
                    if (ip.interest_transfer)
                    {
                        int? interestBankId = ip.interest_bank_id;
                        int num = 0;
                        if (interestBankId.GetValueOrDefault() == num & interestBankId.HasValue)
                        {
                            this.Danger("Select Interest Transfer Bank", true);
                            this.PolicyBag(ip.Id);
                            return PartialView("PolicyInfo", ip);
                        }
                    }
                    if (!ip.interest_transfer)
                        ip.interest_bank_id = new int?();
                    if (!ip.renewable)
                        ip.renewal_basis = "";
                    this.dc.InsurancePolicy.Add(ip);
                    if (flag1)
                    {
                        ip.entry_date = DateTime.Now;
                        if (this.dc.SaveChanges() > 0)
                        {
                            this.Success("Record Saved Successfully", true);
                            if (flag2)
                            {
                                InsurancePolicy entity = this.dc.InsurancePolicy.Find((object)ip.renewal_policy_id);
                                if (entity != null)
                                {
                                    entity.renewed = true;
                                    this.dc.Entry<InsurancePolicy>(entity).State = EntityState.Modified;
                                    this.dc.SaveChanges();
                                }
                            }
                            if (ip.product_id == 1 || ip.product_id == 2)
                            {
                                if (flag2)
                                {
                                    this.dc.Policy_Vehicle.Where<Policy_Vehicle>(x => (int?)x.contract_id == ip.renewal_policy_id).ToList<Policy_Vehicle>().ForEach((Action<Policy_Vehicle>)(v =>
                                    {
                                        v.contract_id = ip.Id;
                                        this.dc.Policy_Vehicle.Add(v);
                                    }));
                                    this.dc.SaveChanges();
                                }
                                return this.RedirectToAction("ListPolicyVehicle", (object)new
                                {
                                    CId = ip.Id
                                });
                            }
                            if (ip.product_id == 3)
                            {
                                this.dc.Policy_Property.Where<Policy_Property>(x => (int?)x.contract_id == ip.renewal_policy_id).ToList<Policy_Property>().ForEach((Action<Policy_Property>)(v =>
                                {
                                    v.contract_id = ip.Id;
                                    this.dc.Policy_Property.Add(v);
                                }));
                                this.dc.SaveChanges();
                                return this.RedirectToAction("ListPolicyProperty", (object)new
                                {
                                    CId = ip.Id
                                });
                            }
                        }
                    }
                    else
                    {
                        this.dc.Entry<InsurancePolicy>(ip).State = EntityState.Modified;
                        this.dc.SaveChanges();
                    }
                    if (flag1)
                        return (ActionResult)this.RedirectToAction("ListPolicy");
                    return (ActionResult)this.Content("1");
                }
                this.Danger("Input validation errors! Kindly check if all fields are populated properly", true);
                this.PolicyBag(ip.Id);
            }
            catch (Exception ex)
            {
                this.Danger(ex.Message, true);
            }
            this.PolicyBag(ip.Id);
            if (!flag1)
                return this.PartialView("PolicyInfo", ip);
            // ISSUE: reference to a compiler-generated field
         
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            return this.View("PolicyInfo", ip);
        }
        private void PolicyBag(int Id)
        {
            //Reading Customer Recruiter
            var recruiter = (from r in dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
            var rec = (from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
            recruiter.AddRange(rec);
            ViewBag.Recruiters = recruiter;
            var insurers = (from a in dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
            ViewBag.Insurers = insurers;
            var products = (from a in dc.Insurance_Product.OrderBy(x => x.product_name).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
            ViewBag.Products = products;
            var clients = (from a in dc.Client.OrderBy(x => x.client_name).ToList() select new SelectListItem { Text = a.client_name, Value = a.Id.ToString() }).ToList();
            ViewBag.Clients = clients;
            var banks = (from a in dc.Bank.OrderBy(x => x.BankName).ToList() select new SelectListItem { Text = a.BankName, Value = a.Id.ToString() }).ToList();
            ViewBag.Banks = banks;
            ViewBag.UserId = Id;
        }

        [HttpPost]
        public ActionResult CreatePolicyProperty(eBroker.Policy_Property ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string Resp = "";
                    dc.Policy_Property.Add(ip);
                    if (ip.Id == 0)
                    {
                        ip.entry_date = DateTime.Now;
                        int res = dc.SaveChanges();
                        if (res > 0)
                            Success("Property Details Saved Successfully", true);
                        else
                            throw new Exception(Resp);
                    }
                    else//Update
                    {
                        dc.Entry(ip).State = EntityState.Modified;
                        dc.SaveChanges();
                    }
                    return Content("1");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            this.PolicyPropertyInfo(ip.contract_id, ip.Id);
            return PartialView("PolicyPropertyInfo", ip);
        }

        [HttpPost]
        public ActionResult CreatePolicyVehicle(eBroker.Policy_Vehicle ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string Resp = "";
                    dc.Policy_Vehicle.Add(ip);
                    if (ip.Id == 0)
                    {
                        ip.entry_date = DateTime.Now;
                        int res = dc.SaveChanges();
                        if (res > 0)
                        {
                            Success("Vehicle Details Saved Successfully", true);
                        }
                        else
                            throw new Exception(Resp);
                    }
                    else//Update
                    {
                        dc.Entry(ip).State = EntityState.Modified;
                        dc.SaveChanges();
                    }
                    return Content("1");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            this.PolicyVehicleInfo(ip.contract_id, ip.Id);
            return PartialView("PolicyVehicleInfo", ip);
        }


        [HttpGet]
        public ActionResult PolicyInfo(int Id = 0)
        {
            try
            {
                InsurancePolicy insurancePolicy = this.dc.InsurancePolicy.Where<InsurancePolicy>(x => x.Id == Id).FirstOrDefault<InsurancePolicy>();
                if (insurancePolicy == null)
                {
                    insurancePolicy = new InsurancePolicy();
                    insurancePolicy.entry_date = DateTime.Now;
                    insurancePolicy.entry_user = this.AppUserData.Login;
                    insurancePolicy.amendment_no = "000";
                    insurancePolicy.Id = 0;
                    insurancePolicy.effective_dt = DateTime.Now;
                    insurancePolicy.expiry_dt = DateTime.Now.AddYears(1);
                    insurancePolicy.payment_date = DateTime.Now;
                    insurancePolicy.guid = Guid.NewGuid().ToString();
                    insurancePolicy.renewed = false;
                    insurancePolicy.renewable = true;
                    insurancePolicy.invoiceable = true;
                }
                this.PolicyBag(Id);
                return this.PartialView(insurancePolicy);
            }
            catch (Exception ex)
            {
                this.Danger(ex.Message, true);
            }
            return this.PartialView();
        }

        [HttpGet]
        public ActionResult PolicyInfoView(int Id)
        {
            try
            {
                var Model = dc.InsurancePolicy.Where(x => x.Id == Id).FirstOrDefault();

                //Reading Customer Recruiter
                var recruiter = (from r in dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                var rec = (from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
                recruiter.AddRange(rec);
                ViewBag.Recruiters = recruiter;
                var insurers = (from a in dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                ViewBag.Insurers = insurers;
                var products = (from a in dc.Insurance_Product.OrderBy(x => x.product_name).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
                ViewBag.Products = products;
                var clients = (from a in dc.Client.OrderBy(x => x.client_name).ToList() select new SelectListItem { Text = a.client_name, Value = a.Id.ToString() }).ToList();
                ViewBag.Clients = clients;
                var banks = (from a in dc.Bank.OrderBy(x => x.BankName).ToList() select new SelectListItem { Text = a.BankName, Value = a.Id.ToString() }).ToList();
                ViewBag.Banks = banks;

                return View(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult PolicyPropertyInfo(int CId, int Id = 0)
        {
            try
            {
                var Model = dc.Policy_Property.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new Policy_Property();
                    Model.contract_id = CId;
                    Model.entry_date = DateTime.Now;
                    Model.entry_user = AppUserData.Login;
                    Model.Id = 0;
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Customer Recruiter
                var wallMaterial = (from r in dc.Property_Wall_Material.ToList() select new SelectListItem { Text = r.WallMaterial, Value = r.WallMaterial }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                ViewBag.WallMaterial = wallMaterial;
                var roofMaterial = (from a in dc.Property_Roof_Material.ToList() select new SelectListItem { Text = a.RoofMaterial, Value = a.RoofMaterial }).ToList();
                ViewBag.RoofMaterial = roofMaterial;
                var windowMaterial = (from a in dc.Property_Window_Material.ToList() select new SelectListItem { Text = a.WindowMaterial, Value = a.WindowMaterial }).ToList();
                ViewBag.WindowMaterial = windowMaterial;
                var propertyUse = (from a in dc.Property_Use.ToList() select new SelectListItem { Text = a.PropertyUse, Value = a.PropertyUse }).ToList();
                ViewBag.PropertyUse = propertyUse;
                return PartialView(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult PolicyVehicleInfo(int CId, int Id = 0)
        {
            try
            {
                var Model = dc.Policy_Vehicle.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new Policy_Vehicle();
                    Model.contract_id = CId;
                    Model.entry_date = DateTime.Now;
                    Model.entry_user = AppUserData.Login;
                    Model.Id = 0;
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Customer Recruiter
                var vehicleUsage = (from r in dc.Vehicle_Usage.ToList() select new SelectListItem { Text = r.usage, Value = r.Id.ToString() }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                ViewBag.VehicleUsage = vehicleUsage;
                var insuranceProduct = (from a in dc.Insurance_Product.Where(x => x.category_id == 1).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
                ViewBag.InsuranceProduct = insuranceProduct;
                var vehicleClass = (from a in dc.Vehicle_Class.ToList() select new SelectListItem { Text = a.vehicle_class, Value = a.Id.ToString() }).ToList();
                ViewBag.VehicleClass = vehicleClass;
                var territoryCover = (from a in dc.Vehicle_Territorial_Limit.ToList() select new SelectListItem { Text = a.territorial_limit, Value = a.Id.ToString() }).ToList();
                ViewBag.TerritoryCover = territoryCover;
                var occupantCategory = (from a in dc.Vehicle_Occupant.ToList() select new SelectListItem { Text = a.death_amount.ToString(), Value = a.Id.ToString() }).ToList();
                ViewBag.OccupantCategory = occupantCategory;
                return PartialView(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult ListPolicyLoan(int CId)
        {
            this.PolicyLoanInfo(CId, 0);
            var policyLoans = new List<Policy_Loan_Account>();
            try
            {
                policyLoans = dc.Policy_Loan_Account.Where(x => x.contract_id == CId).ToList();
                ViewBag.PolicyId = CId;
                return View(policyLoans);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult PolicyLoanInfo(int CId, int Id = 0)
        {
            try
            {
                var Model = dc.Policy_Loan_Account.Where(x => x.Id == Id).Take(20).FirstOrDefault();
                if (Id == 0)
                {
                    Model = new Policy_Loan_Account();
                    Model.Id = 0;
                    Model.contract_id = CId;
                    Model.entered_by = AppUserData.Login;
                    Model.entry_date = DateTime.Now;
                    Model.loan_disbursement_date = DateTime.Now;
                    Model.loan_expiry_date = DateTime.Now;
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Loan Types
                var loanTypes = (from r in dc.LoanType.ToList() select new SelectListItem { Text = r.loan_type, Value = r.Id.ToString() }).ToList();
                ViewBag.LoanTypes = loanTypes;
                return PartialView(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreatePolicyLoan(eBroker.Policy_Loan_Account ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string Resp = "";
                    dc.Policy_Loan_Account.Add(ip);
                    if (ip.Id == 0)
                    {
                        ip.entry_date = DateTime.Now;
                        ip.entered_by = this.AppUserData.Login;
                        int res = dc.SaveChanges();
                        if (res > 0)
                            Success("Record Saved Successfully", true);
                        else
                            throw new Exception(Resp);
                    }
                    else//Update
                    {
                        dc.Entry(ip).State = EntityState.Modified;
                        dc.SaveChanges();
                    }

                    return Content("1");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            PolicyLoanInfo(ip.contract_id, ip.Id);
            return PartialView("PolicyLoanInfo", ip);
        }

        public ActionResult ExportToExcel(int ccode = 0)
        {
            try
            {
                if (ccode == 0)
                    Toolkit.ExportListUsingEPPlus("select * from Vw_Policy_Report", "Policy Listing");
                else
                    Toolkit.ExportListUsingEPPlus("select a.* from Vw_Policy_Report a, insurance_policy b where a.contract_id=b.contract_id and b.client_id=" + ccode, "Policy Listing");
                return View();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult ListBankPolicy(int BankId, string query, string ExpStartDate, string ExpEndDate, string EffStartDate, string EffEndDate)
        {
            var policy = new List<Vw_Policy_Report>();
            try
            {
                DateTime _ExpStartDate = new DateTime();
                DateTime _ExpEndDate = new DateTime();
                DateTime _EffStartDate = new DateTime();
                DateTime _EffEndDate = new DateTime();

                DateTime.TryParse(ExpStartDate, out _ExpStartDate);
                DateTime.TryParse(ExpEndDate, out _ExpEndDate);
                DateTime.TryParse(EffStartDate, out _EffStartDate);
                DateTime.TryParse(EffEndDate, out _EffEndDate);

                if (string.IsNullOrEmpty(query))
                {
                    policy = dc.Vw_Policy_Report.OrderByDescending(x => x.Id).ToList();
                    if (policy == null || policy.Count == 0)
                        return View(policy);
                }
                else
                    policy = dc.Vw_Policy_Report.Where(x => x.client_name.Contains(query) || x.product_name.Contains(query) || x.insurer.Contains(query) || x.policy_no.Contains(query)).ToList();
                if (!string.IsNullOrEmpty(ExpStartDate))
                    policy = policy.Where(x => x.expiry_dt >= _ExpStartDate).ToList();
                if (!string.IsNullOrEmpty(ExpEndDate))
                    policy = policy.Where(x => x.expiry_dt <= _ExpEndDate).ToList();
                if (!string.IsNullOrEmpty(EffStartDate))
                    policy = policy.Where(x => x.expiry_dt >= _EffStartDate).ToList();
                if (!string.IsNullOrEmpty(EffEndDate))
                    policy = policy.Where(x => x.expiry_dt <= _EffEndDate).ToList();
                return View(policy);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

    }
}
