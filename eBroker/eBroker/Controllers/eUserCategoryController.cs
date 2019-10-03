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
    public class eUserCategoryController : BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListUserCategory(string query)
        {
            var eUserCategory = new List<eUserCategory>();
            try
            {
                if (string.IsNullOrEmpty(query))
                    eUserCategory = dc.eUserCategory.OrderBy(b => b.Id).Take(50).ToList();
                else
                    eUserCategory = dc.eUserCategory.Where(b => b.Category.Contains(query) || b.Description.Contains(query)).ToList();
                return View(eUserCategory);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult UserCategoryInfo(int Id=0)
        {
            eUserCategory Model = new eUserCategory();
            try
            {
                if (Id==0)
                {
                      Model.insert = true;
                      Model.Id = 0;
                }
                else
                    Model = dc.eUserCategory.Where(x => x.Id == Id).FirstOrDefault();
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView(Model);
        }

        [HttpPost]
        public ActionResult CreateUserCategory(eUserCategory uc)
        {
            if (ModelState.IsValid)
            {
                
                try
                {
                    string Resp = "";
                    dc.eUserCategory.Add(uc);
                    if (uc.insert)
                    {
                        int res = dc.SaveChanges();
                        if (res > 0)
                            Success("Record Saved Successfully", true);
                        else
                        {
                            Resp = "Unable to insert the User Category nfo";
                            throw new Exception(Resp);
                        }
                    }
                    else//Update
                    {
                        dc.Entry(uc).State = EntityState.Modified;
                        dc.SaveChanges();
                        
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
