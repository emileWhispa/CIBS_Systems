using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;
using System.Data;

namespace eBroker.Controllers
{
    public class BankPolicyController:BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
       
        public ActionResult ListLoanPolicy(string AN)
        {
            var loanPolicy = dc.Policy_Loan_Account.Where(x => x.loan_account == AN).OrderByDescending(x => x.Id).ToList();
            var loanDetails = dc.Policy_Loan_Account.Where(x => x.loan_account == AN).FirstOrDefault();
            ViewBag.LoanDetails = loanDetails;
            var loanTypes = (from r in dc.LoanType.ToList() select new SelectListItem { Text = r.loan_type, Value = r.Id.ToString() }).ToList();
            ViewBag.LoanTypes = loanTypes;
            return View(loanPolicy);
        }


        public ActionResult ListBankCollateral()
        {
            var banks = (from r in dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            if (AppUserData.Category == "5")//Bank Staff
            {
                banks = (from r in dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            }

            ViewBag.Banks = banks;
            return View();
        }

        public ActionResult ListBankCollateralView(string query, string BankName)
        {
            var collateral = new List<BankCollateral>();
            try
            {
                //var banks = (from r in dc.Bank.Where(x => x.BankName == BankName).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                //ViewBag.Banks = banks;

                if (string.IsNullOrEmpty(BankName))
                {
                    Danger("Kindly select the bank", true);
                    return RedirectToAction("ListBankCollateral");
                }
                if (string.IsNullOrEmpty(query))
                    collateral = dc.BankCollateral.OrderByDescending(x => x.MarketValue).ToList();
                else
                    collateral = dc.BankCollateral.Where(x => x.AccountNumber.Contains(query) || x.CollateralType.Contains(query) || x.CollateralDescription.Contains(query) || x.BankLoans.AccountName.Contains(query)).OrderByDescending(x => x.MarketValue).ToList();
                if (!string.IsNullOrEmpty(BankName))
                    collateral = collateral.Where(x => x.Banks.BankName == BankName).ToList();
                ViewBag.BankName = BankName;
                return View(collateral);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult ListBankLoans()
        {
            var banks = (from r in dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            if (AppUserData.Category == "5")//Bank Staff
            {
                banks = (from r in dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            }
            ViewBag.Banks = banks;
            return View();
        }

        public ActionResult ListBankLoansView(string query, string BankName)
        {
            var loans = new List<BankLoan>();
            try
            {
                //var banks = (from r in dc.Bank.Where(x => x.BankName == BankName).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                //ViewBag.Banks = banks;

                if (string.IsNullOrEmpty(BankName))
                {
                    Danger("Kindly select the bank", true);
                    return RedirectToAction("ListBankCollateral");
                }
                if (string.IsNullOrEmpty(query))
                    loans = dc.BankLoans.Where(x=>x.Status!="Closed" ).OrderByDescending(x => x.DisbursementDate).ToList();
                else
                    loans = dc.BankLoans.Where(x => x.Status != "Closed").Where(x => x.AccountNumber.Contains(query) || x.ProductName.Contains(query) || x.AccountName.Contains(query)).OrderByDescending(x => x.DisbursementDate).ToList();
                if (!string.IsNullOrEmpty(BankName))
                    loans.Where(x => x.Banks.BankName== BankName).ToList();
                ViewBag.BankName = BankName;
                return View(loans);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult Reports()
        {
            try
            {
                var reports = (from r in dc.BankReports.OrderBy(x => x.ReportName).ToList() select new SelectListItem { Text = r.ReportName, Value = r.Id.ToString() }).ToList();
                ViewBag.Reports = reports;
                var banks = (from r in dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                if (AppUserData.Category == "5")//Bank Staff
                {
                    banks = (from r in dc.Bank.Where(x=>x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                }
                ViewBag.Banks = banks;
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult ExportToExcel(string ReportId, string startDate, string EndDate, string SearchKey, string BankName)
        {
            try
            {
                if (string.IsNullOrEmpty(ReportId))
                {
                    Danger("Invalid Report", true);
                    var reports = (from r in dc.BankReports.OrderBy(x => x.ReportName).ToList() select new SelectListItem { Text = r.ReportName, Value = r.Id.ToString() }).ToList();
                    ViewBag.Reports = reports;
                    var banks = (from r in dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    if (AppUserData.Category == "5")//Bank Staff
                    {
                        banks = (from r in dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    }

                    ViewBag.Banks = banks;

                    return View("Reports");
                }

                if (string.IsNullOrEmpty(BankName))
                {
                    Danger("You must select the bank", true);
                    var reports = (from r in dc.BankReports.OrderBy(x => x.ReportName).ToList() select new SelectListItem { Text = r.ReportName, Value = r.Id.ToString() }).ToList();
                    ViewBag.Reports = reports;
                    var banks = (from r in dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    if (AppUserData.Category == "5")//Bank Staff
                    {
                        banks = (from r in dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    }

                    ViewBag.Banks = banks;

                    return View("Reports");
                }
                DataTable dt = new DataTable();

                dt = Toolkit.ReturnDatatable("Select * from BankReports where ReportId=" + ReportId);
                if (dt == null || dt.Rows.Count != 1)
                {
                    Danger("Invalid Report", true);
                    var reports = (from r in dc.BankReports.OrderBy(x => x.ReportName).ToList() select new SelectListItem { Text = r.ReportName, Value = r.Id.ToString() }).ToList();
                    ViewBag.Reports = reports;
                    var banks = (from r in dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    if (AppUserData.Category == "5")//Bank Staff
                    {
                        banks = (from r in dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    }

                    ViewBag.Banks = banks;

                    return View("Reports");
                }
                DataRow row = dt.Rows[0];
                string cmd = @"SELECT " + row["ExcelColumnList"].ToString() + " FROM " + row["Datasource"].ToString() + " WHERE BankName='" + BankName+"'";
                ;
                if (!string.IsNullOrEmpty(SearchKey))
                {
                    string filter = row["SearchFilter"].ToString().Contains("%") ? SearchKey + "%'" : (row["SearchFilter"].ToString().Contains("'") ? SearchKey + "'" : SearchKey);
                    cmd += " AND " + row["SearchFilter"].ToString() + filter;
                }
                if (row["DateFilter"].ToString().ToUpper() == "TRUE")
                {
                    if (!string.IsNullOrEmpty(startDate))
                    {
                            cmd += " AND " + row["DateFilterColumn"].ToString() + ">='" + startDate + "'";
                    }

                    if (!string.IsNullOrEmpty(EndDate))
                    {
                            cmd += " AND " + row["DateFilterColumn"].ToString() + "<='" + EndDate + "'";
                    }
                }
                if (row["SortBy"].ToString().Length > 0)
                    cmd += " ORDER BY " + row["SortBy"].ToString();
                Toolkit.ExportListUsingEPPlus(cmd, row["ReportName"].ToString());
                return View();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetReportDetails(int ReportId)
        {
            var rp = (from x in dc.BankReports
                      where x.Id == ReportId
                      select new { x.Id, x.Datasource, x.DateFilter, x.DateFilterColumn, x.ExcelColumnList, x.ReportName, x.SearchFilter, x.ShowReport, x.SortBy, x.SearchFilterDescription, x.DateFilterDescription });
            return Json(rp.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

    }
}