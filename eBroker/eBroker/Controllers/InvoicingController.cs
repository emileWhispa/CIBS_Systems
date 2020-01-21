using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBroker.Controllers
{
    public class InvoicingController : BaseController
    {
        eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListInvoice(string startDate, string endDate)
        {
            this.InvoiceInfo(0);
            try
            {
                var invoices = new List<Invoice>();
                if (String.IsNullOrEmpty(startDate) || String.IsNullOrEmpty(endDate))
                    invoices = _dc.Invoice.OrderByDescending(x => x.Id).Take(30).ToList();
                else
                {
                    DateTime start = DateTime.Parse(startDate);
                    DateTime end = DateTime.Parse(endDate);
                    invoices = _dc.Invoice.Where(x => x.invoice_dt >= start && x.invoice_dt <= end).ToList();

                }
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View(invoices);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult InvoiceInfo(int id = 0)
        {
            try
            {
                var model = _dc.Invoice.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new Invoice();
                    model.Id = 0;
                    model.user_id = AppUserData.Login;
                    model.invoice_dt = DateTime.Now;
                    model.invoice_due_dt = DateTime.Today.AddDays(5);//5 Days by default
                    model.invoice_until_dt = DateTime.Today;
                    model.Status = "Pending";
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Customer Recruiter
                var insurers = (from r in _dc.Partner.Where(x=>x.partnership_type=="Insurance").ToList() select new SelectListItem { Text = r.company_short_name, Value = r.Id.ToString() }).ToList();
                ViewBag.Insurers = insurers;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateInvoice(Invoice inv)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    string message = "";
                    this._dc.Invoice.Add(inv);
                    if (inv.Id == 0)
                    {
                        inv.Status = "Pending";
                        inv.invoice_dt = DateTime.Now;
                        if (this._dc.SaveChanges() <= 0)
                            throw new Exception(message);
                        this.Success("Invoice Header Created Successfully", true);
                    }
                    else
                    {
                        this._dc.Entry<Invoice>(inv).State = EntityState.Modified;
                        this._dc.SaveChanges();
                    }
                    return (ActionResult)this.Content("1");
                }
                catch (Exception ex)
                {
                    this.Danger(ex.Message, true);
                }
            }
            this.InvoiceInfo(inv.Id);
            return (ActionResult)this.PartialView("InvoiceInfo", (object)inv);
        }
        public ActionResult InvoiceToExcel()
        {
            try
            {
                string fileName = "Invoices_" + DateTime.Today.ToShortDateString().Replace("/", "") + ".xls";
                string cmd = @"select p.company_name as [Company name],i.invoice_dt as [Invoice date.],i.invoice_until_dt as [Invoice until date] from Invoice i INNER JOIN Partner p ON p.company_code=i.insurer_id";
                Toolkit.ExportListUsingEPPlus(cmd, fileName);
                return RedirectToAction("ListInvoice");
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return RedirectToAction("ListInvoice");
        }

        public ActionResult TariffInfo(int id = 0)
        {
            var insurers = (from r in _dc.Partner.Where(x => x.partnership_type == "Insurance").ToList() select new SelectListItem { Text = r.company_short_name, Value = r.Id.ToString() }).ToList();
            ViewBag.Insurers = insurers;
            var products = (from r in _dc.Insurance_Product.ToList() select new SelectListItem { Text = r.product_name, Value = r.Id.ToString() }).ToList();
            ViewBag.Products = products;
            var el = _dc.Commission_Tariff.Where(e => e.Id == id).FirstOrDefault();

            if (el != null) return PartialView(el);

            return (ActionResult)this.PartialView();
        }

        public ActionResult Tariff()
        {
            this.TariffInfo(0);
            List<Commission_Tariff> commissionTariffList = new List<Commission_Tariff>();
            var list = _dc.Commission_Tariff.OrderByDescending<Commission_Tariff, int>(x => x.Id).ToList();
            return this.View(list);
        }
        public ActionResult InvoicePolicyList(int id)
        {
            try
            {
                var inv = _dc.Invoice.Where(x => x.Id == id).FirstOrDefault();
                ViewBag.InvoiceInfo = inv;
                var policyList = _dc.Invoice_Detail.Where(x => x.invoice_id == id).OrderBy(x => x.contract_id).ToList();
                if (inv.Status == "Paid")//
                {
                    return View(policyList);//To avoid attaching new policies to the existing invoice
                }
                if (policyList == null || policyList.Count() == 0)
                {
                    using (SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString))
                    {
                        dbConnection.Open();
                        //Deleting existing records for the batch
                        SqlCommand cmd = new SqlCommand("exec sp_create_InvoicePolicyList " + id, dbConnection);
                        cmd.ExecuteNonQuery();
                        //return RedirectToAction("MSAPaymentList", "MSA", new { BId = BId });
                    }
                    policyList = _dc.Invoice_Detail.Where(x => x.invoice_id == id).OrderBy(x => x.contract_id).ToList();
                }
                return View(policyList);

            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateTariff(Commission_Tariff inv)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    string message = "";
                    this._dc.Commission_Tariff.Add(inv);
                    if (inv.Id == 0)
                    {
                        if (this._dc.SaveChanges() <= 0)
                            throw new Exception(message);
                        this.Success("Tariff Header Created Successfully", true);
                    }
                    else
                    {
                        this._dc.Entry<Commission_Tariff>(inv).State = EntityState.Modified;
                        this._dc.SaveChanges();
                    }
                    return (ActionResult)this.Content("1");
                }
                catch (Exception ex)
                {
                    this.Danger(ex.Message, true);
                }
            }
            this.TariffInfo(inv.Id);
            return (ActionResult)this.PartialView("TariffInfo", (object)inv);
        }
        public ActionResult ConfirmInvoice(int id)
        {
            try
            {
                using (SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString))
                {
                    dbConnection.Open();
                    //Deleting existing records for the batch
                    SqlCommand cmd = new SqlCommand($"UPDATE Invoice set status='Paid' where invoice_id={id}", dbConnection);
                    cmd.ExecuteNonQuery();
                    return RedirectToAction("ListInvoice", "Invoicing");
                }
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return Content("1");
        }

        public ActionResult DeleteInvoiceDetails(int id)
        {
            try
            {
                int res = Toolkit.RunSQLCommand($"Delete from invoice_detail where invoice_id={id}");//Deleting the batch details
                if (res > 0)
                {
                    Success("Invoice Details deleted successfully", true);
                    return RedirectToAction("ListInvoice", "Invoicing");
                }
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return RedirectToAction("ListInvoice", "Invoicing");
        }

        public ActionResult PolicyExclusion(int cId, int id)
        {
            try
            {
                int res = Toolkit.RunSQLCommand($"Delete from invoice_detail where detail_invoice_id={cId}");//Deleting the batch details
                if (res > 0)
                {
                    Success("Policy excluded successfully", true);
                }
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return RedirectToAction("InvoicePolicyList", "Invoicing", new { Id = id });
        }

        public ActionResult DeleteInvoice(int id)
        {
            try
            {
                int res = Toolkit.RunSQLCommand($"Delete from invoice_detail where invoice_id={id}");//Deleting the batch details
                //if (res > 0)
                //{
                    int res1 = Toolkit.RunSQLCommand($"Delete from invoice where invoice_id={id}");//Deleting the batch
                    //if (res1 > 0)
                //}
                //else
                //    return View();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            
            return RedirectToAction("ListInvoice", "Invoicing");
        }

        public ActionResult ExportToExcel(int id)
        {
            try
            {
                string cmd = $@"SELECT ic.invoice_id as [Invoice No.]
                              ,[invoice_dt] as [Invoice Date]
                              ,[company_short_name] as [Insurer]
                              ,[client_name] as [Client Name]
                              ,[product_name] as [Product]
                              ,[policy_no] as [Policy No.]
                                ,[effective_dt] as [Effective Date]
                                ,[expiry_dt] as [Expiry Date]
                              ,[net_premium] as [Net Premium]
                              ,[commission_percentage] as [Commission %]
                              ,[commission_amt] as [Commission Amount]
                          FROM Invoice_Detail inv INNER JOIN Invoice ic ON ic.invoice_id=inv.invoice_id INNER JOIN Partner p ON p.company_code=ic.insurer_id INNER JOIN insurance_policy i ON i.contract_id=inv.contract_id INNER JOIN Client c ON c.client_id=i.client_id INNER JOIN Insurance_Product ip ON ip.product_id=i.product_id WHERE ic.invoice_id={id} order by [effective_dt]";
           string cmd2 = $@"SELECT [invoice_id] as [Invoice No.]
                              ,[invoice_dt] as [Invoice Date]
                              ,[company_short_name] as [Insurer]
                              ,[client_name] as [Client Name]
                              ,[product_name] as [Product]
                              ,[policy_no] as [Policy No.]
                                ,[effective_dt] as [Effective Date]
                                ,[expiry_dt] as [Expiry Date]
                              ,[net_premium] as [Net Premium]
                              ,[commission_percentage] as [Commission %]
                              ,[commission_amt] as [Commission Amount]
                          FROM [Vw_Invoice_Broker] where invoice_id={id} order by [effective_dt]";
                Toolkit.ExportListUsingEPPlus(cmd, "Invoice Details");
                return RedirectToAction("InvoicePolicyList", new { Id = id });
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return RedirectToAction("InvoicePolicyList", new { Id = id });
        }
    }
}
