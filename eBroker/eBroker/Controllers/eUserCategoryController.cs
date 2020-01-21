using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;

namespace eBroker.Controllers
{
    [Authorize]
    public class EUserCategoryController : BaseController
    {
        eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListUserCategory(string query)
        {
            var eUserCategory = new List<eUserCategory>();
            try
            {
                if (string.IsNullOrEmpty(query))
                    eUserCategory = _dc.eUserCategory.OrderBy(b => b.Id).Take(50).ToList();
                else
                    eUserCategory = _dc.eUserCategory.Where(b => b.Category.Contains(query) || b.Description.Contains(query)).ToList();
                return View(eUserCategory);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult UserCategoryInfo(int id=0)
        {
            eUserCategory model = new eUserCategory();
            try
            {
                if (id==0)
                {
                      model.insert = true;
                      model.Id = 0;
                }
                else
                    model = _dc.eUserCategory.Where(x => x.Id == id).FirstOrDefault();
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult CreateUserCategory(eUserCategory uc)
        {
            if (ModelState.IsValid)
            {
                
                try
                {
                    string resp = "";
                    _dc.eUserCategory.Add(uc);
                    if (uc.insert)
                    {
                        int res = _dc.SaveChanges();
                        if (res > 0)
                            Success("Record Saved Successfully", true);
                        else
                        {
                            resp = "Unable to insert the User Category nfo";
                            throw new Exception(resp);
                        }
                    }
                    else//Update
                    {
                        _dc.Entry(uc).State = EntityState.Modified;
                        _dc.SaveChanges();
                        
                    }

                    return Content("1");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return PartialView("UserCategoryInfo", uc);
        }
    }
}
