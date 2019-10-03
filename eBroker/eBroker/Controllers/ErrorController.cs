using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBroker.Controllers
{
    public class ErrorController : Controller
    {


        public ActionResult AccessDenied()
        {
            return View();
        }


        [HandleError]
        public ActionResult Error()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            //return HttpNotFound();
            return View();
        }
        [HandleError]
        public ActionResult BadRequest()
        {
            return View();
        }
        [AntiForgeryTokenFilter]
        public ActionResult SessionTerminated()
        {
            return View();
        }
    }

}