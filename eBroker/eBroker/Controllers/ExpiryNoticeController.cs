using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBroker.Controllers
{
    public class ExpiryNoticeController : BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ExpiryNoticeView(string startDate, string endDate)
        {
            try
            {
                var policies = new List<InsurancePolicy>();
                if (String.IsNullOrEmpty(startDate) || String.IsNullOrEmpty(endDate))
                    policies = dc.InsurancePolicy.Where(x => x.expiry_dt == DateTime.Today && x.renewable == true).ToList();
                else
                {
                    DateTime start = DateTime.Parse(startDate);
                    DateTime end = DateTime.Parse(endDate);
                    policies = dc.InsurancePolicy.Where(x => x.expiry_dt >= start && x.expiry_dt <= end && x.renewable == true).OrderBy(x=>x.expiry_dt).ToList();

                }
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                    return View(policies);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult SendSMSNotice(string startDate, string endDate)
        {
            try
            {
                var policies = new List<InsurancePolicy>();
                if (!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate))
                {
                    DateTime start = DateTime.Parse(startDate);
                    DateTime end = DateTime.Parse(endDate);
                    policies = dc.InsurancePolicy.Where(x => x.expiry_dt >= start && x.expiry_dt <= end && x.renewable == true).ToList();
                    if (policies.Count > 0)
                    {
                        string sms_account = ConfigurationManager.AppSettings["SMS_User"];
                        string sms_pin = ConfigurationManager.AppSettings["SMS_Password"];
                        string language = "Kinyarwanda";
                        string balance = "";
                        string status = "";
                        string msg = "";
                        string msgId = "";
                        SMSApi.ksms smsSender = new SMSApi.ksms();
                        for (int i = 0; i < policies.Count; i++)
                        {
                            try
                            {
                                string phone = policies[i].Clients.mobile;
                                if (phone.Length == 10)
                                    phone = "25" + phone;
                                else if (phone.StartsWith("+") && phone.Length == 13)
                                    phone = phone.Replace("+", "");
                                else if (phone.Length != 12)
                                    continue;
                                language = policies[i].Clients.language;
                                msgId = DateTime.Now.ToFileTime().ToString();
                                if (language == "Kinyarwanda")
                                {
                                    msg = policies[i].Clients.client_name + ", CIBS irabamenyesha ko ubwishingizi No. " + policies[i].policy_no + " mufite muri " + policies[i].Partners.company_short_name + " buzarangira le " + policies[i].expiry_dt.ToString("yyyy-MM-dd") + ". Mwahamagara " + ConfigurationManager.AppSettings["Contact_Phone"];
                                }
                                else if (language == "English")
                                {
                                    msg = "Dear " + policies[i].Clients.client_name + ", CIBS would like to inform you that Insurance Policy No. " + policies[i].policy_no + " held in " + policies[i].Partners.company_short_name + " will expire on " + policies[i].expiry_dt.ToString("yyyy-MM-dd") + ". Contact " + ConfigurationManager.AppSettings["Contact_Phone"];
                                }
                                else if (language == "French")
                                {
                                    msg = policies[i].Clients.client_name + ", CIBS voudrait vous informer que votre assurance No. " + policies[i].policy_no + " de " + policies[i].Partners.company_short_name + " expire le " + policies[i].expiry_dt.ToString("yyyy-MM-dd") + ". Contact " + ConfigurationManager.AppSettings["Contact_Phone"];
                                }
                                smsSender.ksend(sms_account, sms_pin, "CIBS", ref msg, phone, ref msgId, "", "", "", out balance, out status);
                                Toolkit.RunSQLCommand("Insert into SMS_Log (client_name,phone, policy_no, insurer,expiry_date, status, balance,user_id) values ('" + policies[i].Clients.client_name.Replace("'", "''") + "','" + phone + "','" + policies[i].policy_no + "','" +
                                  policies[i].Partners.company_short_name + "','" + policies[i].expiry_dt + "','" + balance + "','" + status + "','"+ AppUserData.Login+"')");
                            }
                            catch (Exception ex)
                            {
                                Danger(ex.Message, true);
                            }
                        }
                        Success("SMS sent to customers", true);
                    }
                }
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return RedirectToAction("ExpiryNoticeView", new { startDate = startDate, endDate = endDate });
        }

        public ActionResult ExportToExcel(string startDate, string endDate)
        {
            try
            {
                string fileName = "SMSNotification_" + DateTime.Today.ToShortDateString().Replace("/", "") + ".xls";
                string cmd = @"select policy_no as [POLICY NO.], policy_type AS [TYPE], convert(varchar(25),effective_dt,105) AS [START DATE], 
                    convert(varchar(25),expiry_dt,105) as [EXPIRY DATE],net_premium AS [NET PREMIUM], product_name AS [INSURANCE TYPE],
                    INSURER, client_name as CLIENT, MOBILE
                    from vw_policy_report where renewable =1 and expiry_dt between '" + startDate + "' and '" + endDate + "' order by expiry_dt";
                Toolkit.ExportListUsingEPPlus(cmd, fileName);
                return RedirectToAction("ExpiryNoticeView", new { startDate = startDate, endDate = endDate });
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return RedirectToAction("ExpiryNoticeView", new { startDate = startDate, endDate = endDate });
        }

        public ActionResult SMSView(string startDate, string endDate)
        {
            try
            {
                var sms = new List<SMS_Log>();
                if (String.IsNullOrEmpty(startDate) || String.IsNullOrEmpty(endDate))
                    sms = dc.SMS_Log.Where(x => x.system_date== DateTime.Today).ToList();
                else
                {
                    DateTime start = DateTime.Parse(startDate);
                    DateTime end = DateTime.Parse(endDate);
                    sms = dc.SMS_Log.Where(x => x.system_date >= start && x.system_date <= end).ToList();

                }
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View(sms);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult SMSToExcel(string startDate, string endDate)
        {
            try
            {
                string fileName = "SMSNotification_" + DateTime.Today.ToShortDateString().Replace("/", "") + ".xls";
                string cmd = @"select client_name as [Client Name],phone as [Mobile], policy_no as [Policy No.], insurer as [Insurer],convert(varchar(25),expiry_date,106) as [Expiry Date], status as Status, balance as Balance,user_id as [User],convert(varchar(50),system_date) as [System Date] from SMS_Log where system_date between '" + (startDate ==null?"2000-01-01":startDate) + "' and '" + (endDate==null?"2050-12-31":endDate) + "'";
                Toolkit.ExportListUsingEPPlus(cmd, fileName);
                return View();
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

    }
}
