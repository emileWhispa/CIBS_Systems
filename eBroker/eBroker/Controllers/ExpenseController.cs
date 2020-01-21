using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using eBroker.Services;
using Forex.ViewModels;

namespace eBroker.Controllers
{
    public class ExpenseController : BaseController
    {
        //
        // GET: /Custome/
        eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);


        [HttpGet]
        public ActionResult ExpenseInfo(TransactionFilter filter)
        {
            try
            {
                IQueryable<Vw_Expense_Voucher> listTransactions;
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    listTransactions = _dc.Vw_Expense_Voucher
                        .Where(x => (x.ExpenseDate >= filter.StartDate && x.ExpenseDate <= filter.EndDate)
                                    )
                        .OrderByDescending(x => x.Id);

                    ViewBag.StartDate = filter.StartDate.GetValueOrDefault().ToString("MM/dd/yyyy");
                    ViewBag.EndDate = filter.EndDate.GetValueOrDefault().ToString("MM/dd/yyyy");

                    return View("ViewExpense", listTransactions.ToList());
                }


                listTransactions = _dc.Vw_Expense_Voucher
                    .Where(x => x.UserID == AppUserData.Login)
                    .OrderByDescending(x => x.Id);

                ViewBag.lstTrans = listTransactions.ToList();

                var model = new Expense
                {
                    ExpenseDate = DateTime.Now,
                    UserID = AppUserData.Login
                };


                //Dropdownlist definition
                var expenseCategories =
                (from e in _dc.GL_Account.Where(x =>
                        x.GL_Level == 2 && x.LedgerNo % 400 < 100).ToList()
                    select new SelectListItem {Text = e.LedgerDescription, Value = e.LedgerNo.ToString()}).ToList();

                ViewBag.ExpenseCategory = expenseCategories;
                var paymentLedgers =
                    (from e in _dc.GL_Account.Where(x =>
                            x.GL_Level == 3 && x.LedgerNo < 10300).ToList()
                        select new SelectListItem {Text = e.LedgerDescription, Value = e.LedgerNo.ToString()})
                    .ToList(); //Cash (101) or bank (102)
                ViewBag.PaymentLedger = paymentLedgers;


                return View("ExpenseInfo", model);
            }

            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message, true);
            }
            return View();
        }
        
        public FileResult ToExcel(TransactionFilter viewModel)
        {
            var tranDate = DateTime.Now;
            IQueryable<Vw_Expense_Voucher> expenseVouchers;
            if (viewModel.StartDate != null && viewModel.EndDate != null)
            {
                expenseVouchers = _dc.Vw_Expense_Voucher
                    .Where(t =>
                                t.ExpenseDate >= viewModel.StartDate &&
                                t.ExpenseDate <= viewModel.EndDate);
            }
            else
            {
                expenseVouchers =
                    _dc.Vw_Expense_Voucher.Where(t =>
                        t.ExpenseDate == tranDate);
            }
            var stream = ExpensesService.GetExcelData(expenseVouchers);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "expenses_branches_" + DateTime.Now + ".xlsx");
        }


        public ActionResult Reverse(int id)
        {
            var expense = _dc.Expense.Find(id);
            if (expense == null) return RedirectToAction("ExpenseInfo");

            expense.Reversal = !expense.Reversal;
            _dc.Entry(expense).State = EntityState.Modified;
            _dc.SaveChanges();
            return RedirectToAction("ExpenseInfo");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult ExpenseEdit(int id)
        {
            var model = new Expense();
            try
            {
                model = _dc.Expense.FirstOrDefault(x => x.Id == id);
                if (model != null)
                {
                    ViewBag.ExpenseCat = model.ExpenseLedgerNo.ToString().Substring(1, 3);
                    model.ExpenseCategory = int.Parse(model.ExpenseLedgerNo.ToString().Substring(0, 3));
                }
            }
            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateExpense(Expense e)
        {
            e.UserID = AppUserData.Login;
            if (!ModelState.IsValid)
            {
                Warning("Please! Make sure the data entered is correct");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                  if (e.Id == 0)
                  {
                      e.CreatedOn = DateTime.Now;
                      e.Id = 0;
                      e.ExpenseDate = DateTime.Now;

                      _dc.Expense.Add(e);
                      int i = _dc.SaveChanges();
                      Console.Out.Write(i);
                      Success("Expense successfully saved");
                      return new HttpStatusCodeResult(HttpStatusCode.OK);
                  }
                  e.ExpenseDate = DateTime.Now;

                  var oldValExp = _dc.Expense.FirstOrDefault(x => x.Id == e.Id);
                  if (oldValExp == null) return RedirectToAction("ExpenseInfo");

                  var copyOldValExpense = new Expense
                  {
                      Id = oldValExp.Id,
                      Amount = oldValExp.Amount,
                      CreatedOn = oldValExp.CreatedOn,
                      CurrencyID = oldValExp.CurrencyID,
                      Description = oldValExp.Description,
                      ExpenseDate = oldValExp.ExpenseDate,
                      ExpenseLedgerNo = oldValExp.ExpenseLedgerNo,
                      PaymentLedgerNo = oldValExp.PaymentLedgerNo,
                      Reversal = oldValExp.Reversal,
                      UserID = oldValExp.UserID
                  };
                  //Setting up the new values
                  oldValExp.Amount = e.Amount;
                  oldValExp.CurrencyID = e.CurrencyID;
                  oldValExp.Description = e.Description;
                  oldValExp.ExpenseDate = e.ExpenseDate;
                  oldValExp.ExpenseLedgerNo = e.ExpenseLedgerNo;
                  oldValExp.PaymentLedgerNo = e.PaymentLedgerNo;
                  oldValExp.Reversal = e.Reversal;
                  _dc.SaveChanges();

                  //CreateAuditTrail("Update", "Expense", AppUserData.Login, e.Id, copyOldValExpense, e);
                  Information("Expense successfully updated");
                  return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.NoContent);
            }
        }

        public ActionResult PrintExpense(string eId) //Transaction Id
        {
            var path = "";
            try
            {
                var bId =eId;

                var dt = Toolkit.ReturnDatatable("select * from Vw_Expense_Voucher where ID=" + bId);
                var rd = new ReportDocument();
                path = Server.MapPath("~/Reports/rptExpenseVoucher.rpt");
                rd.Load(Server.MapPath("~/Reports/rptExpenseVoucher.rpt"));
                rd.SetDataSource(dt);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                rd.Dispose();
                return File(stream, "application/pdf", "rptExpenseVoucher.pdf");
            }
            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message + " Path: " + path);
            }
            return RedirectToAction("ExpenseInfo");
        }

    }
}