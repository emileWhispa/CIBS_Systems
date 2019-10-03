using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.Data.SqlClient;
using System.Configuration;

namespace eBroker
{
    public static class Toolkit
    {
        public static void ExportListUsingEPPlus(string query, string sheetName)
        {
            DataTable dt = new DataTable();
            dt = ReturnDatatable(query);
            ExcelPackage excel = new ExcelPackage();
            var workSheet = excel.Workbook.Worksheets.Add(sheetName);
            workSheet.Cells[1, 1].LoadFromDataTable(dt, true);
            using (var memoryStream = new MemoryStream())
            {
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;  filename=" + sheetName.Replace(" ", "") + "_" + DateTime.Today.ToString("dd-MM-yyyy").Replace("-", "_") + ".xlsx");
                excel.SaveAs(memoryStream);
                memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();
            }
        }

        public static string AuditTrailConnectionString(string login, string pass)
        {
            string t = ConfigurationManager.ConnectionStrings["eBroker.Properties.Settings.dynamicConnnection"].ConnectionString;
            eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
            t = t.Replace("{user}", login).Replace("{password}", securityComponents.Cryptography.Decrypt(pass));
            return t;
        }

        public static DataTable ReturnDatatable(string query)
        {
            using (SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString))
            {
                dbConnection.Open();
                //Deleting existing records for the batch
                SqlCommand cmd = new SqlCommand(query, dbConnection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static int RunSQLCommand(string command)
        {
            using (SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString))
            {
                dbConnection.Open();
                //Deleting existing records for the batch
                SqlCommand cmd = new SqlCommand(command, dbConnection);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}