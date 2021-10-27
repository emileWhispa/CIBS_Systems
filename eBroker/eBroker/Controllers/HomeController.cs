using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using eBroker;

namespace eBroker.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            eBroker.BrokerDataContext dc=new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
                //var model = dc.Vw_Brokerage_Summary.ToList();
            List<Vw_Policy_Report> sc = new List<Vw_Policy_Report>();
            DateTime today=DateTime.Today;
            if (AppUserData.Category == "3")//Bank User
            {
                var bankName=dc.Bank.Where(x=>x.Id==AppUserData.CompanyID).FirstOrDefault();
                sc = dc.Vw_Policy_Report.Where(x => x.expiry_dt == today && x.BankName== bankName.BankName).OrderByDescending(x => x.Id).ToList();
                ViewBag.BankPolicies = dc.InsurancePolicy.Where(x => x.interest_bank_id == AppUserData.CompanyID).Count();
                ViewBag.BankNetPremium = dc.InsurancePolicy.Where(x => x.interest_bank_id == AppUserData.CompanyID).Sum(x=>x.net_premium);
                ViewBag.BankCustomers = dc.Client.Where(x => x.recruited_by == bankName.BankName).Distinct().Count();
                ViewBag.FireInsurance = dc.InsurancePolicy.Where(x => x.interest_bank_id == AppUserData.CompanyID && x.product_id ==3).Count();
                ViewBag.MotorInsurance = dc.InsurancePolicy.Where(x => x.interest_bank_id == AppUserData.CompanyID && (x.product_id ==1 || x.product_id ==2)).Count();
                ViewBag.PersonalAccidentInsurance = dc.InsurancePolicy.Where(x => x.interest_bank_id == AppUserData.CompanyID && x.product_id == 4).Count();
                ViewBag.TravelInsurance = dc.InsurancePolicy.Where(x => x.interest_bank_id == AppUserData.CompanyID && x.product_id == 5).Count();
                ViewBag.OtherInsurance = dc.InsurancePolicy.Where(x => x.interest_bank_id == AppUserData.CompanyID && x.product_id != 1 && x.product_id != 2 && x.product_id != 3 && x.product_id != 4 && x.product_id != 5).Count();
            }
            else
            {
                sc = dc.Vw_Policy_Report.Where(x => x.expiry_dt == today).OrderByDescending(x => x.Id).ToList();
            }
            ViewData["Vw_Policies"] = sc;

                var model = from e in dc.Vw_Brokerage_Summary
                                orderby e.Id
                                select e;
                ViewBag.UserCategoryId = AppUserData.Category;
            return View(model.FirstOrDefault());
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
