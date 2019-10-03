using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;
using AutoGenPassword;
using securityComponents;
using Hangfire;
using Forex.Services;

namespace eBroker.Controllers
{
    public class ManagementController : BaseController
    {
        //
        // GET: /Management/
        [HttpGet]
        public ActionResult ListUser(string query)
        {
            eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
            var users = new List<eUser>();
            this.UserInfo(0);
            try
            {
                if (string.IsNullOrEmpty(query))
                    users = dc.eUser.OrderBy(x => x.Names).ToList();
                else
                    users = dc.eUser.Where(x => x.Names.Contains(query) || x.Login.Contains(query)).OrderBy(x => x.Login).ToList();
                return View(users);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        [HttpGet]
        public ActionResult ChangePass()
        {
            BrokerDataContext dc = new BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
            var userCred = new Models.LocalPasswordModel();
            try
            {
               var user = dc.eUser.Where(x => x.Login == AppUserData.Login).FirstOrDefault();
               userCred.EncryptedPassword = securityComponents.Cryptography.Decrypt(user.Password);
               userCred.Login = user.Login;
               userCred.Id = user.Id;
                return View(userCred);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(Models.LocalPasswordModel usr)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    eBroker.BrokerDataContext dc = new BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
                    var chgUser = dc.eUser.Where(y => y.Id == usr.Id).FirstOrDefault();
                    chgUser.Password = Cryptography.Encrypt(usr.NewPassword);
                    dc.eUser.Add(chgUser);
                    dc.Entry(chgUser).State = EntityState.Modified;
                    int res = dc.SaveChanges();
                    if (res > 0)
                        Success("Password Changed Successfully", false);
                    else
                        throw new Exception("Unable to change the password");
                }
                else
                    throw new Exception("Unable to change the password");
                return RedirectToAction("ListUser");
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }


        [HttpGet]
        public ActionResult ResetPassword(int Id)
        {
            eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
            var user = new eUser();
            try
            {
                user = dc.eUser.Where(x => x.Id == Id).FirstOrDefault();
                return PartialView(user);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult GeneratePassword(eUser usr)
        {
            try
            {
                string Resp = "";
                eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
                var resetUser = dc.eUser.Where(y => y.Id == usr.Id).FirstOrDefault();
                AutoGenPassword.PasswordGenerator x = new PasswordGenerator();
                x.Minimum = 8;
                x.Maximum = 8;
                x.ExcludeSymbols = true;
                x.ConsecutiveCharacters = false;
                x.RepeatCharacters = false;
                resetUser.Password = securityComponents.Cryptography.Encrypt(x.Generate());
                resetUser.ChangePassword = true;
                resetUser.PwdChangeDate = DateTime.Today.AddMonths(3);
                dc.eUser.Add(resetUser);
                dc.Entry(resetUser).State = EntityState.Modified;
                int res = dc.SaveChanges();
                if (res > 0)
                    Success("Password Reset Successfully", true);
                else
                    throw new Exception(Resp);

                return Content("1");
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        //
        // GET: /Management/Details/5

        public ActionResult UserInfo(int Id = 0)
        {
            try
            {
                eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
                var Model = dc.eUser.Where(x => x.Id == Id).Take(20).FirstOrDefault();
                if (Model == null)
                {
                    PasswordGenerator x = new PasswordGenerator();
                    x.Minimum = 8;
                    x.Maximum = 8;
                    x.ExcludeSymbols = true;
                    x.ConsecutiveCharacters = false;
                    x.RepeatCharacters = false;
                    Model = new eUser();
                    Model.Id = 0;
                    Model.CreatedOn = DateTime.Today;
                    Model.CreatedBy = AppUserData.Login;
                    Model.ChangePassword = true;
                    Model.Attempts = 0;
                    Model.Active = true;
                    Model.Locked = false;
                    Model.PwdChangeDate = DateTime.Today.AddMonths(3);
                    Model.LastLogin = DateTime.Now;
                    Model.Password = Cryptography.Encrypt(x.Generate());
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Customer Recruiter
                var profile = (from r in dc.eUserCategory.ToList() select new SelectListItem { Text = r.Category, Value = r.Id.ToString() }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                ViewBag.profile = profile;
                var company = (from r in dc.Partner.Where(x => x.partnership_type != "Agent").ToList() select new SelectListItem { Text = r.company_short_name, Value = r.Id.ToString() }).ToList();//.Union((from a in dc.Partner.Where(x => x.partnership_type == "Agent").ToList() select new SelectListItem { Text = a.company_name, Value = a.company_name }).ToList());
                ViewBag.company = company;
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
        // GET: /Management/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Management/Create

        [HttpPost]
        public ActionResult CreateUser(eUser usr)
        {
            BrokerDataContext dc = new BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
            if (this.ModelState.IsValid)
            {
                try
                {
                    string message = "";
                    dc.eUser.Add(usr);
                    usr.Category = "";
                    if (usr.Id == 0)
                    {
                        PasswordGenerator passwordGenerator = new PasswordGenerator();
                        passwordGenerator.Minimum = 8;
                        passwordGenerator.Maximum = 8;
                        passwordGenerator.ExcludeSymbols = true;
                        passwordGenerator.ConsecutiveCharacters = false;
                        passwordGenerator.RepeatCharacters = false;
                        usr.CreatedOn = DateTime.Today;
                        usr.CreatedBy = this.AppUserData.Login;
                        usr.ChangePassword = true;
                        usr.Attempts = 0;
                        usr.Active = true;
                        usr.Locked = false;
                        usr.PwdChangeDate = DateTime.Today.AddMonths(3);
                        usr.LastLogin = DateTime.Now;
                        string gen = passwordGenerator.Generate();
                        usr.Password = Cryptography.Encrypt(gen);
                        BackgroundJob.Enqueue(() => EmailSenderService.SendEmail("User " + usr.Login + " Created Successfully with Default Password: <h4>" + gen + "</h4><br/> Kindly log to the system and change the password", usr.Email, "CBIS User Creation"));
                        if (dc.SaveChanges() <= 0)
                            throw new Exception(message);
                        this.Success("Record Saved Successfully", true);
                    }
                    else
                    {
                        dc.Entry<eUser>(usr).State = EntityState.Modified;
                        dc.SaveChanges();
                    }
                    return Content("1");
                }
                catch (Exception ex)
                {
                    Danger(ex.Message, true);
                }
            }
            this.UserInfo(usr.Id);
            return PartialView("UserInfo", usr);
        }
    }
}
