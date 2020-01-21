using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ClosedXML.Excel;
using CrystalDecisions.CrystalReports.Engine;
using Forex.ViewModels;

namespace eBroker.Controllers
{
    [Authorize]
    public class CashMovementController : BaseController
    {
        eBroker.BrokerDataContext _dc =
            new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"]
                .ConnectionString);

        public ActionResult ListCashMovement(string tranDate)
        {
            try
            {
                var listTransactions = new List<Vw_CashMovement>();
                var edit = false;
                var currentDate = ""; //Setting the date to today's date

                if (string.IsNullOrEmpty(tranDate))
                {
                    tranDate = currentDate;
                }

                if (tranDate == currentDate)
                    edit = true;

                var tDate = DateTime.Parse(tranDate);
                listTransactions = _dc.Vw_CashMovement.Where(x => x.TransactionDate == tDate).ToList();
                //Getting today's date
                ViewBag.CurrentDate = tranDate;
                ViewBag.AllowEdit = edit;
                return PartialView("_ListCashMovement", listTransactions);
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }

            return PartialView();
        }


        [HttpGet]
        public ActionResult CashMovementInfo(TransactionFilter filter)
        {
            try
            {
                var model = new CashMovement();
                IQueryable<Vw_CashMovement> listCashMovements;
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    listCashMovements = _dc.Vw_CashMovement
                        .Where(x => (x.TransactionDate >= filter.StartDate && x.TransactionDate <= filter.EndDate)
                        )
                        .OrderByDescending(x => x.Id);

                    ViewBag.StartDate = filter.StartDate.GetValueOrDefault().ToString("MM/dd/yyyy");
                    ViewBag.EndDate = filter.EndDate.GetValueOrDefault().ToString("MM/dd/yyyy");

                    return View("ViewCashMovement", listCashMovements.ToList());
                }

                /*    listCashMovements = _dc.Vw_CashMovement
                       .Where(x => x.TransactionDate == tranDate && x.ForexId == AppUserData.CompanyId && x.Inputer == AppUserData.Login && x.BranchId == AppUserData.BranchId)
                       .OrderByDescending(x => x.Id);*/

                var list = _dc.CashMovement
                    .Include(c => c.Currencies)
                    .Include(c => c.Banks)
                    .Where(c => c.UserID == AppUserData.Login
                    ).ToList();
//            ViewBag.lstTrans = listCashMovements;
                Console.Out.Write((list.Count));
                ViewBag.cashMovements = list;
                model.Id = 0;
                model.TransactionDate = DateTime.Now;

                //Dropdownlist definition

                var banks = (from b in _dc.AcBanks
                    join r in _dc.BankAccount on b.Id equals r.BankId
                    select new SelectListItem {Text = b.BankName, Value = b.Id.ToString()}).Distinct().ToList();
                ViewBag.Banks = banks;
                var currencies = (from t in _dc.Currency.ToList()
                    select new SelectListItem {Text = t.IsoCode, Value = t.Id.ToString()}).ToList();
                ViewBag.Currencies = currencies;
                ViewBag.CurrentDate = model.TransactionDate;

                return View(model);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult CashMovementEdit(int id)
        {
            var model = new CashMovement();
            try
            {
                model = _dc.CashMovement.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateCashMovement(CashMovement txn)
        {
            try
            {
                txn.UserID = AppUserData.Login;
                if (txn.Id == 0)
                {
                    txn.Id = 0;
                    txn.CreatedOn = DateTime.Now;
                    txn.Reversal = false;

                    txn.TransactionDate = DateTime.Now;
                }

                //Adding Source and Destination Ledger
                //Determining Bank Ledger
                var baccount = _dc.BankAccount.FirstOrDefault(x => x.AccountNo == txn.AccountNo);
                if (txn.TransactionType.ToLower().Equals("CASH DEPOSIT".ToLower()))
                {
                    // if (_dc.EODSettings != null) txn.SourceLedger = _dc.EODSettings.FirstOrDefault().CashLedger;
                    if (baccount != null) txn.DestinationLedger = baccount.LedgerNo;
                }
                else
                {
                    txn.DestinationLedger = 0;
                    txn.SourceLedger = baccount.LedgerNo;
                }

                if (!ModelState.IsValid)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (txn.Id == 0)
                {
                    _dc.CashMovement.Add(txn);
                    var res = _dc.SaveChanges();
                    if (res > 0)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.OK);
                    }
                }
                else //Update
                {
                    var oldValCashMove = _dc.CashMovement.FirstOrDefault(x => x.Id == txn.Id);
                    //Audit Trail
                    if (oldValCashMove == null) return RedirectToAction("CashMovementInfo");

                    var copyOldValCashMovement = new CashMovement
                    {
                        Id = oldValCashMove.Id,
                        Amount = oldValCashMove.Amount,
                        CreatedOn = oldValCashMove.CreatedOn,
                        CurrencyID = oldValCashMove.CurrencyID,
                        BankID = oldValCashMove.BankID,
                        AccountNo = oldValCashMove.AccountNo,
                        TransactionType = oldValCashMove.TransactionType,
                        TransactionDate = oldValCashMove.TransactionDate,
                        Reversal = oldValCashMove.Reversal,
                        UserID = oldValCashMove.UserID,
                        Reason = oldValCashMove.Reason,
                        CheckNo = oldValCashMove.CheckNo,
                        SourceLedger = oldValCashMove.SourceLedger,
                        DestinationLedger = oldValCashMove.DestinationLedger
                    };
                    //Setting up the new values
                    oldValCashMove.Amount = txn.Amount;
                    oldValCashMove.CurrencyID = txn.CurrencyID;
                    oldValCashMove.BankID = txn.BankID;
                    oldValCashMove.AccountNo = txn.AccountNo;
                    oldValCashMove.TransactionType = txn.TransactionType;
                    oldValCashMove.TransactionDate = DateTime.Now;
                    oldValCashMove.Reversal = txn.Reversal;
                    oldValCashMove.Reason = txn.Reason;
                    oldValCashMove.CheckNo = txn.CheckNo;
                    oldValCashMove.SourceLedger = txn.SourceLedger;
                    oldValCashMove.DestinationLedger = txn.DestinationLedger;
                    _dc.SaveChanges();

                    //CreateAuditTrail("Update", "CashMovement", AppUserData.Login, txn.Id, copyOldValCashMovement,
                    //  txn);
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message, true);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult ReverseCashMovement(int id)
        {
            var movement = _dc.CashMovement.Find(id);
            if (movement == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            movement.Reversal = !movement.Reversal;
            _dc.Entry(movement).State = EntityState.Modified;
            var i = _dc.SaveChanges();
            return RedirectToAction("CashMovementInfo");
        }

        public ActionResult CashMovementReceipt(string cId)
        {
            var path = "";
            var bId = cId;
            var dt = Toolkit.ReturnDatatable("select * from Vw_CashMovement where UID=" + bId);
            var rd = new ReportDocument();
            path = Server.MapPath("~/Reports/rptCashMovement.rpt");
            rd.Load(Server.MapPath("~/Reports/rptCashMovement.rpt"));
            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            var stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", "rptCashMovement.pdf");
        }

        public FileResult ToExcel(TransactionFilter viewModel)
        {
            var dt = new DataTable("Grid");
            dt.Columns.AddRange(new[]
            {
                new DataColumn("Date"),
                new DataColumn("Type"),
                new DataColumn("Bank"),
                new DataColumn("Acc. Number"),
                new DataColumn("Source Ledger"),
                new DataColumn("Dest. Ledger"),
                new DataColumn("Currency"),
                new DataColumn("Amount"),
                new DataColumn("Reversed"),
                new DataColumn("Branch")
            });

            var tranDate = DateTime.Now;

            IQueryable<Vw_CashMovement> transactions;
            if (viewModel.StartDate != null && viewModel.EndDate != null)
            {
                transactions = _dc.Vw_CashMovement
                    .Where(t => t.TransactionDate >= viewModel.StartDate &&
                                t.TransactionDate <= viewModel.EndDate);
            }
            else
            {
                transactions = _dc.Vw_CashMovement.Where(t => t.TransactionDate == tranDate);
            }

            foreach (var transaction in transactions)
            {
                dt.Rows.Add(
                    transaction.TransactionDate.ToString("yyyy MM dd"),
                    transaction.TransactionType,
                    transaction.BankName,
                    transaction.AccountNo,
                    transaction.SourceLedger,
                    transaction.DestinationLedger,
                    transaction.IsoCode,
                    transaction.Amount,
                    transaction.Reversal ? "Yes" : "No");
            }

            using (var wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "cash_movement.xlsx");
                }
            }
        }
    }
}