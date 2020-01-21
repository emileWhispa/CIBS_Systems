using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace eBroker.Controllers
{
    public class JsonDataController : BaseController
    {
        private readonly eBroker.BrokerDataContext _dc =
            new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"]
                .ConnectionString);

        public ActionResult GetClosingBalance(int currencyId, string transactionType)
        {
            var dt=new DataTable();

            switch (transactionType.ToUpper())
            {
                case "PURCHASE":
                    //Getting Local Currency ID
                    var localCurrencyId = _dc.Currency.FirstOrDefault(x => x.IsoCode == "RWF");
                    if (localCurrencyId != null)
                    {
                        dt = Toolkit.ReturnDatatable("exec sp_teller_report_indiv '" + AppUserData.Login + "'," +
                                                     localCurrencyId.Id );
                    }
                    break;
                case "SALE":
                    dt = Toolkit.ReturnDatatable("exec sp_teller_report_indiv '" + AppUserData.Login + "'," +
                                                 currencyId );
                    break;
                default:
                    if (transactionType.StartsWith("101")) //Cash
                    {
                        dt = Toolkit.ReturnDatatable("exec sp_teller_report_indiv '" + AppUserData.Login + "'," +
                                                     currencyId );
                    }
                    else
                    {
                        dt = Toolkit.ReturnDatatable("exec sp_teller_report_indiv '" + AppUserData.Login + "'," +
                                                     currencyId);
                    }
                    break;
            }

            decimal balance;
            if (dt == null || dt.Rows.Count == 0)
                balance = 0;
            else
            {
                decimal.TryParse(dt.Rows[0].ItemArray.GetValue(0).ToString(), out balance);
            }
            return Json(balance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAccountCurrency(string AccountNo)
        {
            //var accounts = dc.BankAccount
            //    .FirstOrDefault(x => x.AccountNo == AccountNo && x.ForexId == AppUserData.CompanyId);
            //return Json(accounts.Currencies.IsoCode, JsonRequestBehavior.AllowGet);

            var accounts = (from r in _dc.BankAccount.Where(x => x.AccountNo == AccountNo).ToList()
                select new SelectListItem { Text = r.Currencies.IsoCode, Value = r.CurrencyID.ToString() }).ToList();
            return Json(new SelectList(accounts, "Value", "Text"), JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetAssignGLAccounts(int bankId)
        {
            var accounts =
                (from r in _dc.BankAccount.Where(x => x.BankId == bankId).ToList()
                    select new SelectListItem {Text = r.AccountNo, Value = r.AccountNo.ToString()}).ToList();
            return Json(new SelectList(accounts, "Value", "Text"), JsonRequestBehavior.AllowGet);

        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetParentGLs(int glLevel)
        {
            var gLs = (from r in _dc.GL_Account.Where(x => x.GL_Level == (glLevel - 1)).OrderBy(x => x.ParentLedgerNo).ThenBy(x => x.LedgerDescription).ToList() select new SelectListItem { Text = r.LedgerDescription, Value = r.LedgerNo.ToString() }).ToList();
            return Json(new SelectList(gLs, "Value", "Text"), JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetLedgerCurrency(int ledgerNo)
        {
            var currencies  = (from r in _dc.Currency.ToList()
                select new SelectListItem
                {
                    Text = r.IsoCode, Value = r.Id.ToString()
                }).ToList();

            if (ledgerNo!=10101)//Cash ledger
            {
                currencies= (from r in _dc.BankAccount
                    join r1 in _dc.Currency on r.CurrencyID equals r1.Id
                    where (r.LedgerNo == ledgerNo )
                    select new SelectListItem { Text = r1.IsoCode, Value = r1.Id.ToString() }).ToList();
            }
            return Json(new SelectList(currencies, "Value", "Text"), JsonRequestBehavior.AllowGet);

        }
    }
}
