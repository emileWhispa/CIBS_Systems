using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using CrystalDecisions.Shared;

namespace eBroker.Controllers
{
    public class ReportingController : BaseController
    {
        eBroker.BrokerDataContext _dc =
            new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"]
                .ConnectionString);

        public ActionResult ExportToPdf(string id, string reportName, string filterName, string orderBy)
        {
            DataTable dt = new DataTable();
            dt = Toolkit.ReturnDatatable("select * from Vw_" + reportName + " where " + filterName + "=" + id +
                                         (string.IsNullOrEmpty(orderBy) ? "" : " order by " + orderBy));
            ReportDocument rd = new ReportDocument();
            string path = Server.MapPath("~/Reports/" + reportName + ".rpt");
            rd.Load(Server.MapPath("~/Reports/" + reportName + ".rpt"));
            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", reportName + ".pdf");
        }

        public ActionResult Reports()
        {
            try
            {
                var reports = (from r in _dc.SystemReports.OrderBy(x => x.ReportName).ToList()
                    select new SelectListItem {Text = r.ReportName, Value = r.Id.ToString()}).ToList();
                ViewBag.Reports = reports;
                //return View();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }

            return View();
        }

        public ActionResult PdfReports()
        {
            try
            {
                //ViewBag.Branch = GetBranches();
            }
            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message, true);
            }

