using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ClosedXML.Excel;
using eBroker.DAL;

namespace eBroker.Controllers
{
    public class ReconciliationController : BaseController
    {
        private readonly BrokerDataContext _dc = new BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        // GET: Reconciliation
        public ActionResult Index()
        {
            var a = (from b in _dc.AcBanks
                join ba in _dc.BankAccount on b.Id equals ba.BankId
                select new
                {
                    b.Id,
                    b.BankName
                }).Distinct().ToList();

            ViewBag.BankId = new SelectList(a, "Id", "BankName");
            return View();
        }

        public JsonResult AccountNumberByBank(int bankId)
        {
            var bankAccounts = _dc.BankAccount
                //.Select(x => x.AccountNo)
                .ToList();
            return Json(bankAccounts, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLastReconciliationDate(int ledgerNo)
        {
            var reconc =
                _dc.BankReconciliation
                    .Where(x => x.LedgerNo == ledgerNo)
                    .OrderByDescending(x => x.ReconciliationDate).FirstOrDefault();
            if (reconc == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            return Json(reconc, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetGlHistory(Reconciliation reconciliation)
        {
            if (!ModelState.IsValid) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            try
            {
                int.TryParse(reconciliation.AccountNumber, out var ledgerNo);

                if (ledgerNo == 0) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                IQueryable<GL_History> glHistories;
                if (reconciliation.LastReconDate == null)
                {
                    glHistories = _dc.GlHistories
                        .Where(x => x.LedgerNo == ledgerNo &&
                                    DbFunctions.TruncateTime(x.EffectiveDate) <= reconciliation.BankStatementDate);
                }
                else
                {
                    glHistories = _dc.GlHistories
                        .Where(x => x.LedgerNo == ledgerNo &&
                                    x.EffectiveDate > reconciliation.LastReconDate &&
                                    x.EffectiveDate <= reconciliation.BankStatementDate);
                }

                var gls = glHistories
                    .Include(x => x.Currency)
                    .OrderBy(x => x.EffectiveDate)
                    .ToList();

                ViewBag.Reconciliation = reconciliation;
                var bankAccount = _dc.BankAccount
                    .Include(x => x.Banks)
                    .Include(x => x.Currencies)
                    .FirstOrDefault(x => x.LedgerNo == ledgerNo && x.BankId == reconciliation.BankId);
                ViewBag.BankAccount = bankAccount;

                ViewBag.GlBalance = _dc.GlBalances
                    .Include(x => x.Currency)
                    .FirstOrDefault(x =>
                        DbFunctions.TruncateTime(x.BalanceDate) == reconciliation.BankStatementDate
                        && x.LedgerNo == ledgerNo
                        && x.CurrencyId == bankAccount.CurrencyID);


                return PartialView("_GlHistories", gls);
            }
            catch (Exception e)
            {
                e = GetInnerException(e);
                return PartialView("_NoDataFound");
            }
        }


        public ActionResult Reconcile(Reconciliation reconciliation)
        {
            if (!ModelState.IsValid) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var ledgerNo = Convert.ToInt32(reconciliation.AccountNumber);

            var bankRecon =
                _dc.BankReconciliation.FirstOrDefault(x => x.LedgerNo == ledgerNo);

            var d = new DateTime(2019, 7, 31);
            if (reconciliation.LastReconDate != null)
                d = reconciliation.LastReconDate.GetValueOrDefault();
            var lastDate = d;
            if (bankRecon == null)
            {
                var rec = new BankReconciliation
                {
                    LedgerNo = ledgerNo,
                    ReconciliationDate = reconciliation.BankStatementDate,
                    LastReconciliationDate = lastDate,
                    BeginningBalance = (double) reconciliation.StatementAmount,
                    EndingBalance = (double) reconciliation.StatementAmount,
                    UserID = AppUserData.Login
                };

                _dc.BankReconciliation.Add(rec);
                _dc.SaveChanges();
            }
            else
            {
                bankRecon.LedgerNo = ledgerNo;
                bankRecon.ReconciliationDate = reconciliation.BankStatementDate;
                bankRecon.LastReconciliationDate = lastDate;
                bankRecon.BeginningBalance = (double) reconciliation.StatementAmount;
                bankRecon.EndingBalance = (double) reconciliation.StatementAmount;
                bankRecon.UserID = AppUserData.Login;

                _dc.BankReconciliation.Add(bankRecon);
                _dc.Entry(bankRecon).State = EntityState.Modified;
                _dc.SaveChanges();
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public FileResult ToExcel(int ledgerNo)
        {
            var gl = _dc.GlHistories
                .Where(x => x.LedgerNo == ledgerNo)
                .Include(x => x.Currency).ToList();
            var dt = new DataTable("Grid");
            dt.Columns.AddRange(new[]
            {
                new DataColumn("Ledger No"),
                new DataColumn("Date"),
                new DataColumn("Currency"),
                new DataColumn("Amount"),
                new DataColumn("Operation"),
                new DataColumn("Narration")
            });


            foreach (var transaction in gl)
            {
                dt.Rows.Add(
                    transaction.LedgerNo,
                    transaction.EffectiveDate.ToString("yyyy-MM-dd"),
                    transaction.Currency.IsoCode,
                    transaction.Amount.ToString("N1"),
                    transaction.Sign,
                    transaction.Description);
            }
            using (var wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "gl_history_" + DateTime.Now + ".xlsx");
                }
            }
        }
    }
}