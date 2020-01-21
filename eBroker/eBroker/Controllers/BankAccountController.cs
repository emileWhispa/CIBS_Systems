using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace eBroker.Controllers
{
    [Authorize]
    public class BankAccountController : BaseController
    {
        private readonly BrokerDataContext _dc = new BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        [HttpGet]
        public ActionResult BankAccountInfo(int id = 0)
        {
            var model = new BankAccount();
            try
            {
                ViewBag.lstTrans = _dc.BankAccount.ToList();
                model.Id = 0;
            }

            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message);
            }

            var banks = (from t in _dc.AcBanks.ToList()
                select new SelectListItem {Text = t.BankName, Value = t.Id.ToString()}).ToList();

            ViewBag.Banks = banks;
            var currencies = (from t in _dc.Currency
                    select new SelectListItem {Text = t.IsoCode, Value = t.Id.ToString()})
                .ToList(); //Join to Exchange rate to exclude those currencies without middle rate
            ViewBag.Currencies = currencies;
            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult BankAccountEdit(int id)
        {
            var model = _dc.BankAccount.FirstOrDefault(x => x.Id == id);
            if (model == null) HttpNotFound("Not found");

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateBankAccount(BankAccount bakacc)
        {
            try
            {
                if (!ModelState.IsValid) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                _dc.BankAccount.Add(bakacc);
                if (bakacc.Id == 0)
                {
                    //Creating the Ledger Number to be associated to the Bank Account
                    const int bankLedgerGroupNo = 102; //To be changed later
                    var gLs = _dc.GL_Account.Where(x =>
                        x.ParentLedgerNo == bankLedgerGroupNo).ToList();
                    int ledgerNo;

                    if (gLs.Count == 0)
                        ledgerNo = int.Parse(bankLedgerGroupNo + "01");
                    else
                        ledgerNo = gLs.Max(x => x.LedgerNo) + 1;


                    //Creating a new GL on the chart of account
                    var gl = new GL_Account
                    {
                        Active = true,
                        GL_Level = 3,
                        ID = 0,
                        LedgerDescription = bakacc.AccountDescription + "-Account No: " + bakacc.AccountNo,
                        LedgerNo = ledgerNo,
                        LedgerType = "D",
                        ParentLedgerNo = bankLedgerGroupNo,
                        Vision_GL = "100030"
                    };


                    //Final Level
                    //New GL
                    //Debit
                    _dc.GL_Account.Add(gl);
                    var res1 = _dc.SaveChanges();
                    if (res1 <= 0) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                    bakacc.LedgerNo = ledgerNo;

                    var res = _dc.SaveChanges();
                    return res > 0
                        ? new HttpStatusCodeResult(HttpStatusCode.OK)
                        : new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }


                // update Gl Account
                var glAccount = _dc.GL_Account.FirstOrDefault(x =>
                    x.LedgerNo == bakacc.LedgerNo);
                if (glAccount == null)
                {
                    Danger("Invalid GL Account");
                    return new HttpStatusCodeResult(HttpStatusCode.NoContent);
                }

                glAccount.LedgerDescription = bakacc.AccountDescription + "-Account No: " + bakacc.AccountNo;
                _dc.Entry(glAccount).State = EntityState.Modified;

                _dc.Entry(bakacc).State = EntityState.Modified;
                _dc.SaveChanges();

                Information("Data successfully updated");
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
        }

        public async Task<ActionResult> GetBankAccountByBank(int bankId, string type)
        {
            IQueryable<BankAccount> accounts;
            if (type.Equals("foreign"))
            {
                accounts = _dc.BankAccount
                    .Where(x =>  x.BankId == bankId &&
                                !x.Currencies.IsoCode.ToLower().Equals("rwf"));
            }
            else
            {
                accounts = _dc.BankAccount
                    .Where(x =>  x.BankId == bankId &&
                                x.Currencies.IsoCode.ToLower().Equals("rwf"));
            }

            return Json(await accounts.ToListAsync(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetBankByCurrency(int currencyId)
        {
            var banks = await (from bankAccount in _dc.BankAccount
                    join bank in _dc.AcBanks on bankAccount.BankId equals bank.Id
                    where ( bankAccount.CurrencyID == currencyId)
                    select new {bankAccount}
                ).ToListAsync();
            return Json(banks, JsonRequestBehavior.AllowGet);
        }
    }
}