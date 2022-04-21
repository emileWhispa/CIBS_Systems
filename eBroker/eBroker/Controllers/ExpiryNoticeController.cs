using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using eBroker.DAL;
using Newtonsoft.Json;

namespace eBroker.Controllers
{
    public class ExpiryNoticeController : BaseController
    {
        
        private static readonly HttpClient client = new HttpClient();
        eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult CustomSMSView()
        {
            var people = _dc.Client.Where(x => x.mobile != null && x.mobile.Trim().Length > 0).ToList();
            return View(people);
        }

        [HttpPost]
        public async Task<ActionResult> SendMultipleSMS(IEnumerable<string> clients,string content)
        {
            string resp = "response";
            if (clients != null && content != null)
            {
                var phones = new List<string>();
                var names = new List<string>();
                foreach (var px in clients)
                {
                    var num = int.Parse(px);
                    var user = _dc.Client.FirstOrDefault(x=>x.Id == num);
                    
                    if(user == null || string.IsNullOrEmpty(user.mobile) ) continue;
                    
                    var phone = user.mobile;
                    if (phone.Length == 10)
                        phone = "25" + phone;
                    else if (phone.StartsWith("+") && phone.Length == 13)
                        phone = phone.Replace("+", "");
                    else if (phone.Length != 12)
                        continue;   
                    
                    phones.Add(phone);
                    names.Add(user.client_name??"---");
                    
                }
                
                var values = new Dictionary<string, string>
                {
                    { "token", "Q5G2NFOvi0FAPTXVWwFitg2VFCKIdZFr" },
                    { "phone", string.Join(",",phones) },
                    { "message", content },
                    { "sender_name", "CIBS" },
                };

                var data = new FormUrlEncodedContent(values);
                //
                var response = await client.PostAsync("http://sms.besoft.rw/api/v1/client/bulksms", data);
                                
                resp = await response.Content.ReadAsStringAsync();

                var log = JsonConvert.DeserializeObject<SmsResponse>(resp);

                if (log != null && log.statusCode == 201)
                {
                    resp = log.response;
                    Success(resp);
                    var index = 0;
                    foreach (var item in phones)
                    {
                
                        // smsSender.ksend(smsAccount, smsPin, "CIBS", ref msg, phone, ref msgId, "", "", "", out balance, out status);
                        Toolkit.RunSQLCommand("Insert into SMS_Log (client_name,phone, policy_no, insurer,expiry_date, status, balance,user_id,content) values ('" + names[index] + "','" + item + "','---','---','"+DateTime.Now.ToString("yyyy-MM-dd")+"','','','"+ AppUserData.Login+"','"+content.Replace("'", "''")+"')");
                        index++;
                    }
                }
                else
                {
                    resp = "SMS Failed To Send";
                    Danger(resp);
                }
                

            }
            return RedirectToAction("CustomSMSView");
        }

