using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBroker.Controllers
{
    public class CommissionTariffController : BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListCommissionTariff(string query)
        {
            try
            {
                var Model = new List<Commission_Tariff>();
                try
                {
                    if (string.IsNullOrEmpty(query))
                        Model = dc.Commission_Tariff.OrderByDescending(x => x.Id).ToList();
                    else
                        Model = dc.Commission_Tariff.Where(x => x.Insurers.company_short_name.Contains(query) || x.Insurers.company_name.Contains(query) || x.InsuranceProducts.product_name.Contains(query)).ToList();
                    return View(Model);
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
                return View();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult CommissionTariffInfo(int Id = 0)
        {
            try
            {
                var Model = dc.Commission_Tariff.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new Commission_Tariff();
                    Model.Id = 0;
                    Model.user_id = AppUserData.Login;
                    Model.invoice_dt = DateTime.Now;
                    Model.invoice_due_dt = DateTime.Today.AddDays(5);//5 Days by default
                    Model.invoice_until_dt = DateTime.Today;
                    Model.Status = "Pending";
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Customer Recruiter
                var insurers = (from r in dc.Partner.Where(x => x.partnership_type == "Insurance").ToList() select new SelectListItem { Text = r.company_short_name, Value = r.Id.ToString() }).ToList();
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
        public ActionResult CreateInvoice(eBroker.Invoice inv)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string Resp = "";
                    dc.Invoice.Add(inv);
                    if (inv.Id == 0)
                    {
                        int res = dc.SaveChanges();
                        if (res > 0)
                            Success("Invoice Header Created Successfully", true);
                        else
                            throw new Exception(Resp);
                    }
                    else//Update
                    {
                        dc.Entry(inv).State = EntityState.Modified;
                        dc.SaveChanges();
                    }

                    return RedirectToAction("ListInvoice");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View(inv);
        }


    }
}
