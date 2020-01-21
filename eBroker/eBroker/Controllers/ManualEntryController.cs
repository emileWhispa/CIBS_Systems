using System;
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
    public class ManualEntryController : BaseController
    {
        //
        // GET: /ManualEntry/
        private readonly eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        [HttpGet]
        public ActionResult ListManualEntries(string query)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView("_ListManualEntries");
        }

        [HttpGet]
        public ActionResult ManualEntryInfo(TransactionFilter filter)
        {
            try
            {
                IQueryable<Vw_ManualEntry> manualEntry;
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    manualEntry = _dc.Vw_ManualEntry
                        .Where(x => (x.EntryDate >= filter.StartDate && x.EntryDate <= filter.EndDate)
                                     )
                        .OrderByDescending(x => x.Id);

                    ViewBag.StartDate = filter.StartDate.GetValueOrDefault().ToString("MM/dd/yyyy");
                    ViewBag.EndDate = filter.EndDate.GetValueOrDefault().ToString("MM/dd/yyyy");

                    return View("ViewManulEntries", manualEntry.ToList());
                }

                manualEntry = _dc.Vw_ManualEntry.OrderByDescending(x => x.EntryDate)
                    .Where(z => z.UserID == AppUserData.Login).Take(25);
                ViewBag.lstTrans = manualEntry.ToList();

                var model = new ManualEntry
                {
                    CreatedOn = DateTime.Now,
                    EntryDate = DateTime.Now,
                    UserID = AppUserData.Login,
                    Id = 0
                };

                var ledgerGroups = (from e in _dc.GL_Account.Where(x => x.GL_Level == 2).ToList()
                    select new SelectListItem {Text = e.LedgerDescription, Value = e.LedgerNo.ToString()}).ToList();
                ViewBag.LedgerGroup = ledgerGroups;

                //var debitLedgers = (from e in dc.ManualEntry join c in dc.GL_Account on e.DebitLedgerNo equals c.LedgerNo where e.Id == c.LedgerNo select new SelectListItem { Text = c.LedgerDescription, Value = e.DebitLedgerNo.ToString() });
                //ViewBag.DebitLedger = debitLedgers;
                //var creditLedgers = (from e in dc.ManualEntry join c in dc.GL_Account on e.CreditLedgerNo equals c.LedgerNo where e.Id == c.LedgerNo select new SelectListItem { Text = c.LedgerDescription, Value = e.CreditLedgerNo.ToString() });
                //ViewBag.CreditLedger = creditLedgers;

                var currencies = (from e in _dc.Currency.ToList()
                    select new SelectListItem {Text = e.IsoCode, Value = e.Id.ToString()}).ToList();
                ViewBag.Currency = currencies;
                return View("ManualEntryInfo", model);
            }

            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message, true);
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult ManualEntryEdit(int id)
        {
            var model = new ManualEntry();
            try
            {
                model = _dc.ManualEntry.FirstOrDefault(x => x.Id == id);

                model.DebitLedgerGroup = int.Parse(model.DebitLedgerNo.ToString().Substring(0, 3));

                model.CreditLedgerGroup = int.Parse(model.CreditLedgerNo.ToString().Substring(0, 3));
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateManualEntry(ManualEntry e)
        {
            e.UserID = AppUserData.Login;
            if (e.Id == 0)
            {
                e.CreatedOn = DateTime.Now;
                e.EntryDate = e.CreatedOn;
            }
            if (!ModelState.IsValid)
            {
                Warning("Please make sure that data are entered correctly");
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            try
            {
                if (e.Id == 0)
                {
                    _dc.ManualEntry.Add(e);
                    var res = _dc.SaveChanges();
                    if (res <= 0) throw new Exception("Unable to insert the new Customer");

                    Success("Record Saved Successfully");
                    return  new HttpStatusCodeResult(HttpStatusCode.OK);
                }

                var oldValManuEntry = _dc.ManualEntry.FirstOrDefault(x => x.Id == e.Id);
                //Audit Trail
                if (oldValManuEntry != null)
                {
                    var copyOldValManualEntry = new ManualEntry
                    {
                        Id = oldValManuEntry.Id,
                        Amount = oldValManuEntry.Amount,
                        CreatedOn = oldValManuEntry.CreatedOn,
                        CurrencyID = oldValManuEntry.CurrencyID,
                        Description = oldValManuEntry.Description,
                        DebitLedgerNo = oldValManuEntry.DebitLedgerNo,
                        CreditLedgerNo = oldValManuEntry.CreditLedgerNo,
                        Reversal = oldValManuEntry.Reversal,
                        UserID = oldValManuEntry.UserID
                    };
                    //Setting up the new values
                    oldValManuEntry.Amount = e.Amount;
                    oldValManuEntry.CurrencyID = e.CurrencyID;
                    oldValManuEntry.Description = e.Description;
                    oldValManuEntry.DebitLedgerNo = e.DebitLedgerNo;
                    oldValManuEntry.CreditLedgerNo = e.CreditLedgerNo;
                    oldValManuEntry.Reversal = e.Reversal;
                    //dc.Expense.Add(e);
                    //dc.Entry(e).State = EntityState.Modified;
                    _dc.SaveChanges();

                    //CreateAuditTrail("Update", "ManualEntry", AppUserData.Login, e.Id, copyOldValManualEntry,
                      //  e);
                }
               
            }
            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult ReverseManualEntry(int id)
        {
            var entry = _dc.ManualEntry.Find(id);

            if (entry != null)
            {
                entry.Reversal = !entry.Reversal;
                _dc.Entry(entry).State = EntityState.Modified;
            }
            _dc.SaveChanges();
            return RedirectToAction("ManualEntryInfo");
        }

        public ActionResult ManualEntryReceipt(string mId)
        {
            var path = "";
            try
            {
                var bId = mId;
                var dt = Toolkit.ReturnDatatable("select * from Vw_Manual_Entries where UID=" + bId);
                var rd = new ReportDocument();
                path = Server.MapPath("~/Reports/rptManualVoucher.rpt");
                rd.Load(Server.MapPath("~/Reports/rptManualVoucher.rpt"));
                rd.SetDataSource(dt);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "rptManualVoucher.pdf");
            }
            catch (Exception ex)
            {
                Danger(ex.Message + " Path: " + path);
            }
            return View("Error");
        }


        public FileResult ToExcel(TransactionFilter viewModel)
        {
            var dt = new DataTable("Grid");
            dt.Columns.AddRange(new[]
            {
                new DataColumn("Date"),
                new DataColumn("Debit Ledger"),
                new DataColumn("Credit Ledger"),
                new DataColumn("Currency"),
                new DataColumn("Amount"),
                new DataColumn("Description"),
                new DataColumn("Reversed"),
                new DataColumn("Branch")
            });

            var tranDate = DateTime.Now;
            //            ViewBag.TransactionDate = tranDate;

            IQueryable<Vw_ManualEntry> transactions;
            if (viewModel.StartDate != null && viewModel.EndDate != null)
            {
                transactions = _dc.Vw_ManualEntry
                    .Where(t => t.EntryDate >= viewModel.StartDate &&
                                t.EntryDate <= viewModel.EndDate);
            }
            else
            {
                transactions =
                    _dc.Vw_ManualEntry.Where(t => t.EntryDate == tranDate);
            }

            foreach (var transaction in transactions)
            {
                dt.Rows.Add(
                    transaction.EntryDate.ToString("yyyy MM dd"),
                    transaction.DebitLedger,
                    transaction.CreditLedger,
                    transaction.IsoCode,
                    transaction.Amount,
                    transaction.Description,
                    transaction.Reversal ? "Yes" : "No");
            }
            using (var wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "manual_entry.xlsx");
                }
            }
        }
    }
}