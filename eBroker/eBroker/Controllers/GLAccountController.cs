using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using ClosedXML.Excel;
using eBroker.DAL;
using Forex.ViewModels;

namespace eBroker.Controllers
{
    [Authorize]
    public class GlAccountController : BaseController
    {
        eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
        public ActionResult ListGlAccount(string query)
        {

            try
            {
                var listGLs = new List<GL_Account>();

                if (string.IsNullOrEmpty(query))
                {
                    listGLs = _dc.GL_Account.OrderBy(x => x.LedgerNo).ToList();
                }
                else
                    listGLs = _dc.GL_Account.Where(x => x.LedgerDescription.Contains(query)).OrderBy(x => x.LedgerNo).ToList();
                return View(listGLs);
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult GeneralLedgerInfo(TransactionFilter filter)
        {
            var model = new Vw_GeneralLedger();
            try
            {
                if (filter.StartDate != null && filter.EndDate != null)
                {
                    IQueryable<Vw_GeneralLedger> ledgerHist = _dc.Vw_GeneralLedger
                        .Where(x => (x.EffectiveDate >= filter.StartDate && x.EffectiveDate <= filter.EndDate))
                        .OrderBy(x => x.Id);

                    ViewBag.StartDate = filter.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd");
                    ViewBag.EndDate = filter.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd");

                    return View("ViewGeneralLedger", ledgerHist);
                }
            }

            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message, true);
            }
            return View(model);
        }
        
        public FileResult ToExcel(TransactionFilter viewModel)
        {

            var dt = new DataTable("Grid");
            dt.Columns.AddRange(new[] {
                new DataColumn("Date"),
                new DataColumn("Ledger No."),
                new DataColumn("Ledger Description"),
                new DataColumn("Description"),
                new DataColumn("Currency") ,
                new DataColumn("Debit"),
                new DataColumn("Credit"),
            });

            var tranDate = DateTime.Now;
            //            ViewBag.TransactionDate = tranDate;

            IQueryable<Vw_GeneralLedger> generalLedger;
            if (viewModel.StartDate != null && viewModel.EndDate != null)
            {
                generalLedger = _dc.Vw_GeneralLedger
                    .Where(t => t.EffectiveDate >= viewModel.StartDate &&
                                t.EffectiveDate <= viewModel.EndDate).OrderBy(x => x.Id);
            }
            else
            {
                generalLedger = _dc.Vw_GeneralLedger.OrderBy(x => x.Id).Take(50);
            }
            foreach (var transaction in generalLedger)
            {
                dt.Rows.Add(
                    transaction.EffectiveDate.ToString("yyyy-MM-dd"),
                    transaction.LedgerNo,
                    transaction.LedgerDescription,
                    transaction.Description,
                    transaction.IsoCode,
                    transaction.Debit,
                    transaction.Credit);
            }
            using (var wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "GeneralLedger.xlsx");
                }
            }
        }
        
        [HttpGet]
        public ActionResult GlAccountInfo(string query)
        {
            var model = new GL_Account();
           
            try
            {
                model = new GL_Account {Active = true, ID = 0};
            }

            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message);
            }
            
            ViewBag.GLAccountList = _dc.GL_Account.ToList();
            return View(model);
        }
        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GlAccountEdit(int id)
        {
            var model = new GL_Account();
            try
            {
                model = _dc.GL_Account.FirstOrDefault(x => x.ID == id);
            }
            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message, true);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CreateGlAccount(GL_Account txn)
        {
            if (txn.ID == 0)
            {
                txn.ID = 0;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    //Getting the leder No
                    if (txn.ID == 0)
                    {
                        var gLs = _dc.GL_Account.Where(x => x.ParentLedgerNo == txn.ParentLedgerNo).ToList();

                        var ledgerNo = 0;

                        if (gLs.Count == 0)
                            ledgerNo = int.Parse(txn.ParentLedgerNo.ToString() + "01");
                        else
                            ledgerNo = gLs.Max(x => x.LedgerNo) + 1;
                        txn.LedgerNo = ledgerNo;
                    }

                    if (txn.ID == 0)
                    {
                        _dc.GL_Account.Add(txn);
                        var res = _dc.SaveChanges();
                        if (res > 0)
                        {
                            //Success("Record Saved Successfully", true);
                            var currency = _dc.GL_Account.OrderBy(x => x.ParentLedgerNo).OrderBy(x => x.LedgerNo).ToList();
                            return PartialView("ListGLAccount", currency);
                        }

                        const string resp = "Unable to insert the new GL Account";
                        throw new Exception(resp);
                    }
                    else//Update
                    {
                        var oldValGlAc = _dc.GL_Account.FirstOrDefault(x => x.ID == txn.ID);
                        if (oldValGlAc != null)
                        {
                            var copyOldValGlAccount = new GL_Account
                            {
                                ID = oldValGlAc.ID,
                                LedgerNo = oldValGlAc.LedgerNo,
                                LedgerDescription = oldValGlAc.LedgerDescription,
                                LedgerType = oldValGlAc.LedgerType,
                                ParentLedgerNo = oldValGlAc.ParentLedgerNo,
                                GL_Level = oldValGlAc.GL_Level,
                                Active = oldValGlAc.Active
                            };
                            //Setting up the new values
                            oldValGlAc.LedgerNo = txn.LedgerNo;
                            oldValGlAc.LedgerDescription = txn.LedgerDescription;
                            oldValGlAc.LedgerType = txn.LedgerType;
                            oldValGlAc.ParentLedgerNo = txn.ParentLedgerNo;
                            oldValGlAc.GL_Level = txn.GL_Level;
                            oldValGlAc.Active = txn.Active;
                            _dc.SaveChanges();
                            //CreateAuditTrail("Update", "GLAccount", AppUserData.Login, txn.ID, copyOldValGlAccount, txn);
                        }
                        var glaccounts = _dc.GL_Account.OrderBy(x => x.ParentLedgerNo).ThenBy(x => x.LedgerNo).ToList();
                        return PartialView("ListGLAccount", glaccounts);
                    }
                }
                catch (Exception ex)
                {
                    ex = GetInnerException(ex);
                    Danger(ex.Message);
                    return View("GLAccountInfo", txn);
                }
            }

            Danger("Input validation errors! Kindly check if all fields are populated properly", true);
            return View("GLAccountInfo", txn);
        }



    }
}