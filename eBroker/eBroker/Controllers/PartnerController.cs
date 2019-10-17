using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;

namespace eBroker.Controllers
{

    public class PartnerController : BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListPartner(string query)
        {
            var partner = new List<Partner>();
            try
            {
            if (string.IsNullOrEmpty(query))
                partner = dc.Partner.OrderByDescending(x => x.Id).ToList();
            else
                partner = dc.Partner.Where(x => x.company_name.Contains(query) || x.company_short_name.Contains(query) || x.mobile1.Contains(query) || x.mobile2.Contains(query) || x.contact_person.Contains(query)).ToList();

            return View(partner);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();

        }

        public ActionResult DeletePartner(int id)
        {
            Partner partner = dc.Partner.Find(id);
            if (partner != null) {
                dc.Partner.Remove(partner);
                dc.SaveChanges();
            }
            return RedirectToAction("ListPartner");
        }

        [HttpPost]
        public ActionResult CreatePartner(eBroker.Partner p)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string Resp = "";
                    dc.Partner.Add(p);
                    if (p.Id == 0)
                    {
                        int res = dc.SaveChanges();
                        if (res > 0)
                            Success("Record Saved Successfully", true);
                        else
                            throw new Exception(Resp);
                    }
                    else//Update
                    {
                        dc.Entry(p).State = EntityState.Modified;
                        dc.SaveChanges();
                    }

                    return Content("1");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return PartialView("PartnerInfo", p);
        }

        [HttpGet]
        public ActionResult PartnerInfo(int Id = 0)
        {
            try
            {
                var Model = dc.Partner.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new Partner();
                    Model.Id = 0;
                    Model.create_dt = DateTime.Now;
                    Model.user_id = AppUserData.Login;
                    //Model.recruited_by = AppUserData.Login;
                }
                return PartialView(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        //
        public ActionResult ExportToExcel()
        {
            try
            {
                Toolkit.ExportListUsingEPPlus("select * from Partner order by company_name", "Partner Listing");
                return View();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

    }
}