        public ActionResult ExpiryNoticeView(string startDate, string endDate)
        {
            try
            {
                var policies = new List<InsurancePolicy>();
                if (String.IsNullOrEmpty(startDate) || String.IsNullOrEmpty(endDate)){
                    startDate = DateTime.Now.ToString("yyyy-MM-dd");
                    endDate = DateTime.Now.ToString("yyyy-MM-dd");
                    policies = _dc.InsurancePolicy.Where(x => x.expiry_dt == DateTime.Today && x.renewable == true && x.Clients != null).ToList();
                }else
                {
                    DateTime start = DateTime.Parse(startDate);
                    DateTime end = DateTime.Parse(endDate);
                    policies = _dc.InsurancePolicy.Where(x => x.expiry_dt >= start && x.expiry_dt <= end && x.renewable == true && x.Clients != null).OrderBy(x=>x.expiry_dt).ToList();

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

        public async Task<ActionResult> SendSmsNotice(string startDate, string endDate)
        {
            // try
            // {
                var policies = new List<InsurancePolicy>();
                var responseString = "SMS sent to customers";
                if (!String.IsNullOrEmpty(startDate) && !String.IsNullOrEmpty(endDate))
                {
                    DateTime start = DateTime.Parse(startDate);
                    DateTime end = DateTime.Parse(endDate);
                    policies = _dc.InsurancePolicy.Where(x => x.expiry_dt >= start && x.expiry_dt <= end && x.renewable == true).ToList();
                    if (policies.Count > 0)
                    {
                        string smsAccount = ConfigurationManager.AppSettings["SMS_User"];
                        string smsPin = ConfigurationManager.AppSettings["SMS_Password"];
                        string language = "Kinyarwanda";
                        string balance = "";
                        string status = "";
                        string msg = "";
                        string msgId = "";
                        //SMSApi.ksms smsSender = new SMSApi.ksms();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                       // Console.Out.WriteLine(smsSender.kchk(smsAccount,smsPin,out balance,out status));
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        Console.Out.WriteLine();
                        for (int i = 0; i < policies.Count; i++)
                        {
                             try
                             {
                                string phone = policies[i].ClientOptionalMobile;

                                if (string.IsNullOrEmpty(phone))
                                {
                                    Danger("No Phone number");
                                    continue;
                                }

                                if (phone.Length == 10)
                                    phone = "25" + phone;
                                else if (phone.StartsWith("+") && phone.Length == 13)
                                    phone = phone.Replace("+", "");
                                else if (phone.Length != 12)
                                {
                                    Danger("Invalid Phone Number `"+phone+"`");
                                    continue;
                                }
                                language = policies[i].Clients != null ? policies[i].Clients.language : "Kinyarwanda";
                                msgId = DateTime.Now.ToFileTime().ToString();
                                if (language == "Kinyarwanda")
                                {
                                    msg = policies[i].ClientOptionalName + ", CIBS irabamenyesha ko ubwishingizi No. " + policies[i].policy_no + " mufite muri " + policies[i].Partners.company_short_name + " buzarangira le " + policies[i].expiry_dt.ToString("yyyy-MM-dd") + ". Mwahamagara " + ConfigurationManager.AppSettings["Contact_Phone"];
                                }
                                else if (language == "English")
                                {
                                    msg = "Dear " + policies[i].ClientOptionalName + ", CIBS would like to inform you that Insurance Policy No. " + policies[i].policy_no + " held in " + policies[i].PartnerOptionalName + " will expire on " + policies[i].expiry_dt.ToString("yyyy-MM-dd") + ". Contact " + ConfigurationManager.AppSettings["Contact_Phone"];
                                }
                                else if (language == "French")
                                {
                                    msg = policies[i].ClientOptionalName + ", CIBS voudrait vous informer que votre assurance No. " + policies[i].policy_no + " de " + policies[i].PartnerOptionalName + " expire le " + policies[i].expiry_dt.ToString("yyyy-MM-dd") + ". Contact " + ConfigurationManager.AppSettings["Contact_Phone"];
                                }
                                var values = new Dictionary<string, string>
                                {
                                    { "token", "Q5G2NFOvi0FAPTXVWwFitg2VFCKIdZFr" },
                                    { "phone", phone },
                                    { "message", msg },
                                    { "sender_name", "CIBS" },
                                };

                                var content = new FormUrlEncodedContent(values);
                                //
                                var response = await client.PostAsync("http://sms.besoft.rw/api/v1/client/bulksms", content);
                                
                                var resp = await response.Content.ReadAsStringAsync();

                                var log = JsonConvert.DeserializeObject<SmsResponse>(resp);

                                if (log != null && log.statusCode >= 200 && log.statusCode <=299)
                                {
                                    Success(log.response);
                                    // smsSender.ksend(smsAccount, smsPin, "CIBS", ref msg, phone, ref msgId, "", "", "", out balance, out status);
                                    Toolkit.RunSQLCommand(
                                        "Insert into SMS_Log (client_name,phone, policy_no, insurer,expiry_date, status, balance,user_id,content) values ('" +
                                        policies[i].ClientOptionalName.Replace("'", "''") + "','" + phone + "','" +
                                        policies[i].policy_no + "','" +
                                        policies[i].PartnerOptionalName + "','" + policies[i].expiry_dt + "','" +
                                        balance + "','" + status + "','" + AppUserData.Login + "','"+msg.Replace("'", "''")+"')");
                                }
                                else
                                {
                                    Danger((log != null ?log.response:"")+" \n SMS Failed To Send To "+policies[i].ClientOptionalName);
                                }
                                 }
                             catch (Exception ex)
                             {
                                 Danger(ex.Message, true);
                             }
                        }
                    }
                }
            // }
            // catch (Exception ex)
            // {
            //     Danger(ex.Message, true);
            // }
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

        public ActionResult SmsView(string startDate, string endDate)
        {
            try
            {
                var sms = new List<SMS_Log>();
                if (String.IsNullOrEmpty(startDate) || String.IsNullOrEmpty(endDate))
                    sms = _dc.SMS_Log.Where(x => x.system_date== DateTime.Today).ToList();
                else
                {
                    DateTime start = DateTime.Parse(startDate);
                    DateTime end = DateTime.Parse(endDate);
                    end = end.Add(TimeSpan.FromDays(1));
                    sms = _dc.SMS_Log.Where(x => x.system_date >= start && x.system_date <= end).ToList();

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

        public ActionResult SmsToExcel(string startDate, string endDate)
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
