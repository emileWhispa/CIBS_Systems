using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using eBroker.DAL;
using eBroker.Services;

namespace eBroker.Controllers
{
    public class GlBalancesController : BaseController
    {
        private readonly BrokerDataContext _dc;

        public GlBalancesController()
        {
            _dc = new BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"]
                .ConnectionString);
        }

        public ActionResult GlView()
        {
            return View("GlBalances");
        }

        public ActionResult GetGlBalances(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return View("GlBalances");
            try
            {
                var dateTime = DateTime.Parse(date);
                var glsList = GlBalances(dateTime);
                ViewBag.Date = dateTime;
                return View(glsList);
            }
            catch (Exception e)
            {
                e = GetInnerException(e);
                Danger(e.Message);
            }
            return View("GlBalances");
        }

        private List<VwLedgerBalance> GlBalances(DateTime dateTime)
        {
            return _dc.VwLedgerBalance.Where(x =>DbFunctions.TruncateTime(x.BalanceDate) == dateTime).ToList();
        }

        public ActionResult ExportGlBalanceToExcel(DateTime date)
        {
            var gls = GlBalances(date);
            return File(TransactionService.GlBalanceToExcel(gls).ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "gl_balance_" + DateTime.Now + ".xlsx");
        }

    }
}