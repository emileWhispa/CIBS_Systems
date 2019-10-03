﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using Excel;
using System.Data.SqlClient;
using System.Data;

namespace eBroker.Controllers
{
    public class CustomerController : BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
        public ActionResult ListCustomer(string query)
        {
            var Model = new List<Client>();
            try
            {
                this.CustomerInfo(0);
                if (string.IsNullOrEmpty(query))
                    Model = dc.Client.OrderByDescending(x => x.Id).ToList();
                else
                    Model = dc.Client.Where(x => x.client_name.Contains(query) || x.contact_person.Contains(query) || x.mobile.Contains(query)).ToList();
                return View(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();

        }

        //
        // POST: /Customer/Create

        [HttpPost]
        public ActionResult CreateCustomer(Client clt)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    string message = "";
                    Client entity = dc.Client.Where<Client>(e => e.client_national_id == clt.client_national_id || e.mobile == clt.mobile).FirstOrDefault<Client>();
                    if (entity != null)
                    {
                        entity.client_national_id = clt.client_national_id;
                        entity.mobile = clt.mobile;
                        entity.client_name = clt.client_name;
                        entity.contact_person = clt.contact_person;
                        entity.contact_person = clt.contact_person;
                        entity.client_type = clt.client_type;
                        entity.language = clt.language;
                        entity.language = clt.language;
                        entity.mobile2 = clt.mobile2;
                        entity.email = clt.email;
                        entity.physical_address = clt.physical_address;
                        entity.physical_address = clt.physical_address;
                        entity.recruited_by = clt.recruited_by;
                        this.dc.Entry(entity).State = EntityState.Modified;
                        this.dc.SaveChanges();
                        return this.Content("1");
                    }
                    if (clt.Id == 0)
                    {
                        clt.create_dt = DateTime.Now;
                        this.dc.Client.Add(clt);
                        if (this.dc.SaveChanges() <= 0)
                            throw new Exception(message);
                        this.Success("Record Saved Successfully", true);
                    }
                    else
                    {
                        this.dc.Entry(clt).State = EntityState.Modified;
                        this.dc.SaveChanges();
                    }
                    return this.Content("1");
                }
                catch (Exception ex)
                {
                    this.Danger(ex.Message, true);
                }
            }
            this.CustomerInfo(clt.Id);
            return PartialView("CustomerInfo", clt);
        }

        public ActionResult SearchCustomer(string id)
        {
            Client client = this.dc.Client.Where<Client>(e => e.client_national_id == id || e.mobile == id).FirstOrDefault<Client>();
            if (client == null)
                return this.Content("0");
            this.CustomerInfo(client.Id);
            return this.PartialView("CustomerInfo",client);
        }

        [HttpGet]
        public ActionResult CustomerInfo(int Id = 0)
        {
            try
            {
                var Model = dc.Client.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new Client();
                    Model.Id = 0;
                    Model.create_dt = DateTime.Now;
                    Model.user_id = AppUserData.Login;

                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Customer Recruiter
                var recruiter = (from r in dc.eUser.ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                var rec = (from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList();
                recruiter.AddRange(rec);
                ViewBag.recruiter = recruiter;
                //ViewData["recruit"] = recruiter;
                //TempData["recr"] = recruiter;
                var tst = ViewBag.recruiter;
                return PartialView(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        //
        public ActionResult ExportToExcel()
        {
            try
            {
                Toolkit.ExportListUsingEPPlus("select * from Client order by client_name", "Client Listing");
                return View();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult ExcelFileUpload()
        {
            try
            {
                return View();
            }

            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExcelDataUpload(HttpPostedFileBase upload)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (upload != null && upload.ContentLength > 0)
                    {
                        // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                        // to get started. This is how we avoid dependencies on ACE or Interop:
                        Stream stream = upload.InputStream;

                        // We return the interface, so that
                        IExcelDataReader reader = null;


                        if (upload.FileName.EndsWith(".xls"))
                        {
                            reader = ExcelReaderFactory.CreateBinaryReader(stream);
                        }
                        else if (upload.FileName.EndsWith(".xlsx"))
                        {
                            reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                        }
                        else
                        {
                            Danger("This file format is not supported");
                            return View("ExcelFileUpload");
                        }

                        reader.IsFirstRowAsColumnNames = true;
                        DataSet result = reader.AsDataSet();
                        reader.Close();
                        using (SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString))
                        {
                            dbConnection.Open();
                            //Deleting existing records for the batch
                            SqlCommand cmd = new SqlCommand("Truncate Table Temp_Client", dbConnection);
                            cmd.ExecuteNonQuery();
                            using (SqlBulkCopy s = new SqlBulkCopy(dbConnection))
                            {
                                s.DestinationTableName = "Temp_Client";
                                //s.ColumnMappings.Add((column.ToString(), column.ToString());
                                s.WriteToServer(result.Tables[0]);
                            }
                        }
                        DataTable dt = Toolkit.ReturnDatatable("Select * from Temp_Client where len(Ltrim(RTRIM(client_name)))>0 and len(Ltrim(RTRIM(client_type)))>0 and client_type in ('INDIVIDUAL','CORPORATE','SME')");
                        if (dt != null && dt.Rows.Count == result.Tables[0].Rows.Count)//If all rows are uploaded
                        {
                            return RedirectToAction("ListExcelLoadedData");
                        }
                        else
                        {
                            Danger("File not uploaded successfully. Please check if Client Name and Client Type values and try again");
                            return View("ExcelFileUpload");
                        }
                    }
                    else
                    {
                        Danger("Please Upload Your file");
                        return View("ExcelFileUpload");
                    }
                }
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult ListExcelLoadedData()
        {
            var uploadedData = new List<Temp_Client>();
            try
            {
                uploadedData = dc.Temp_Client.ToList();
                return View(uploadedData);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult ValidateExcelBatch()
        {
            try
            {
                string cmd = @"INSERT INTO [dbo].[Client]
           ([client_name]
           ,[client_type]
           ,[contact_person]
           ,[physical_address]
           ,[mobile]
           ,[mobile2]
           ,[email]
           ,[language]
           ,[recruited_by]
           ,[create_dt]
           ,[user_id]
           ,[reference_number]) 
           SELECT 
          [client_name]
          ,[client_type]
          ,[contact_person]
          ,[physical_address]
          ,[mobile]
          ,[mobile2]
          ,[email]
          ,[language]
          ,[recruited_by]
            ,getdate()
            ,'" + AppUserData.Login + @"'
          ,[reference_number]
      FROM [dbo].[Temp_Client] where reference_number   not in (select reference_number from client)";
                Toolkit.RunSQLCommand(cmd);
                return RedirectToAction("ListCustomer", "Customer");
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

    }
}
