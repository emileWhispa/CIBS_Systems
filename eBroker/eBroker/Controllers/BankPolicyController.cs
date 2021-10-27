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
        eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
       
        public ActionResult ListLoanPolicy(string an)
        {
            var loanPolicy = _dc.Policy_Loan_Account.Where(x => x.loan_account == an).OrderByDescending(x => x.Id).ToList();
            var loanDetails = _dc.Policy_Loan_Account.Where(x => x.loan_account == an).FirstOrDefault();
            ViewBag.LoanDetails = loanDetails;
            var loanTypes = (from r in _dc.LoanType.ToList() select new SelectListItem { Text = r.loan_type, Value = r.Id.ToString() }).ToList();
            ViewBag.LoanTypes = loanTypes;
            return View(loanPolicy);
        }


        public ActionResult ListBankCollateral()
        {
            var banks = (from r in _dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            if (AppUserData.Category == "5")//Bank Staff
            {
                banks = (from r in _dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            }

            ViewBag.Banks = banks;
            ViewBag.AppUserData = AppUserData;
            return View();
        }

        public ActionResult ListBankCollateralView(string query, string bankName)
        {
            var collateral = new List<BankCollateral>();
            try
            {
                //var banks = (from r in dc.Bank.Where(x => x.BankName == BankName).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                //ViewBag.Banks = banks;

                if (string.IsNullOrEmpty(bankName))
                {
                    Danger("Kindly select the bank", true);
                    return RedirectToAction("ListBankCollateral");
                }
                if (string.IsNullOrEmpty(query))
                    collateral = _dc.BankCollateral.OrderByDescending(x => x.MarketValue).ToList();
                else
                    collateral = _dc.BankCollateral.Where(x => x.AccountNumber.Contains(query) || x.CollateralType.Contains(query) || x.CollateralDescription.Contains(query) || x.BankLoans.AccountName.Contains(query)).OrderByDescending(x => x.MarketValue).ToList();
                if (!string.IsNullOrEmpty(bankName))
                    collateral = collateral.Where(x => x.Banks.BankName == bankName).ToList();
                ViewBag.BankName = bankName;
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
            var banks = (from r in _dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            if (AppUserData.Category == "5")//Bank Staff
            {
                banks = (from r in _dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
            }
            ViewBag.Banks = banks;
            ViewBag.AppUserData = AppUserData;
            return View();
        }

        public ActionResult ListBankLoansView(string query, string bankName)
        {
            var loans = new List<BankLoan>();
            try
            {
                //var banks = (from r in dc.Bank.Where(x => x.BankName == BankName).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                //ViewBag.Banks = banks;

                if (string.IsNullOrEmpty(bankName))
                {
                    Danger("Kindly select the bank", true);
                    return RedirectToAction("ListBankCollateral");
                }
                if (string.IsNullOrEmpty(query))
                    loans = _dc.BankLoans.Where(x=>x.Status!="Closed" ).OrderByDescending(x => x.DisbursementDate).ToList();
                else
                    loans = _dc.BankLoans.Where(x => x.Status != "Closed").Where(x => x.AccountNumber.Contains(query) || x.ProductName.Contains(query) || x.AccountName.Contains(query)).OrderByDescending(x => x.DisbursementDate).ToList();
                if (!string.IsNullOrEmpty(bankName))
                    loans.Where(x => x.Banks.BankName== bankName).ToList();
                ViewBag.BankName = bankName;
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
                var reports = (from r in _dc.BankReports.OrderBy(x => x.ReportName).ToList() select new SelectListItem { Text = r.ReportName, Value = r.Id.ToString() }).ToList();
                ViewBag.Reports = reports;
                var banks = (from r in _dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                if (AppUserData.Category == "5")//Bank Staff
                {
                    banks = (from r in _dc.Bank.Where(x=>x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                }

                ViewBag.AppUserData = AppUserData;
                ViewBag.Banks = banks;
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult ExportToExcel(string reportId, string startDate, string endDate, string searchKey, string bankName)
        {
            try
            {
                if (string.IsNullOrEmpty(reportId))
                {
                    Danger("Invalid Report", true);
                    var reports = (from r in _dc.BankReports.OrderBy(x => x.ReportName).ToList() select new SelectListItem { Text = r.ReportName, Value = r.Id.ToString() }).ToList();
                    ViewBag.Reports = reports;
                    var banks = (from r in _dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    if (AppUserData.Category == "5")//Bank Staff
                    {
                        banks = (from r in _dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    }

                    ViewBag.Banks = banks;

                    return View("Reports");
                }

                if (string.IsNullOrEmpty(bankName))
                {
                    Danger("You must select the bank", true);
                    var reports = (from r in _dc.BankReports.OrderBy(x => x.ReportName).ToList() select new SelectListItem { Text = r.ReportName, Value = r.Id.ToString() }).ToList();
                    ViewBag.Reports = reports;
                    var banks = (from r in _dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    if (AppUserData.Category == "5")//Bank Staff
                    {
                        banks = (from r in _dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    }

                    ViewBag.Banks = banks;

                    return View("Reports");
                }
                DataTable dt = new DataTable();

                dt = Toolkit.ReturnDatatable("Select * from BankReports where ReportId=" + reportId);
                if (dt == null || dt.Rows.Count != 1)
                {
                    Danger("Invalid Report", true);
                    var reports = (from r in _dc.BankReports.OrderBy(x => x.ReportName).ToList() select new SelectListItem { Text = r.ReportName, Value = r.Id.ToString() }).ToList();
                    ViewBag.Reports = reports;
                    var banks = (from r in _dc.Bank.ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    if (AppUserData.Category == "5")//Bank Staff
                    {
                        banks = (from r in _dc.Bank.Where(x => x.Id == AppUserData.CompanyID).ToList() select new SelectListItem { Text = r.BankName, Value = r.BankName }).ToList();
                    }

                    ViewBag.Banks = banks;

                    return View("Reports");
                }
                DataRow row = dt.Rows[0];
                string cmd = @"SELECT " + row["ExcelColumnList"].ToString() + " FROM " + row["Datasource"].ToString() + " WHERE BankName='" + bankName+"'";
                ;
                Console.Out.WriteLine(cmd);
                if("Vw_Expired_Policy_NotRenewed" == row["Datasource"].ToString())
                {
                    var query = "SELECT i.policy_no,i.policy_type,CONVERT(varchar(25), i.expiry_dt, 106) AS [Expiry Date],CONVERT(varchar(25), i.effective_dt, 106) AS [Effective Date],i.net_premium,b.BankName,p.product_name,ins.company_short_name,c.client_name,c.mobile,i.total_paid,ln.loan_account,CONVERT(varchar(25), ln.loan_disbursement_date, 106) AS [Loan disbursement Date],CONVERT(varchar(25), ln.loan_expiry_date, 106) AS [Loan expiry Date],bLn.Branch,bLn.AccountManager,bLn.Status FROM insurance_policy i INNER JOIN Bank b ON b.Bank_ID=i.interest_bank_id INNER JOIN Insurance_Product p ON p.product_id=i.product_id INNER JOIN Partner ins ON ins.company_code=i.insurance_id INNER JOIN Client c ON c.client_id=i.client_id LEFT JOIN Policy_Loan_Account ln ON ln.contract_id=i.contract_id LEFT JOIN BankLoans bLn ON bLn.AccountNumber=ln.loan_account WHERE i.renewed=0 and b.BankName='" + bankName+"'";
                    
                    if (row["DateFilter"].ToString().ToUpper() == "TRUE")
                    {
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            query += " AND i.expiry_dt>='" + startDate + "'";
                        }

                        if (!string.IsNullOrEmpty(endDate))
                        {
                            query += " AND i.expiry_dt<='" + endDate + "'";
                        }
                    }
                    Toolkit.ExportListUsingEPPlus(query, row["ReportName"].ToString());
                    return View();
                }
                if("Vw_Renewed" == row["Datasource"].ToString())
                {
                    var query = "SELECT i.policy_no,i.policy_type,CONVERT(varchar(25), i.expiry_dt, 106) AS [Expiry Date],CONVERT(varchar(25), i.effective_dt, 106) AS [Effective Date],i.net_premium,b.BankName,p.product_name,ins.company_short_name,c.client_name,c.mobile,i.total_paid,ln.loan_account,CONVERT(varchar(25), ln.loan_disbursement_date, 106) AS [Loan disbursement Date],CONVERT(varchar(25), ln.loan_expiry_date, 106) AS [Loan expiry Date],bLn.Branch,bLn.AccountManager,bLn.Status FROM insurance_policy i INNER JOIN Bank b ON b.Bank_ID=i.interest_bank_id INNER JOIN Insurance_Product p ON p.product_id=i.product_id INNER JOIN Partner ins ON ins.company_code=i.insurance_id INNER JOIN Client c ON c.client_id=i.client_id LEFT JOIN Policy_Loan_Account ln ON ln.contract_id=i.contract_id LEFT JOIN BankLoans bLn ON bLn.AccountNumber=ln.loan_account WHERE i.renewed=1 and b.BankName='" + bankName+"'";
                    
                    if (row["DateFilter"].ToString().ToUpper() == "TRUE")
                    {
                        if (!string.IsNullOrEmpty(startDate))
                        {
                            query += " AND i.expiry_dt>='" + startDate + "'";
                        }

                        if (!string.IsNullOrEmpty(endDate))
                        {
                            query += " AND i.expiry_dt<='" + endDate + "'";
                        }
                    }
                    Toolkit.ExportListUsingEPPlus(query, row["ReportName"].ToString());
                    return View();
                }
                if (!string.IsNullOrEmpty(searchKey))
                {
                    string filter = row["SearchFilter"].ToString().Contains("%") ? searchKey + "%'" : (row["SearchFilter"].ToString().Contains("'") ? searchKey + "'" : searchKey);
                    cmd += " AND " + row["SearchFilter"].ToString() + filter;
                }
                if (row["DateFilter"].ToString().ToUpper() == "TRUE")
                {
                    if (!string.IsNullOrEmpty(startDate))
                    {
                            cmd += " AND " + row["DateFilterColumn"].ToString() + ">='" + startDate + "'";
                    }

                    if (!string.IsNullOrEmpty(endDate))
                    {
                            cmd += " AND " + row["DateFilterColumn"].ToString() + "<='" + endDate + "'";
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
        public ActionResult GetReportDetails(int reportId)
        {
            var rp = (from x in _dc.BankReports
                      where x.Id == reportId
                      select new { x.Id, x.Datasource, x.DateFilter, x.DateFilterColumn, x.ExcelColumnList, x.ReportName, x.SearchFilter, x.ShowReport, x.SortBy, x.SearchFilterDescription, x.DateFilterDescription });
            return Json(rp.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

    }
}