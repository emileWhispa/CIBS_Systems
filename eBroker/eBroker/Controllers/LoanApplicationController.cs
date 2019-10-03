using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;

namespace eBroker.Controllers
{
    public class LoanApplicationController : BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListLoanApplications()
        {
            var banks = (from r in dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            if (AppUserData.Category == "5")//Bank Staff
            {
                banks = (from r in dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            }
            ViewBag.Banks = banks;
            return View();
        }

        public ActionResult ListLoanApplicationView(string txtApplicationStartDate, string txtApplicationEndDate, string BankName)
        {
            var loanApp = new List<LoanApplication>();
            try
            {
                if (string.IsNullOrEmpty(BankName))
                {
                    Danger("Kindly select the bank", true);
                    return RedirectToAction("ListLoanApplications");
                }
                if (string.IsNullOrEmpty(txtApplicationStartDate))
                {
                    Danger("Kindly select the Start Date", true);
                    return RedirectToAction("ListLoanApplications");
                }
                if (string.IsNullOrEmpty(txtApplicationEndDate))
                {
                    Danger("Kindly select the End Date", true);
                    return RedirectToAction("ListLoanApplications");
                }
                DateTime _startDate = DateTime.Parse(txtApplicationStartDate);
                DateTime _endDate = DateTime.Parse(txtApplicationEndDate);
                loanApp = dc.LoanApplication.Where(x => x.ApplicationDate >= _startDate && x.ApplicationDate <= _endDate && x.Banks.BankName == BankName).ToList();
                ViewBag.BankName = BankName;
                ViewBag.StartDate = _startDate.ToShortDateString();
                ViewBag.EndDate = _endDate.ToShortDateString();
                return View(loanApp);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult LoanApplicationInfo(string BankName, string StartDate, string EndDate, int Id = 0)
        {
            try
            {
                var Model = dc.LoanApplication.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new LoanApplication();
                }
                ViewBag.BankName = BankName;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;

                return View(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }


        [HttpGet]
        public ActionResult LoanApplicationPropertyInfo(int AId, string BankName, string StartDate, string EndDate, int Id = 0)
        {
            try
            {
                var Model = dc.LoanApplication_Property.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new LoanApplication_Property();
                    Model.loan_application_id = AId;
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
                ViewBag.BankName = BankName;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;

                return View(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult LoanApplicationPropertyInfoView(int AId, int Id = 0)
        {
            try
            {
                var Model = dc.LoanApplication_Property.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new LoanApplication_Property();
                    Model.loan_application_id = AId;
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
                var insurers = (from a in dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                ViewBag.Insurers = insurers;

                return View(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }


        [HttpGet]

        public ActionResult LoanApplicationVehicleInfo(int AId, string BankName, string StartDate, string EndDate, int Id = 0)
        {
            try
            {
                var Model = dc.LoanApplication_Vehicle.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new LoanApplication_Vehicle();
                    Model.loan_application_id = AId;
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
                ViewBag.BankName = BankName;
                ViewBag.StartDate = StartDate;
                ViewBag.EndDate = EndDate;

                return View(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult LoanApplicationVehicleInfoView(int AId, int Id = 0)
        {
            try
            {
                var Model = dc.LoanApplication_Vehicle.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new LoanApplication_Vehicle();
                    Model.loan_application_id = AId;
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
                var insurers = (from a in dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                ViewBag.Insurers = insurers;

                return View(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateLoanApplication(eBroker.LoanApplication lp, string BankName, string StartDate, string EndDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (lp.CollateralType == null)
                    {
                        Danger("Collateral Type cannot be empty");
                        ViewBag.BankName = BankName;
                        ViewBag.StartDate = StartDate;
                        ViewBag.EndDate = EndDate;

                        return View("LoanApplicationInfo", lp);
                    }

                    dc.LoanApplication.Add(lp);

                    dc.Entry(lp).State = EntityState.Modified;
                    dc.SaveChanges();
                    if (lp.CollateralType == "None")
                        return RedirectToAction("ListLoanApplicationView", new { BankName = BankName, txtApplicationStartDate = StartDate, txtApplicationEndDate = EndDate });
                    else if (lp.CollateralType == "Vehicle")
                        return RedirectToAction("ListLoanApplicationVehicles", new { loanAppId = lp.Id, BankName = BankName, StartDate = StartDate, EndDate = EndDate });
                    else if (lp.CollateralType == "Property")
                        return RedirectToAction("ListLoanApplicationProperties", new { loanAppId = lp.Id, BankName = BankName, StartDate = StartDate, EndDate = EndDate });
                    else
                        return RedirectToAction("ListLoanApplicationView", new { BankName = BankName, txtApplicationStartDate = StartDate, txtApplicationEndDate = EndDate });

                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View(lp);
        }

        [HttpPost]
        public ActionResult CreateLoanApplicationProperty(eBroker.LoanApplication_Property ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string Resp = "";
                    dc.LoanApplication_Property.Add(ip);
                    if (ip.Id == 0)
                    {
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

                    return RedirectToAction("ListLoanApplicationProperties", new { loanAppId = ip.loan_application_id });
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View(ip);
        }

        [HttpPost]
        public ActionResult SetPremiumLoanApplicationProperty(eBroker.LoanApplication_Property ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Getting the record being updated
                    var prop = dc.LoanApplication_Property.Where(x => x.Id == ip.Id).FirstOrDefault();
                    prop.PremiumDate = DateTime.Now;
                    prop.PremiumAmount = ip.PremiumAmount;
                    prop.PremiumUser = AppUserData.Login;
                    prop.Insurer_Id = ip.Insurer_Id;
                    dc.LoanApplication_Property.Add(prop);
                    dc.Entry(prop).State = EntityState.Modified;
                    dc.SaveChanges();

                    return RedirectToAction("ListLoanApplicationPropertiesView", new { loanAppId = ip.loan_application_id });
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View(ip);
        }

        [HttpPost]
        public ActionResult CreateLoanApplicationVehicle(eBroker.LoanApplication_Vehicle ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string Resp = "";
                    dc.LoanApplication_Vehicle.Add(ip);
                    if (ip.Id == 0)
                    {
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
                    return RedirectToAction("ListLoanApplicationVehicles", new { loanAppId = ip.loan_application_id });
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View(ip);
        }

        [HttpPost]
        public ActionResult SetPremiumLoanApplicationVehicle(eBroker.LoanApplication_Vehicle ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Getting the record being updated
                    var veh = dc.LoanApplication_Vehicle.Where(x => x.Id == ip.Id).FirstOrDefault();
                    veh.PremiumDate = DateTime.Now;
                    veh.PremiumAmount = ip.PremiumAmount;
                    veh.PremiumUser = AppUserData.Login;
                    veh.Insurer_Id = ip.Insurer_Id;
                    dc.LoanApplication_Vehicle.Add(veh);
                    dc.Entry(veh).State = EntityState.Modified;
                    dc.Entry(veh).State = EntityState.Modified;
                    dc.SaveChanges();
                    return RedirectToAction("ListLoanApplicationVehiclesView", new { loanAppId = ip.loan_application_id });
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View(ip);
        }

        [HttpGet]
        public ActionResult ListLoanApplicationVehicles(int loanAppId, string BankName, string StartDate, string EndDate)
        {
            var vehicles = dc.LoanApplication_Vehicle.Where(x => x.loan_application_id == loanAppId).ToList();
            ViewBag.LoanApplicationId = loanAppId;
            ViewBag.BankName = BankName;
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;

            return View(vehicles);
        }

        public ActionResult ListLoanApplicationVehiclesView(int loanAppId)
        {
            var vehicles = dc.LoanApplication_Vehicle.Where(x => x.loan_application_id == loanAppId).ToList();
            ViewBag.LoanApplicationId = loanAppId;
            return View(vehicles);
        }


        [HttpGet]
        public ActionResult ListLoanApplicationProperties(int loanAppId, string BankName, string StartDate, string EndDate)
        {
            var properties = dc.LoanApplication_Property.Where(x => x.loan_application_id == loanAppId).ToList();
            ViewBag.LoanApplicationId = loanAppId;
            ViewBag.BankName = BankName;
            ViewBag.StartDate = StartDate;
            ViewBag.EndDate = EndDate;

            return View(properties);
        }

        public ActionResult ListLoanApplicationPropertiesView(int loanAppId)
        {
            var properties = dc.LoanApplication_Property.Where(x => x.loan_application_id == loanAppId).ToList();
            ViewBag.LoanApplicationId = loanAppId;
            return View(properties);
        }


        [HttpGet]
        public ActionResult LoanApplicationCollateral(int loanAppId, string BankName, string StartDate, string EndDate)
        {
            var loanApplication = dc.LoanApplication.Where(x => x.Id == loanAppId).FirstOrDefault();
            if (BankName != null)
            {
                if (loanApplication.CollateralType == "Vehicle")
                    return RedirectToAction("ListLoanApplicationVehicles", new { loanAppId = loanAppId, BankName = BankName, StartDate = StartDate, EndDate = EndDate });
                else if (loanApplication.CollateralType == "Property")
                    return RedirectToAction("ListLoanApplicationProperties", new { loanAppId = loanAppId, BankName = BankName, StartDate = StartDate, EndDate = EndDate });
                else if (loanApplication.CollateralType == "None")
                {
                    Danger("No Collateral Item is supposed to be attached. Collateral Type=[None]");
                    return RedirectToAction("ListLoanApplicationView", new { BankName = BankName, txtApplicationStartDate = StartDate, txtApplicationEndDate = EndDate });
                }
                else if (loanApplication.CollateralType == null)
                {
                    Danger("No Collateral Type defined yet. Kindly edit the record and select the Collateral Type first");
                    return RedirectToAction("ListLoanApplicationView", new { BankName = BankName, txtApplicationStartDate = StartDate, txtApplicationEndDate = EndDate });
                }
            }
            else//Submitted loan application
            {
                if (loanApplication.CollateralType == "Vehicle")
                    return RedirectToAction("ListLoanApplicationVehiclesView", new { loanAppId = loanAppId });
                else if (loanApplication.CollateralType == "Property")
                    return RedirectToAction("ListLoanApplicationPropertiesView", new { loanAppId = loanAppId });
                else if (loanApplication.CollateralType == "Stock")
                {
                    Danger("No Collateral Item is supposed to be attached. Collateral Type=[None]");
                    return RedirectToAction("ListLoanApplicationView", new { BankName = BankName, txtApplicationStartDate = StartDate, txtApplicationEndDate = EndDate });
                }
                else if (loanApplication.CollateralType == null)
                {
                    Danger("No Collateral Type defined yet. Kindly edit the record and select the Collateral Type first");
                    return RedirectToAction("ListLoanApplicationView", new { BankName = BankName, txtApplicationStartDate = StartDate, txtApplicationEndDate = EndDate });
                }
            }
            return View();
        }

        //SubmitLoanApplication
        [HttpGet]
        public ActionResult SubmitLoanApplication(int AId)
        {
            var lapp = dc.LoanApplication.Where(x => x.Id == AId).FirstOrDefault();
            if (ModelState.IsValid)
            {
                try
                {
                    //string Resp = "";
                    lapp.Status = "Submitted";
                    dc.LoanApplication.Add(lapp);
                    dc.Entry(lapp).State = EntityState.Modified;
                    dc.SaveChanges();

                    return RedirectToAction("ListLoanApplications");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View();
        }

        //SubmitLoanApplication
        [HttpGet]
        public ActionResult QuoteLoanApplication(int AId)
        {
            var lapp = dc.LoanApplication.Where(x => x.Id == AId).FirstOrDefault();
            if (ModelState.IsValid)
            {
                try
                {
                    //string Resp = "";
                    lapp.Status = "Quoted";
                    dc.LoanApplication.Add(lapp);
                    dc.Entry(lapp).State = EntityState.Modified;
                    dc.SaveChanges();

                    return RedirectToAction("ListLoanApplications");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View();
        }

        public ActionResult ListSubmittedLoanApplication()
        {
            var loanApp = new List<LoanApplication>();
            try
            {
                loanApp = dc.LoanApplication.Where(x => x.Status == "Submitted").ToList();
                return View(loanApp);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }
    }
}