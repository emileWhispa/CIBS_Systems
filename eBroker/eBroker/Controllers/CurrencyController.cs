using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace eBroker.Controllers
{
    [Authorize]
    public class CurrencyController : BaseController
    {
        
        private readonly BrokerDataContext _dc = new BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
        public ActionResult ListCurrencies(string query)
        {
            try
            {
                var listCurrency = new List<Currency>();

                if (string.IsNullOrEmpty(query))
                {
                    listCurrency = _dc.Currency.OrderBy(x => x.Id).ToList();
                }
                else
                    listCurrency = _dc.Currency.Where(x => x.IsoCode.Contains(query)).OrderBy(x => x.Id).ToList();
                return View(listCurrency);
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }
        
        
        [HttpGet]
        public ActionResult CurrencyInfo(int Id = 0)
        {
            var Model = new Currency();
            try
            {
                if (Id == 0)
                {
                    Model = new Currency
                    {
                        Active = true,
                        Id = 0,
                    };
                }
                else
                {
                    Model = _dc.Currency.FirstOrDefault(x => x.Id == Id);
                }
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            ViewBag.CurrencyListData = _dc.Currency.ToList();
            var CurrenciesList = (from v in _dc.CurrencyList.ToList() select new SelectListItem { Text = v.CurrencyName, Value = v.ISO_Currency_Code.ToString() }).ToList();
            ViewBag.CurrenciesList = CurrenciesList;
            return View(Model);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult CurrencyEdit(int Id)
        {
            var Model = new Currency();
            try
            {
                Model = _dc.Currency.Where(x => x.Id == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return Json(Model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateCurrency(Currency cu)
        {
            if (cu != null && cu.IsoCode != null)
            {
                var cur = _dc.CurrencyList.Where(x => x.ISO_Currency_Code == cu.IsoCode).FirstOrDefault();
                if (cur != null)
                    cu.CurrencyName = cur.CurrencyName;
            }
            if (cu.Id == 0)
            {
                cu.CreatedOn = DateTime.Now;
                cu.Id = 0;
                cu.Active = true;
                cu.CreatedBy = AppUserData.Login;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var Resp = "";
                    if (cu.Id == 0)
                    {
                        _dc.Currency.Add(cu);
                        var res = _dc.SaveChanges();
                        if (res > 0)
                        {
                            //Success("Record Saved Successfully", true);
                            var currency = _dc.Currency.ToList();
                            return PartialView("_ListCurrencies", currency);
                        }
                        else
                        {
                            Resp = "Unable to insert the new Currency";
                            throw new Exception(Resp);
                        }
                    }
                    else//Update
                    {
                        var oldValCurre = new Currency();
                        oldValCurre = _dc.Currency.FirstOrDefault(x => x.Id == cu.Id);
                        if (oldValCurre != null)
                        {
                            var CopyOldValCurrency = new Currency
                            {
                                Id = oldValCurre.Id,
                                IsoCode = oldValCurre.IsoCode,
                                CurrencyName = oldValCurre.CurrencyName,
                                Active = oldValCurre.Active,
                                CreatedOn = oldValCurre.CreatedOn,
                                CreatedBy = oldValCurre.CreatedBy
                            };
                            //Setting up the new values
                            oldValCurre.IsoCode = cu.IsoCode;
                            oldValCurre.CurrencyName = cu.CurrencyName;
                            oldValCurre.Active = cu.Active;
                            _dc.SaveChanges();
                            //CreateAuditTrail("Update", "Currency", AppUserData.Login, cu.Id, CopyOldValCurrency, cu);
                        }
                        var currencies = _dc.Currency.ToList();
                        return PartialView("_ListCurrencies", currencies);
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

                    return View("CurrencyInfo", cu);
                }
            }
            else
            {
                Danger("Input validation errors! Kindly check if all fields are populated properly", true);
                return View("CurrencyInfo", cu);
            }
        }
    }
}