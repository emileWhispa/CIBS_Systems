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
        eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListLoanApplications()
        {
            var banks = (from r in _dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            if (AppUserData.Category == "5")//Bank Staff
            {
                banks = (from r in _dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            }
            ViewBag.Banks = banks;
            return View();
        }

        public ActionResult ListLoanApplicationView(string txtApplicationStartDate, string txtApplicationEndDate, string bankName)
        {
            var loanApp = new List<LoanApplication>();
            try
            {
                if (string.IsNullOrEmpty(bankName))
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
                DateTime startDate = DateTime.Parse(txtApplicationStartDate);
                DateTime endDate = DateTime.Parse(txtApplicationEndDate);
                loanApp = _dc.LoanApplication.Where(x => x.ApplicationDate >= startDate && x.ApplicationDate <= endDate && x.Banks.BankName == bankName).ToList();
                ViewBag.BankName = bankName;
                ViewBag.StartDate = startDate.ToShortDateString();
                ViewBag.EndDate = endDate.ToShortDateString();
                return View(loanApp);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult LoanApplicationInfo(string bankName, string startDate, string endDate, int id = 0)
        {
            try
            {
                var model = _dc.LoanApplication.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new LoanApplication();
                }
                ViewBag.BankName = bankName;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                return View(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }


        [HttpGet]
        public ActionResult LoanApplicationPropertyInfo(int aId, string bankName, string startDate, string endDate, int id = 0)
        {
            try
            {
                var model = _dc.LoanApplication_Property.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new LoanApplication_Property();
                    model.loan_application_id = aId;
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
                ViewBag.BankName = bankName;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                return View(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult LoanApplicationPropertyInfoView(int aId, int id = 0)
        {
            try
            {
                var model = _dc.LoanApplication_Property.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new LoanApplication_Property();
                    model.loan_application_id = aId;
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
                var insurers = (from a in _dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                ViewBag.Insurers = insurers;

                return View(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }


        [HttpGet]

        public ActionResult LoanApplicationVehicleInfo(int aId, string bankName, string startDate, string endDate, int id = 0)
        {
            try
            {
                var model = _dc.LoanApplication_Vehicle.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new LoanApplication_Vehicle();
                    model.loan_application_id = aId;
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
                ViewBag.BankName = bankName;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                return View(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult LoanApplicationVehicleInfoView(int aId, int id = 0)
        {
            try
            {
                var model = _dc.LoanApplication_Vehicle.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new LoanApplication_Vehicle();
                    model.loan_application_id = aId;
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
                var insurers = (from a in _dc.Partner.Where(x => x.partnership_type == "Insurance").OrderBy(x => x.company_short_name).ToList() select new SelectListItem { Text = a.company_name, Value = a.Id.ToString() }).ToList();
                ViewBag.Insurers = insurers;

                return View(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateLoanApplication(eBroker.LoanApplication lp, string bankName, string startDate, string endDate)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (lp.CollateralType == null)
                    {
                        Danger("Collateral Type cannot be empty");
                        ViewBag.BankName = bankName;
                        ViewBag.StartDate = startDate;
                        ViewBag.EndDate = endDate;

                        return View("LoanApplicationInfo", lp);
                    }

                    _dc.LoanApplication.Add(lp);

                    _dc.Entry(lp).State = EntityState.Modified;
                    _dc.SaveChanges();
                    if (lp.CollateralType == "None")
                        return RedirectToAction("ListLoanApplicationView", new { BankName = bankName, txtApplicationStartDate = startDate, txtApplicationEndDate = endDate });
                    else if (lp.CollateralType == "Vehicle")
                        return RedirectToAction("ListLoanApplicationVehicles", new { loanAppId = lp.Id, BankName = bankName, StartDate = startDate, EndDate = endDate });
                    else if (lp.CollateralType == "Property")
                        return RedirectToAction("ListLoanApplicationProperties", new { loanAppId = lp.Id, BankName = bankName, StartDate = startDate, EndDate = endDate });
                    else
                        return RedirectToAction("ListLoanApplicationView", new { BankName = bankName, txtApplicationStartDate = startDate, txtApplicationEndDate = endDate });

                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View("LoanApplicationInfo", lp);
        }

        [HttpPost]
        public ActionResult CreateLoanApplicationProperty(eBroker.LoanApplication_Property ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string resp = "";
                    _dc.LoanApplication_Property.Add(ip);
                    if (ip.Id == 0)
                    {
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

                    return RedirectToAction("ListLoanApplicationProperties", new { loanAppId = ip.loan_application_id });
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View("LoanApplicationPropertyInfo",ip);
        }

        [HttpPost]
        public ActionResult SetPremiumLoanApplicationProperty(eBroker.LoanApplication_Property ip)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Getting the record being updated
                    var prop = _dc.LoanApplication_Property.Where(x => x.Id == ip.Id).FirstOrDefault();
                    prop.PremiumDate = DateTime.Now;
                    prop.PremiumAmount = ip.PremiumAmount;
                    prop.PremiumUser = AppUserData.Login;
                    prop.Insurer_Id = ip.Insurer_Id;
                    _dc.LoanApplication_Property.Add(prop);
                    _dc.Entry(prop).State = EntityState.Modified;
                    _dc.SaveChanges();

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
                    string resp = "";
                    _dc.LoanApplication_Vehicle.Add(ip);
                    if (ip.Id == 0)
                    {
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
                    var veh = _dc.LoanApplication_Vehicle.FirstOrDefault(x => x.Id == ip.Id);
                    veh.PremiumDate = DateTime.Now;
                    veh.PremiumAmount = ip.PremiumAmount;
                    veh.PremiumUser = AppUserData.Login;
                    veh.Insurer_Id = ip.Insurer_Id;
                    _dc.LoanApplication_Vehicle.Add(veh);
                    _dc.Entry(veh).State = EntityState.Modified;
                    _dc.Entry(veh).State = EntityState.Modified;
                    _dc.SaveChanges();
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
        public ActionResult ListLoanApplicationVehicles(int loanAppId, string bankName, string startDate, string endDate)
        {
            var vehicles = _dc.LoanApplication_Vehicle.Where(x => x.loan_application_id == loanAppId).ToList();
            ViewBag.LoanApplicationId = loanAppId;
            ViewBag.BankName = bankName;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(vehicles);
        }

        public ActionResult ListLoanApplicationVehiclesView(int loanAppId)
        {
            var vehicles = _dc.LoanApplication_Vehicle.Where(x => x.loan_application_id == loanAppId).ToList();
            ViewBag.LoanApplicationId = loanAppId;
            return View(vehicles);
        }


        [HttpGet]
        public ActionResult ListLoanApplicationProperties(int loanAppId, string bankName, string startDate, string endDate)
        {
            var properties = _dc.LoanApplication_Property.Where(x => x.loan_application_id == loanAppId).ToList();
            ViewBag.LoanApplicationId = loanAppId;
            ViewBag.BankName = bankName;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(properties);
        }

        public ActionResult ListLoanApplicationPropertiesView(int loanAppId)
        {
            var properties = _dc.LoanApplication_Property.Where(x => x.loan_application_id == loanAppId).ToList();
            ViewBag.LoanApplicationId = loanAppId;
            return View(properties);
        }


        [HttpGet]
        public ActionResult LoanApplicationCollateral(int loanAppId, string bankName, string startDate, string endDate)
        {
            var loanApplication = _dc.LoanApplication.Where(x => x.Id == loanAppId).FirstOrDefault();
            if (bankName != null)
            {
                if (loanApplication.CollateralType == "Vehicle")
                    return RedirectToAction("ListLoanApplicationVehicles", new { loanAppId = loanAppId, BankName = bankName, StartDate = startDate, EndDate = endDate });
                else if (loanApplication.CollateralType == "Property")
                    return RedirectToAction("ListLoanApplicationProperties", new { loanAppId = loanAppId, BankName = bankName, StartDate = startDate, EndDate = endDate });
                else if (loanApplication.CollateralType == "None")
                {
                    Danger("No Collateral Item is supposed to be attached. Collateral Type=[None]");
                    return RedirectToAction("ListLoanApplicationView", new { BankName = bankName, txtApplicationStartDate = startDate, txtApplicationEndDate = endDate });
                }
                else if (loanApplication.CollateralType == null)
                {
                    Danger("No Collateral Type defined yet. Kindly edit the record and select the Collateral Type first");
                    return RedirectToAction("ListLoanApplicationView", new { BankName = bankName, txtApplicationStartDate = startDate, txtApplicationEndDate = endDate });
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
                    return RedirectToAction("ListLoanApplicationView", new { BankName = bankName, txtApplicationStartDate = startDate, txtApplicationEndDate = endDate });
                }
                else if (loanApplication.CollateralType == null)
                {
                    Danger("No Collateral Type defined yet. Kindly edit the record and select the Collateral Type first");
                    return RedirectToAction("ListLoanApplicationView", new { BankName = bankName, txtApplicationStartDate = startDate, txtApplicationEndDate = endDate });
                }
            }
            return View();
        }

        //SubmitLoanApplication
        [HttpGet]
        public ActionResult SubmitLoanApplication(int aId)
        {
            var lapp = _dc.LoanApplication.Where(x => x.Id == aId).FirstOrDefault();
            if (ModelState.IsValid)
            {
                try
                {
                    //string Resp = "";
                    lapp.Status = "Submitted";
                    _dc.LoanApplication.Add(lapp);
                    _dc.Entry(lapp).State = EntityState.Modified;
                    _dc.SaveChanges();

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
        public ActionResult QuoteLoanApplication(int aId)
        {
            var lapp = _dc.LoanApplication.Where(x => x.Id == aId).FirstOrDefault();
            if (ModelState.IsValid)
            {
                try
                {
                    //string Resp = "";
                    lapp.Status = "Quoted";
                    _dc.LoanApplication.Add(lapp);
                    _dc.Entry(lapp).State = EntityState.Modified;
                    _dc.SaveChanges();

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
                loanApp = _dc.LoanApplication.Where(x => x.Status == "Submitted").ToList();
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