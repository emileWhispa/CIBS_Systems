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
        private readonly eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListMenu(int categoryId)
        {
            try
            {
                var menus = _dc.eCategoryMenu.Where(x => x.CategoryId== categoryId).ToList();
                var profile = _dc.eUserCategory.FirstOrDefault(x => x.Id == categoryId);
                ViewBag.Profile = profile.Category;
                ViewBag.CategoryId = categoryId;
                ECategoryMenuInfo(categoryId);
                return View(menus);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult ListProfileUsers(int categoryId)
        {
            try
            {
                var users = _dc.eUser.Where(x => x.CategoryId == categoryId).ToList();
                var profile = _dc.eUserCategory.FirstOrDefault(x => x.Id == categoryId);
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
        public ActionResult ECategoryMenuInfo(int categoryId ,int id = 0)
        {
            eCategoryMenu model = new eCategoryMenu();
            try
            {
                if (id == 0)
                {
                    model = new eCategoryMenu();
                    model.Id = 0;
                    model.CategoryId = categoryId;
                }
                else
                    model = _dc.eCategoryMenu.FirstOrDefault(x => x.Id == id);
                var profile = (from r in _dc.eUserCategory.ToList() select new SelectListItem { Text = r.Category, Value = r.Id.ToString() }).ToList();
                ViewBag.profile = profile;
                var menu = (from r in _dc.Menu.OrderBy(x=>x.MenuName).ToList() select new SelectListItem { Text = r.MenuName, Value = r.Id.ToString() }).ToList();
                ViewBag.Menus = menu;
                //ViewBag.CategoryId = CategoryId;
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult RemoveMenu(int id, int categoryId)
        {
            try
            {
                Toolkit.RunSQLCommand("Delete from eCategoryMenus where Id="+id);
                return RedirectToAction("ListMenu", new { CategoryId = categoryId });
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
                    string resp = "";
                    _dc.eCategoryMenu.Add(b);
                    if (b.Id == 0)
                    {
                        int res = _dc.SaveChanges();
                        if (res > 0)
                            Success("Record Saved Successfully", true);
                        else
                        {
                            resp = "Unable to attach this menu";
                            throw new Exception(resp);
                        }
                    }
                    else//Update
                    {
                        _dc.Entry(b).State = EntityState.Modified;
                        _dc.SaveChanges();
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
