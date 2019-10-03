using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBroker.Controllers
{
    public class SecurityController : BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListMenu(int CategoryId)
        {
            try
            {
                var menus = dc.eCategoryMenu.Where(x => x.CategoryId== CategoryId).ToList();
                var profile = dc.eUserCategory.Where(x => x.Id == CategoryId).FirstOrDefault();
                ViewBag.Profile = profile.Category;
                ViewBag.CategoryId = CategoryId;
                return View(menus);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult ListProfileUsers(int CategoryId)
        {
            try
            {
                var users = dc.eUser.Where(x => x.CategoryId == CategoryId).ToList();
                var profile = dc.eUserCategory.Where(x => x.Id == CategoryId).FirstOrDefault();
                ViewBag.Profile = profile.Category;
                return View(users);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult eCategoryMenuInfo(int CategoryId ,int Id = 0)
        {
            eCategoryMenu Model = new eCategoryMenu();
            try
            {
                if (Id == 0)
                {
                    Model = new eCategoryMenu();
                    Model.Id = 0;
                    Model.CategoryId = CategoryId;
                }
                else
                    Model = dc.eCategoryMenu.Where(x => x.Id == Id).FirstOrDefault();
                var profile = (from r in dc.eUserCategory.ToList() select new SelectListItem { Text = r.Category, Value = r.Id.ToString() }).ToList();
                ViewBag.profile = profile;
                var menu = (from r in dc.Menu.OrderBy(x=>x.MenuName).ToList() select new SelectListItem { Text = r.MenuName, Value = r.Id.ToString() }).ToList();
                ViewBag.Menus = menu;
                //ViewBag.CategoryId = CategoryId;
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View(Model);
        }

        [HttpGet]
        public ActionResult RemoveMenu(int Id, int CategoryId)
        {
            try
            {
                Toolkit.RunSQLCommand("Delete from eCategoryMenus where Id="+Id);
                return RedirectToAction("ListMenu", new { CategoryId = CategoryId });
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpPost]
        public ActionResult CreateUserGroup(eCategoryMenu b)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string Resp = "";
                    dc.eCategoryMenu.Add(b);
                    if (b.Id == 0)
                    {
                        int res = dc.SaveChanges();
                        if (res > 0)
                            Success("Record Saved Successfully", true);
                        else
                        {
                            Resp = "Unable to attach this menu";
                            throw new Exception(Resp);
                        }
                    }
                    else//Update
                    {
                        dc.Entry(b).State = EntityState.Modified;
                        dc.SaveChanges();
                    }

                    return RedirectToAction("ListMenu", new { CategoryId = b.CategoryId });
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            return View(b);
        }

    }
}
