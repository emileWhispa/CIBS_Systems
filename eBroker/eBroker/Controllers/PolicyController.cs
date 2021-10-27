using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;
using eBroker.Services;

namespace eBroker.Controllers
{
    public class PolicyController : BaseController
    {
        BrokerDataContext _dc = new BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListPolicy(string query, string expStartDate, string expEndDate, string effStartDate, string effEndDate)
        {
            PolicyInfo(0);
            ViewBag.renewed = true;
            var policy = new List<Vw_Policy_Report>();
            try
            {
                DateTime expStartDate1 = new DateTime();
                DateTime expEndDate1 = new DateTime();
                DateTime effStartDate1 = new DateTime();
                DateTime effEndDate1 = new DateTime();

                DateTime.TryParse(expStartDate, out expStartDate1);
                DateTime.TryParse(expEndDate, out expEndDate1);
                DateTime.TryParse(effStartDate, out effStartDate1);
                DateTime.TryParse(effEndDate, out effEndDate1);
                ViewBag.query = query;
                ViewBag.expStartDate = expStartDate;
                ViewBag.expEndDate = expEndDate;
                ViewBag.effStartDate = effStartDate;
                ViewBag.effEndDate = effEndDate;
                ViewBag.AppUserData = AppUserData;
                if (string.IsNullOrEmpty(query))
                {
                    policy = _dc.Vw_Policy_Report.Where(x=>x.interest_bank_id == AppUserData.CompanyID || AppUserData.Category != "3").OrderByDescending(x => x.Id).ToList();
                    if ( policy.Count == 0)
                        return View(policy);
                }
                else
                    policy = _dc.Vw_Policy_Report.Where(x=>x.interest_bank_id == AppUserData.CompanyID || AppUserData.Category != "3").Where(x => x.client_name.Contains(query) || x.product_name.Contains(query) || x.insurer.Contains(query) || x.policy_no.Contains(query)).ToList();
                if (!string.IsNullOrEmpty(expStartDate))
                    policy = policy.Where(x => x.expiry_dt >= expStartDate1).Where(x=>x.interest_bank_id == AppUserData.CompanyID || AppUserData.Category != "3").ToList();
                //else if (!string.IsNullOrEmpty(ExpStartDate) && string.IsNullOrEmpty(query))
                //    policy = dc.Vw_Policy_Report.Where(x => x.expiry_dt >= _ExpStartDate).ToList();
                if (!string.IsNullOrEmpty(expEndDate))
                    policy = policy.Where(x => x.expiry_dt <= expEndDate1).Where(x=>x.interest_bank_id == AppUserData.CompanyID || AppUserData.Category != "3").ToList();
                //else if (!string.IsNullOrEmpty(ExpEndDate) && string.IsNullOrEmpty(query))
                //    policy = dc.Vw_Policy_Report.Where(x => x.expiry_dt <= _ExpEndDate).ToList();
                if (!string.IsNullOrEmpty(effStartDate))
                    policy = policy.Where(x => x.effective_dt >= effStartDate1).Where(x=>x.interest_bank_id == AppUserData.CompanyID || AppUserData.Category != "3").ToList();
                //else if (!string.IsNullOrEmpty(EffStartDate) && string.IsNullOrEmpty(query))
                //    policy = dc.Vw_Policy_Report.Where(x => x.expiry_dt >= _EffStartDate).ToList();
                if (!string.IsNullOrEmpty(effEndDate))
                    policy = policy.Where(x => x.effective_dt <= effEndDate1).Where(x=>x.interest_bank_id == AppUserData.CompanyID || AppUserData.Category != "3").ToList();
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
                policy = _dc.InsurancePolicy.Where(x => x.client_id == ccode).OrderByDescending(x => x.Id).ToList();
                ViewBag.ClientInfo = _dc.Client.Where(x => x.Id == ccode).FirstOrDefault();
                return View(policy);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }


        public ActionResult PolicyItems(int cId, int dis = 0)
        {
            try
            {
                var policy = _dc.InsurancePolicy.Where(x => x.Id == cId).FirstOrDefault();
                ViewBag.DisableAddNew = dis;
                if (policy.InsuranceProducts.category_id == 1)//Motor
                {
                    return RedirectToAction("ListPolicyVehicle", new { CId = cId });
                }
                else if (policy.InsuranceProducts.category_id == 2)//Fire
                {
                    return RedirectToAction("ListPolicyProperty", new { CId = cId });
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

        public ActionResult ListPolicyVehicle(int cId)
        {

            PolicyVehicleInfo(cId, 0);
            var policyVehicle = new List<Policy_Vehicle>();
            try
            {
                policyVehicle = _dc.Policy_Vehicle.Where(x => x.contract_id == cId).ToList();
                ViewBag.PolicyId = cId;
                ViewBag.AppUserData = AppUserData;
                return View(policyVehicle);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }
        public ActionResult ListPolicyProperty(int cId)
        {
            PolicyPropertyInfo(cId, 0);
            var policyProperty = new List<Policy_Property>();
            try
            {
                policyProperty = _dc.Policy_Property.Where(x => x.contract_id == cId).ToList();
                ViewBag.PolicyId = cId;
                ViewBag.AppUserData = AppUserData;
                return View(policyProperty);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreatePolicy2(InsurancePolicy ip)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    string resp = "";
                    //Data Validation
                    if (ip.renewable && ip.renewal_basis == "")
                    {
                        Danger("Select Renewable Basis", true);
                        //Reading Customer Recruiter
                        var recruiter = (from r in _dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                        var rec = (from a in _dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
                        recruiter.AddRange(rec);
                        ViewBag.Recruiters = recruiter;
                        var insurers = (from a in _dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Insurers = insurers;
                        var products = (from a in _dc.Insurance_Product.OrderBy(x => x.product_name).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Products = products;
                        var clients = (from a in _dc.Client.OrderBy(x => x.client_name).ToList() select new SelectListItem { Text = a.client_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Clients = clients;
                        var banks = (from a in _dc.Bank.OrderBy(x => x.BankName).ToList() select new SelectListItem { Text = a.BankName, Value = a.Id.ToString() }).ToList();
                        ViewBag.Banks = banks;
                        return View("PolicyInfo", ip);
                    }
                    if (ip.interest_transfer && ip.interest_bank_id == 0)
                    {
                        Danger("Select Interest Transfer Bank", true);
                        //Reading Customer Recruiter
                        var recruiter = (from r in _dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                        var rec = (from a in _dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
                        recruiter.AddRange(rec);
                        ViewBag.Recruiters = recruiter;
                        var insurers = (from a in _dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Insurers = insurers;
                        var products = (from a in _dc.Insurance_Product.OrderBy(x => x.product_name).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Products = products;
                        var clients = (from a in _dc.Client.OrderBy(x => x.client_name).ToList() select new SelectListItem { Text = a.client_name, Value = a.Id.ToString() }).ToList();
                        ViewBag.Clients = clients;
                        var banks = (from a in _dc.Bank.OrderBy(x => x.BankName).ToList() select new SelectListItem { Text = a.BankName, Value = a.Id.ToString() }).ToList();
                        ViewBag.Banks = banks;
                        return View("PolicyInfo", ip);
                    }
                    if (ip.interest_transfer == false)
                        ip.interest_bank_id = null;
                    if (ip.renewable == false)
                        ip.renewal_basis = "";
                    _dc.InsurancePolicy.Add(ip);

                    if (ip.Id == 0)
                    {
                        int res = _dc.SaveChanges();
                        if (res > 0)
                        {
                            Success("Record Saved Successfully", true);
                            var policy = _dc.InsurancePolicy.Where(x => x.guid == ip.guid).FirstOrDefault();
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
                            throw new Exception(resp);
                    }
                    else//Update
                    {
                        _dc.Entry(ip).State = EntityState.Modified;
                        _dc.SaveChanges();
                    }

                    return RedirectToAction("ListPolicy");

                }
                else
                {
                    Danger("Input validation errors! Kindly check if all fields are populated properly", true);
                    //Reading Customer Recruiter
                    var recruiter = (from r in _dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                    var rec = (from a in _dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
                    recruiter.AddRange(rec);
                    ViewBag.Recruiters = recruiter;
                    var insurers = (from a in _dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                    ViewBag.Insurers = insurers;
                    var products = (from a in _dc.Insurance_Product.OrderBy(x => x.product_name).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
                    ViewBag.Products = products;
                    var clients = (from a in _dc.Client.OrderBy(x => x.client_name).ToList() select new SelectListItem { Text = a.client_name, Value = a.Id.ToString() }).ToList();
                    ViewBag.Clients = clients;
                    var banks = (from a in _dc.Bank.OrderBy(x => x.BankName).ToList() select new SelectListItem { Text = a.BankName, Value = a.Id.ToString() }).ToList();
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

        public ActionResult Renew(int id = 0)
        {
            InsurancePolicy insurancePolicy = _dc.InsurancePolicy.Where(x => x.Id == id).FirstOrDefault();
            if (insurancePolicy == null)
                return (ActionResult)Content("1");
            PolicyBag(0);
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
            insurancePolicy.renewal_policy_id = new int?(id);
            insurancePolicy.policy_type = "Renewal";
            // ISSUE: reference to a compiler-generated field
            ViewBag.renewed = true;
 return (ActionResult)PartialView("PolicyInfo", (object)insurancePolicy);
        }


        public ActionResult CreatePolicy(InsurancePolicy ip)
        {
            bool flag1 = ip.Id == 0;
            bool flag2 = ip.renewal_policy_id != null && flag1;
            try
            {
                if (ModelState.IsValid)
                {
                    ip.entry_user = AppUserData.Login;
                    if (ip.renewable && ip.renewal_basis == "")
                    {
                        Danger("Select Renewable Basis", true);
                        PolicyBag(ip.Id);
                        return (ActionResult)PartialView("PolicyInfo", (object)ip);
                    }
                    if (ip.interest_transfer)
                    {
                        int? interestBankId = ip.interest_bank_id;
                        int num = 0;
                        if (interestBankId.GetValueOrDefault() == num & interestBankId.HasValue)
                        {
                            Danger("Select Interest Transfer Bank", true);
                            PolicyBag(ip.Id);
                            return PartialView("PolicyInfo", ip);
                        }
                    }
                    if (!ip.interest_transfer)
                        ip.interest_bank_id = new int?();
                    if (!ip.renewable)
                        ip.renewal_basis = "";
                    _dc.InsurancePolicy.Add(ip);
                    if (flag1)
                    {
                        ip.entry_date = DateTime.Now;
                        if (_dc.SaveChanges() > 0)
                        {
                            Success("Record Saved Successfully", true);
                            if (flag2)
                            {
                                InsurancePolicy entity = _dc.InsurancePolicy.Find((object)ip.renewal_policy_id);
                                if (entity != null)
                                {
                                    entity.renewed = true;
                                    _dc.Entry(entity).State = EntityState.Modified;
                                    _dc.SaveChanges();
                                }
                            }
                            if (ip.product_id == 1 || ip.product_id == 2)
                            {
                                if (flag2)
                                {
                                    _dc.Policy_Vehicle.Where(x => (int?)x.contract_id == ip.renewal_policy_id).ToList().ForEach((Action<Policy_Vehicle>)(v =>
                                    {
                                        v.contract_id = ip.Id;
                                        _dc.Policy_Vehicle.Add(v);
                                    }));
                                    _dc.SaveChanges();
                                }
                                return RedirectToAction("ListPolicyVehicle", (object)new
                                {
                                    CId = ip.Id
                                });
                            }
                            if (ip.product_id == 3)
                            {
                                _dc.Policy_Property.Where(x => (int?)x.contract_id == ip.renewal_policy_id).ToList().ForEach((Action<Policy_Property>)(v =>
                                {
                                    v.contract_id = ip.Id;
                                    _dc.Policy_Property.Add(v);
                                }));
                                _dc.SaveChanges();
                                return RedirectToAction("ListPolicyProperty", (object)new
                                {
                                    CId = ip.Id
                                });
                            }
                        }
                    }
                    else
                    {
                        _dc.Entry(ip).State = EntityState.Modified;
                        _dc.SaveChanges();
                    }
                    if (flag1)
                        return (ActionResult)RedirectToAction("ListPolicy");
                    return (ActionResult)Content("1");
                }
                Danger("Input validation errors! Kindly check if all fields are populated properly", true);
                PolicyBag(ip.Id);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            PolicyBag(ip.Id);
            if (!flag1)
                return PartialView("PolicyInfo", ip);
            // ISSUE: reference to a compiler-generated field
         
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            return View("PolicyInfo", ip);
        }
        private void PolicyBag(int id)
        {
            //Reading Customer Recruiter
            var recruiter = (from r in _dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
            var rec = (from a in _dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
            recruiter.AddRange(rec);
            ViewBag.Recruiters = recruiter;
            var insurers = (from a in _dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
            ViewBag.Insurers = insurers;
            var products = (from a in _dc.Insurance_Product.OrderBy(x => x.product_name).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
            ViewBag.Products = products;
            var clients = (from a in _dc.Client.OrderBy(x => x.client_name).ToList() select new SelectListItem { Text = a.client_name, Value = a.Id.ToString() }).ToList();
            ViewBag.Clients = clients;
            var banks = (from a in _dc.Bank.Where(x=>x.Id == AppUserData.CompanyID || AppUserData.Category != "3").OrderBy(x => x.BankName).ToList() select new SelectListItem { Text = a.BankName, Value = a.Id.ToString() }).ToList();
            ViewBag.Banks = banks;
            ViewBag.UserId = id;
        }

        public ActionResult DeletePolicy(int id)
        {
            InsurancePolicy ins = _dc.InsurancePolicy.Find(id);
            if( ins != null)
            {
                List<Invoice_Detail> list = _dc.Invoice_Detail.Where(e => e.contract_id == id).ToList();
                _dc.Invoice_Detail.RemoveRange(list);
                _dc.InsurancePolicy.Remove(ins);
                _dc.SaveChanges();
            }
            return RedirectToAction("ListPolicy");
        }

        [HttpPost]
        public ActionResult CreatePolicyProperty(Policy_Property ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string resp = "";
                    _dc.Policy_Property.Add(ip);
                    if (ip.Id == 0)
                    {
                        ip.entry_date = DateTime.Now;
                        int res = _dc.SaveChanges();
                        if (res > 0)
                            Success("Property Details Saved Successfully", true);
                        else
                            throw new Exception(resp);
                    }
                    else//Update
                    {
                        _dc.Entry(ip).State = EntityState.Modified;
                        _dc.SaveChanges();
                    }
                    return Content("1");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            PolicyPropertyInfo(ip.contract_id, ip.Id);
            return PartialView("PolicyPropertyInfo", ip);
        }

        [HttpPost]
        public ActionResult CreatePolicyVehicle(Policy_Vehicle ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string resp = "";
                    _dc.Policy_Vehicle.Add(ip);
                    if (ip.Id == 0)
                    {
                        ip.entry_date = DateTime.Now;
                        int res = _dc.SaveChanges();
                        if (res > 0)
                        {
                            Success("Vehicle Details Saved Successfully", true);
                        }
                        else
                            throw new Exception(resp);
                    }
                    else//Update
                    {
                        _dc.Entry(ip).State = EntityState.Modified;
                        _dc.SaveChanges();
                    }
                    return Content("1");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            PolicyVehicleInfo(ip.contract_id, ip.Id);
            return PartialView("PolicyVehicleInfo", ip);
        }


        [HttpGet]
        public ActionResult PolicyInfo(int id = 0)
        {
            try
            {
                InsurancePolicy insurancePolicy = _dc.InsurancePolicy.Where(x => x.Id == id).FirstOrDefault();
                if (insurancePolicy == null)
                {
                    insurancePolicy = new InsurancePolicy();
                    insurancePolicy.entry_date = DateTime.Now;
                    insurancePolicy.entry_user = AppUserData.Login;
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
                PolicyBag(id);
                return PartialView(insurancePolicy);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult PolicyInfoView(int id)
        {
            try
            {
                var model = _dc.InsurancePolicy.Where(x => x.Id == id).FirstOrDefault();

                //Reading Customer Recruiter
                var recruiter = (from r in _dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                var rec = (from a in _dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
                recruiter.AddRange(rec);
                ViewBag.Recruiters = recruiter;
                var insurers = (from a in _dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                ViewBag.Insurers = insurers;
                var products = (from a in _dc.Insurance_Product.OrderBy(x => x.product_name).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
                ViewBag.Products = products;
                var clients = (from a in _dc.Client.OrderBy(x => x.client_name).ToList() select new SelectListItem { Text = a.client_name, Value = a.Id.ToString() }).ToList();
                ViewBag.Clients = clients;
                var banks = (from a in _dc.Bank.OrderBy(x => x.BankName).ToList() select new SelectListItem { Text = a.BankName, Value = a.Id.ToString() }).ToList();
                ViewBag.Banks = banks;

                return View(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult PolicyPropertyInfo(int cId, int id = 0)
        {
            try
            {
                var model = _dc.Policy_Property.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new Policy_Property();
                    model.contract_id = cId;
                    model.entry_date = DateTime.Now;
                    model.entry_user = AppUserData.Login;
                    model.Id = 0;
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Customer Recruiter
                var wallMaterial = (from r in _dc.Property_Wall_Material.ToList() select new SelectListItem { Text = r.WallMaterial, Value = r.WallMaterial }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                ViewBag.WallMaterial = wallMaterial;
                var roofMaterial = (from a in _dc.Property_Roof_Material.ToList() select new SelectListItem { Text = a.RoofMaterial, Value = a.RoofMaterial }).ToList();
                ViewBag.RoofMaterial = roofMaterial;
                var windowMaterial = (from a in _dc.Property_Window_Material.ToList() select new SelectListItem { Text = a.WindowMaterial, Value = a.WindowMaterial }).ToList();
                ViewBag.WindowMaterial = windowMaterial;
                var propertyUse = (from a in _dc.Property_Use.ToList() select new SelectListItem { Text = a.PropertyUse, Value = a.PropertyUse }).ToList();
                ViewBag.PropertyUse = propertyUse;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult PolicyVehicleInfo(int cId, int id = 0)
        {
            try
            {
                var model = _dc.Policy_Vehicle.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new Policy_Vehicle();
                    model.contract_id = cId;
                    model.entry_date = DateTime.Now;
                    model.entry_user = AppUserData.Login;
                    model.Id = 0;
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Customer Recruiter
                var vehicleUsage = (from r in _dc.Vehicle_Usage.ToList() select new SelectListItem { Text = r.usage, Value = r.Id.ToString() }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                ViewBag.VehicleUsage = vehicleUsage;
                var insuranceProduct = (from a in _dc.Insurance_Product.Where(x => x.category_id == 1).ToList() select new SelectListItem { Text = a.product_name, Value = a.Id.ToString() }).ToList();
                ViewBag.InsuranceProduct = insuranceProduct;
                var vehicleClass = (from a in _dc.Vehicle_Class.ToList() select new SelectListItem { Text = a.vehicle_class, Value = a.Id.ToString() }).ToList();
                ViewBag.VehicleClass = vehicleClass;
                var territoryCover = (from a in _dc.Vehicle_Territorial_Limit.ToList() select new SelectListItem { Text = a.territorial_limit, Value = a.Id.ToString() }).ToList();
                ViewBag.TerritoryCover = territoryCover;
                var occupantCategory = (from a in _dc.Vehicle_Occupant.ToList() select new SelectListItem { Text = a.death_amount.ToString(), Value = a.Id.ToString() }).ToList();
                ViewBag.OccupantCategory = occupantCategory;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult ListPolicyLoan(int cId)
        {
            PolicyLoanInfo(cId, 0);
            var policyLoans = new List<Policy_Loan_Account>();
            try
            {
                policyLoans = _dc.Policy_Loan_Account.Where(x => x.contract_id == cId).ToList();
                ViewBag.PolicyId = cId;
                ViewBag.AppUserData = AppUserData;
                return View(policyLoans);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult PolicyLoanInfo(int cId, int id = 0)
        {
            try
            {
                var model = _dc.Policy_Loan_Account.Where(x => x.Id == id).Take(20).FirstOrDefault();
                if (id == 0)
                {
                    model = new Policy_Loan_Account();
                    model.Id = 0;
                    model.contract_id = cId;
                    model.entered_by = AppUserData.Login;
                    model.entry_date = DateTime.Now;
                    model.loan_disbursement_date = DateTime.Now;
                    model.loan_expiry_date = DateTime.Now;
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Loan Types
                var loanTypes = (from r in _dc.LoanType.ToList() select new SelectListItem { Text = r.loan_type, Value = r.Id.ToString() }).ToList();
                ViewBag.LoanTypes = loanTypes;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreatePolicyLoan(Policy_Loan_Account ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string resp = "";
                    _dc.Policy_Loan_Account.Add(ip);
                    if (ip.Id == 0)
                    {
                        ip.entry_date = DateTime.Now;
                        ip.entered_by = AppUserData.Login;
                        int res = _dc.SaveChanges();
                        if (res > 0)
                            Success("Record Saved Successfully", true);
                        else
                            throw new Exception(resp);
                    }
                    else//Update
                    {
                        _dc.Entry(ip).State = EntityState.Modified;
                        _dc.SaveChanges();
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

        public ActionResult ExportToExcel(string query, string expStartDate, string expEndDate, string effStartDate, string effEndDate,int ccode = 0)
        {
            try
            {
                
                DateTime expStartDate1 = new DateTime();
                DateTime expEndDate1 = new DateTime();
                DateTime effStartDate1 = new DateTime();
                DateTime effEndDate1 = new DateTime();

                DateTime.TryParse(expStartDate, out expStartDate1);
                DateTime.TryParse(expEndDate, out expEndDate1);
                DateTime.TryParse(effStartDate, out effStartDate1);
                DateTime.TryParse(effEndDate, out effEndDate1);
                var policy = _dc.InsurancePolicy.Where(x=>true);
                policy = string.IsNullOrEmpty(query) ? _dc.InsurancePolicy.OrderByDescending(x => x.Id) : _dc.InsurancePolicy.Where(x => x.Clients.client_name.Contains(query) || x.InsuranceProducts.product_name.Contains(query) || x.Partners.company_name.Contains(query) || x.policy_no.Contains(query));
                if (!string.IsNullOrEmpty(expStartDate))
                    policy = policy.Where(x => x.expiry_dt >= expStartDate1);
                //else if (!string.IsNullOrEmpty(ExpStartDate) && string.IsNullOrEmpty(query))
                //    policy = dc.Vw_Policy_Report.Where(x => x.expiry_dt >= _ExpStartDate).ToList();
                if (!string.IsNullOrEmpty(expEndDate))
                    policy = policy.Where(x => x.expiry_dt <= expEndDate1);
                //else if (!string.IsNullOrEmpty(ExpEndDate) && string.IsNullOrEmpty(query))
                //    policy = dc.Vw_Policy_Report.Where(x => x.expiry_dt <= _ExpEndDate).ToList();
                if (!string.IsNullOrEmpty(effStartDate))
                    policy = policy.Where(x => x.effective_dt >= effStartDate1);
                //else if (!string.IsNullOrEmpty(EffStartDate) && string.IsNullOrEmpty(query))
                //    policy = dc.Vw_Policy_Report.Where(x => x.expiry_dt >= _EffStartDate).ToList();
                if (!string.IsNullOrEmpty(effEndDate))
                    policy = policy.Where(x => x.effective_dt <= effEndDate1);
                if (ccode != 0)
                    policy = policy.Where(x => x.Clients.Id == ccode);
                return File(TransactionService.PolicyToExcel(policy.ToList()).ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "transactions_" + DateTime.Now + ".xlsx");
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return RedirectToAction("ListPolicy");
        }

        public ActionResult ListBankPolicy(int bankId, string query, string expStartDate, string expEndDate, string effStartDate, string effEndDate)
        {
            var policy = new List<Vw_Policy_Report>();
            try
            {
                DateTime expStartDate1 = new DateTime();
                DateTime expEndDate1;
                DateTime effStartDate1 = new DateTime();
                DateTime effEndDate1 = new DateTime();

                DateTime.TryParse(expStartDate, out expStartDate1);
                DateTime.TryParse(expEndDate, out expEndDate1);
                DateTime.TryParse(effStartDate, out effStartDate1);
                DateTime.TryParse(effEndDate, out effEndDate1);

                if (string.IsNullOrEmpty(query))
                {
                    policy = _dc.Vw_Policy_Report.OrderByDescending(x => x.Id).ToList();
                    if (policy.Count == 0)
                        return View(policy);
                }
                else
                    policy = _dc.Vw_Policy_Report.Where(x => x.client_name.Contains(query) || x.product_name.Contains(query) || x.insurer.Contains(query) || x.policy_no.Contains(query)).ToList();
                if (!string.IsNullOrEmpty(expStartDate))
                    policy = policy.Where(x => x.expiry_dt >= expStartDate1).ToList();
                if (!string.IsNullOrEmpty(expEndDate))
                    policy = policy.Where(x => x.expiry_dt <= expEndDate1).ToList();
                if (!string.IsNullOrEmpty(effStartDate))
                    policy = policy.Where(x => x.expiry_dt >= effStartDate1).ToList();
                if (!string.IsNullOrEmpty(effEndDate))
                    policy = policy.Where(x => x.expiry_dt <= effEndDate1).ToList();
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
