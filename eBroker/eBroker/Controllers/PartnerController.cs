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
        eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListPartner(string query)
        {
            var partner = new List<Partner>();
            try
            {
            if (string.IsNullOrEmpty(query))
                partner = _dc.Partner.OrderByDescending(x => x.Id).ToList();
            else
                partner = _dc.Partner.Where(x => x.company_name.Contains(query) || x.company_short_name.Contains(query) || x.mobile1.Contains(query) || x.mobile2.Contains(query) || x.contact_person.Contains(query)).ToList();

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
            Partner partner = _dc.Partner.Find(id);
            if (partner != null) {
                _dc.Partner.Remove(partner);
                _dc.SaveChanges();
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
                    string resp = "";
                    _dc.Partner.Add(p);
                    if (p.Id == 0)
                    {
                        int res = _dc.SaveChanges();
                        if (res > 0)
                            Success("Record Saved Successfully", true);
                        else
                            throw new Exception(resp);
                    }
                    else//Update
                    {
                        _dc.Entry(p).State = EntityState.Modified;
                        _dc.SaveChanges();
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
        public ActionResult PartnerInfo(int id = 0)
        {
            try
            {
                var model = _dc.Partner.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new Partner();
                    model.Id = 0;
                    model.create_dt = DateTime.Now;
                    model.user_id = AppUserData.Login;
                    //Model.recruited_by = AppUserData.Login;
                }
                return PartialView(model);
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
                return Content("1");
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return Content("1");
        }

    }
}
