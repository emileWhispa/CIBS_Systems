using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Data;
using System.Configuration;

namespace eBroker.Controllers
{
    public class ReportingController : BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
        
        public ActionResult ExportToPDF(string Id, string ReportName, string FilterName, string OrderBy)
        {
            DataTable dt = new DataTable();
            dt = Toolkit.ReturnDatatable("select * from Vw_" + ReportName + " where " + FilterName + "=" + Id +(string.IsNullOrEmpty(OrderBy)?"":" order by "+OrderBy));
            ReportDocument rd = new ReportDocument();
            string path = Server.MapPath("~/Reports/" + ReportName + ".rpt");
            rd.Load(Server.MapPath("~/Reports/" + ReportName + ".rpt"));
            rd.SetDataSource(dt);
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "application/pdf", ReportName + ".pdf");
        }

        public ActionResult Reports()
        {
            try
            {
                var reports = (from r in dc.SystemReports.OrderBy(x => x.ReportName).ToList() select new SelectListItem { Text = r.ReportName, Value = r.Id.ToString() }).ToList();
                ViewBag.Reports = reports;
                //return View();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult ExportToExcel(string ReportId, string startDate, string EndDate, string SearchKey)
        {
            try
            {
                if (string.IsNullOrEmpty(ReportId))
                {
                    Danger("Invalid Report", true);
                    return View();
                }
                DataTable dt = new DataTable();

                dt = Toolkit.ReturnDatatable("Select * from SystemReports where ReportId=" + ReportId);
                if (dt == null || dt.Rows.Count != 1)
                {
                    Danger("Invalid Report", true);
                    return View();
                }
                DataRow r = dt.Rows[0];
                string cmd = @"SELECT " + r["ExcelColumnList"].ToString() + " FROM " + r["Datasource"].ToString();
                if (!string.IsNullOrEmpty(SearchKey))
                {
                    string filter = r["SearchFilter"].ToString().Contains("%") ? SearchKey + "%'" : (r["SearchFilter"].ToString().Contains("'") ? SearchKey + "'" : SearchKey);
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

                    if (!string.IsNullOrEmpty(EndDate))
                    {
                        if (cmd.Contains("WHERE"))
                        {
                            cmd += " AND " + r["DateFilterColumn"].ToString() + "<='" + EndDate + "'";
                        }
                        else
                            cmd += " WHERE " + r["DateFilterColumn"].ToString() + "<='" + EndDate + "'";
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
        public ActionResult GetReportDetails(int ReportId)
        {
            var rp = (from x in dc.SystemReports
                      where x.Id == ReportId
                      select new { x.Id, x.Datasource, x.DateFilter, x.DateFilterColumn, x.ExcelColumnList, x.ReportName, x.SearchFilter, x.ShowReport, x.SortBy, x.SearchFilterDescription, x.DateFilterDescription });
            return Json(rp.FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

    }
}
