using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using eBroker.DAL;

namespace eBroker.Controllers
{
    public class BanksController : BaseController
    {
        private readonly eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);


        [HttpGet]
        public ActionResult BankInfo(int id = 0)
        {
            var model = new AcBank();
            try
            {

                ViewBag.lstTrans = _dc.AcBanks.ToList();
                model.Id = 0;
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult BankEdit(int id)
        {
            var model = new Bank();
            try
            {
                model = _dc.Bank.Where(x => x.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CreateBank(AcBank bak)
        {
            if (bak.Id == 0)
            {
                bak.Id = 0;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var resp = "";
                    _dc.AcBanks.Add(bak);
                    if (bak.Id == 0)
                    {
                        var res = _dc.SaveChanges();
                        if (res > 0)
                        {
                            var banks = _dc.AcBanks.OrderByDescending(x => x.Id).Take(10).ToList();
                            return PartialView("_ListBanks", banks);
                        }
                        else
                        {
                            resp = "Unable to insert the new Bank";
                            throw new Exception(resp);
                        }
                    }
                    else
                    {
                        _dc.Entry(bak).State = EntityState.Modified;
                        _dc.SaveChanges();
                        var banks = _dc.AcBanks.OrderByDescending(x => x.Id).Take(10).ToList();
                        return PartialView("_ListBanks", banks);
                    }
                }
                catch (Exception ex)
                {
                    var msg = "";
                    if (ex.InnerException != null)
                        if (ex.InnerException.InnerException != null)
                        {
                            msg = ex.InnerException.InnerException.Message;
                        }
                        else
                            msg = ex.InnerException.Message;
                    else
                        msg = ex.Message;
                    Danger(msg);
                    return new HttpStatusCodeResult(200);
                    //return View("BankInfo", bak);
                }
            }
            else
            {
                Danger("Input validation errors! Kindly check if all fields are populated properly", true);
                return View("BankInfo", bak);
            }
        }
    }
}