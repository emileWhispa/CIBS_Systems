using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace eBroker.Controllers
{
    public class ExpiryPoliceController : Controller
    {
        //
        // GET: /ExpiryPolice/

        public ActionResult DataProviderAction()
        {
            eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
            var filteredPolicies = from c in dc.Vw_Policy_Report
                                    .Where(c => (c.expiry_dt > DateTime.Today)
                                            )
                                   select new {
                                    c.policy_no,
                                    c.product_name,
                                    c.insurer,
                                    c.effective_dt,
                                    c.expiry_dt,
                                    c.net_premium
                                   };


            return Json(new { data = filteredPolicies }, JsonRequestBehavior.AllowGet);
        }

    }
}