            return View();
        }
        
        public ActionResult FinancialReportView(string reportId, string branch, string startDate, string endDate)
        {

            //Start Date is mandatory for all reports
            if (string.IsNullOrEmpty(startDate) && reportId != "AB")
            {
                Danger("Start Date Must be selected");
                return View("PDFReports");
            }

            if (reportId == "CB" && string.IsNullOrEmpty(endDate))
            {
                Danger("End Date Must be selected for Cash Book");
                return View("PDFReports");
            }

            if (reportId.Equals("TR") && string.IsNullOrWhiteSpace(branch))
            {
                Danger("Please select the branch");
                return View("PDFReports");
            }

            if (reportId.Equals("AB"))
            {
                //var currentBalances = _dc.Database.SqlQuery<VwCurrentBalance>("SP_AVAILABLE_BALANCE @ForexId",
                 //   new SqlParameter("@ForexId", AppUserData.CompanyId)).OrderBy(x => x.IsoCode).ToList();
               // return View("AvailableBalances", currentBalances);
            }

            if (reportId.Equals("TR"))
            {
                
            }


            var rd = new ReportDocument();
            var ds = new DataSet();
            try
            {
                string queryStatement;

                string path;
                switch (reportId)
                {
                    case "BS":
                        //Don't change this order my friend!!================================================================
                        queryStatement = " and substring(convert(varchar(5),a.LedgerNo),1,1) in ('1','2')";

                        var dtLevel1 = Toolkit.ReturnDatatable(
                            @"select distinct convert(varchar(10),a.LedgerNo) as LedgerNo, convert(varchar(10),a.ParentLedgerNo) as ParentLedgerNo,a.LedgerDescription,sum(b.Balance*b.middleRate) as Balance
							 from GL_ACCOUNT a,GL_Balance b
							 where a.GL_Level=1 and a.LedgerNo = substring(convert(varchar(10),b.LedgerNo),1,1) 
							and b.LedgerNo not in(10410,20310,10409) and b.BalanceDate='" + startDate + @"' " +
                            queryStatement + @" 
							group by a.LedgerNo, a.ParentLedgerNo, a.LedgerDescription
							having sum(b.Balance)<>0");
                        var dtLevel2 = Toolkit.ReturnDatatable(
                            @"select distinct convert(varchar(10),a.LedgerNo)  as LedgerNo, convert(varchar(10),a.ParentLedgerNo) as ParentLedgerNo, a.LedgerDescription,sum(b.Balance*b.middleRate) as Balance from GL_ACCOUNT a,GL_Balance b
							where a.GL_Level=2 and a.LedgerNo =substring(convert(varchar(10),b.LedgerNo),1,3)
							and b.LedgerNo not in(10410,20310,10409) and b.BalanceDate='" + startDate + @"'  " +
                            queryStatement + @"  
							group by a.LedgerNo, a.ParentLedgerNo, a.LedgerDescription
							having (sum(b.Balance)<>0 OR (a.LedgerNo=206))");

                        var dtLevel3 = Toolkit.ReturnDatatable(
                            @"select distinct convert(varchar(10),a.LedgerNo)  as LedgerNo, convert(varchar(10),a.ParentLedgerNo) as ParentLedgerNo, a.LedgerDescription,c.IsoCode,b.Balance*b.middleRate as Balance from GL_ACCOUNT a,GL_Balance b,
							Currency c where a.GL_Level=3 and a.LedgerNo =b.LedgerNo and b.CurrencyID =c.uid
							and b.LedgerNo not in(10410,20310,10409) and  b.BalanceDate='" + startDate + @"'  " +
                            queryStatement + @"  and (b.balance<>0 OR (b.LedgerNo=20601 and c.IsoCode='RWF'))");
                        var dtLevel4 =
                            Toolkit.ReturnDatatable("exec SP_INCOME_STATEMENT '" + startDate + "'");
                        //Updating the Profit of the year based on the net profit before tax
                        double net = 0;
                        double grossProfit = 0;
                        if (dtLevel4 != null && dtLevel4.Rows.Count > 0)
                        {
                            for (var j = 0; j < dtLevel4.Rows.Count; j++)
                            {
                                if (dtLevel4.Rows[j].ItemArray.GetValue(0).ToString().ToUpper() == "INCOME")
                                    net += double.Parse(dtLevel4.Rows[j].ItemArray.GetValue(2).ToString());
                                else
                                    net -= double.Parse(dtLevel4.Rows[j].ItemArray.GetValue(2).ToString());
                            }
                        }

                        //Finding Profit for the year Ledger and replace its content
                        for (var i = 0; i < dtLevel3.Rows.Count; i++)
                        {
                            if (dtLevel3.Rows[i].ItemArray.GetValue(0).ToString() == "20601")
                            {
                                grossProfit =
                                    double.Parse(dtLevel3.Rows[i].ItemArray.GetValue(4)
                                        .ToString()); //Read the existing gross profit
                                dtLevel3.Rows[i][4] = net; //Replacing the gross profit by the net profit
                            }
                        }

                        //Updating the P&L account
                        for (var i = 0; i < dtLevel2.Rows.Count; i++)
                        {
                            if (dtLevel2.Rows[i].ItemArray.GetValue(0).ToString() == "206")
                            {
                                dtLevel2.Rows[i][3] = double.Parse(dtLevel2.Rows[i].ItemArray.GetValue(3).ToString()) -
                                                      (grossProfit -
                                                       net); //Replacing the gross profit by the net profit
                            }
                        }

                        //Updating Total Liabilities
                        for (var i = 0; i < dtLevel1.Rows.Count; i++)
                        {
                            if (dtLevel1.Rows[i].ItemArray.GetValue(0).ToString() == "2")
                            {
                                dtLevel1.Rows[i][3] = double.Parse(dtLevel1.Rows[i].ItemArray.GetValue(3).ToString()) -
                                                      (grossProfit -
                                                       net); //Replacing the gross profit by the net profit
                            }
                        }

                        //==========================================End of order==========================================
                        ds.Merge(dtLevel1);
                        ds.Merge(dtLevel2);
                        ds.Merge(dtLevel3);
                        ds.Merge(dtLevel4);
                        ds.Relations.Add(ds.Tables["Table1"].Columns["LedgerNo"],
                            ds.Tables["Table2"].Columns["ParentLedgerNo"]);
                        ds.Relations.Add(ds.Tables["Table2"].Columns["LedgerNo"],
                            ds.Tables["Table3"].Columns["ParentLedgerNo"]);
                        //Renaming the datatables
                        ds.Tables[0].TableName = "StatementLvl1";
                        ds.Tables[1].TableName = "StatementLvl12";
                        ds.Tables[2].TableName = "StatementLvl13";
                        ds.Tables[3].TableName = "Level4";
                        path = Server.MapPath("~/Reports/Statement.rpt");
                        rd.Load(path);
                        break;
                    case "IS":
                        queryStatement = "exec SP_INCOME_STATEMENT '" + startDate + "'";
                        DataTable dt;
                        dt = Toolkit.ReturnDatatable(queryStatement);
                        ds.Merge(dt);
                        ds.Tables[0].TableName = "IncomeStatement";
                        path = Server.MapPath("~/Reports/IncomeStatement.rpt");
                        rd.Load(Server.MapPath("~/Reports/IncomeStatement.rpt"));
                        break;
                    case "CB":
                    {
                        var dtCashBook = Toolkit.ReturnDatatable("EXEC sp_multiple_days_cash_book '" + startDate + "', '" + endDate + "'");
                        ds.Merge(dtCashBook);
                        ds.Tables[0].TableName = "Cash_Book_Report";
                        path = Server.MapPath("~/Reports/Period_Cash_Book.rpt");
                        rd.Load(Server.MapPath("~/Reports/Period_Cash_Book.rpt"));
                        break;
                    }
                    case "TB":
                    {
                        var cmd =
                            @"SELECT     convert(varchar(20),LedgerNo) as LedgerNo, LedgerDescription, case  when sum(Debit)>sum(Credit) then (sum(Debit)-sum(Credit)) else 0 end TotalDebits,
					  case when sum(Debit)<sum(Credit) then  (sum(Credit)-sum(Debit)) else 0 end TotalCredits,  IsoCode,'" +
                            startDate + "' as StartDate, '" + endDate + "' as EndDate " +
                            " from Vw_TrialBalance where effectiveDate between '" + startDate + "' and '" + endDate + "' " +
                            " group by LedgerNo, LedgerDescription,IsoCode";
                        var dtCashBook = Toolkit.ReturnDatatable(cmd);
                        ds.Merge(dtCashBook);
                        ds.Tables[0].TableName = "Vw_TrialBalance";
                        path = Server.MapPath("~/Reports/rptTrialBalance.rpt");
                        rd.Load(Server.MapPath("~/Reports/rptTrialBalance.rpt"));
                        break;
                    }

                    default:
                        return View("PDFReports");
                }

                rd.SetDataSource(ds);
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                var stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);

                return File(stream, "application/pdf", reportId + ".pdf");
            }
            catch (Exception ex)
            {
                ex = GetInnerException(ex);
                Danger(ex.Message);
            }
            finally
            {
                //Dispose reports objectsFinancialReportView
                rd.Close();
                rd.Dispose();
                ds.Clear();
                ds.Dispose();
                GC.Collect();
            }

            return View("PDFReports");
        }


        public ActionResult ExportToExcel(string reportId, string startDate, string endDate, string searchKey)
        {
            try
            {
                if (string.IsNullOrEmpty(reportId))
                {
                    Danger("Invalid Report", true);
                    return View();
                }

                var dt = Toolkit.ReturnDatatable("Select * from SystemReports where ReportId=" + reportId);
                if (dt == null || dt.Rows.Count != 1)
                {
                    Danger("Invalid Report", true);
                    return View();
                }

                DataRow r = dt.Rows[0];
                string cmd = @"SELECT " + r["ExcelColumnList"].ToString() + " FROM " + r["Datasource"].ToString();
                if (!string.IsNullOrEmpty(searchKey))
                {
                    string filter = r["SearchFilter"].ToString().Contains("%")
                        ? searchKey + "%'"
                        : (r["SearchFilter"].ToString().Contains("'") ? searchKey + "'" : searchKey);
                    cmd += " WHERE " + r["SearchFilter"].ToString() + filter;
                }

                if (r["DateFilter"].ToString().ToUpper() == "TRUE")
                {
                    if (!string.IsNullOrEmpty(startDate))
                    {
                        if (cmd.Contains("WHERE"))
                        {
                            cmd += " AND " + r["DateFilterColumn"].ToString() + ">='" + startDate + "'";
                        }
                        else
                            cmd += " WHERE " + r["DateFilterColumn"].ToString() + ">='" + startDate + "'";
                    }

                    if (!string.IsNullOrEmpty(endDate))
                    {
                        if (cmd.Contains("WHERE"))
                        {
                            cmd += " AND " + r["DateFilterColumn"].ToString() + "<='" + endDate + "'";
                        }
                        else
                            cmd += " WHERE " + r["DateFilterColumn"].ToString() + "<='" + endDate + "'";
                    }
                }

                if (r["SortBy"].ToString().Length > 0)
                    cmd += " ORDER BY " + r["SortBy"].ToString();
                Toolkit.ExportListUsingEPPlus(cmd, r["ReportName"].ToString());
                return View();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }

            return View();
        }

        //Json function to return the bio data details of a given demob number
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetReportDetails(int reportId)
        {
            var rp = (from x in _dc.SystemReports
                where x.Id == reportId
                select new
                {
                    x.Id, x.Datasource, x.DateFilter, x.DateFilterColumn, x.ExcelColumnList, x.ReportName,
                    x.SearchFilter, x.ShowReport, x.SortBy, x.SearchFilterDescription, x.DateFilterDescription
                });
            return Json(rp.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }
    }
}