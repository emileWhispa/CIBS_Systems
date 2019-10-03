using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace eBroker.Controllers
{
   
    public class SharedController : BaseController
    {
        //private Bl bl;

        public SharedController()
        {
            //bl = new Bl(Util.GetDbConnString());
        }
        
        public ActionResult Menus()
        {
            try
            {
                eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
                //var model = (from mn in dc.Menu
                //             select mn);
                int categoryId = 0;
                int.TryParse(AppUserData.Category, out categoryId);
                var model = (from m in dc.Menu
                             join d in dc.eCategoryMenu on m.Id equals d.MenuID
                             where d.CategoryId == categoryId
                             select m).OrderBy(x => x.ParentId).OrderBy(x => x.OrderNo);
                //if (model==null)
                //    model = new IQueryable<Menu>();
                //Selecting main menu
                var mainMenus = (from m in dc.Menu
                                 join d in dc.eCategoryMenu on m.Id equals d.Menus.ParentId
                                 where d.CategoryId == categoryId && m.MenuLevel == 0
                                 select m).OrderBy(x => x.OrderNo).Distinct();
                ViewBag.MainMenus = mainMenus.OrderBy(x=>x.OrderNo).ToList();
                return PartialView("_Menus", model.ToList());
            }
            catch (Exception)
            {
                //Util.LogError("Shared-Menus", ex);
                return RedirectToAction("Login", "Account", new { returnUrl = "" });
            }
        }

         

        public ActionResult Index()
        {
            return View();
        }
        
    }
}