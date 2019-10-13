using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;

namespace eBroker.Controllers
{
    public class AppointmentController : BaseController
    {
        eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListAppointment(string startDate, string endDate)
        {
            this.AppointmentInfo(0);
            try
            {
                var appointments = new List<Appointment>();
                if (String.IsNullOrEmpty(startDate) || String.IsNullOrEmpty(endDate))
                    appointments = dc.Appointment.Where(x=>x.Status!="Closed").OrderByDescending(x=>x.Id).Take(20).ToList();
                else
                {
                    DateTime start = DateTime.Parse(startDate);
                    DateTime end = DateTime.Parse(endDate);
                    appointments = dc.Appointment.Where(x => x.AppointmentDate >= start && x.AppointmentDate <= end).ToList();

                }
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                return View(appointments);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }

        public ActionResult ListEvent(int Id)
        {
            this.EventInfo(Id, 0);
            try
            {
                var appointmentEvents = new List<AppointmentDetail>();
                appointmentEvents = dc.AppointmentDetails.Where(x => x.AppointmentID == Id).ToList();
                var appointment = dc.Appointment.Where(x => x.Id == Id).FirstOrDefault();
                ViewBag.AppointmentInfo = appointment;
                return View(appointmentEvents);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateAppointment(Appointment app)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    string message = "";
                    this.dc.Appointment.Add(app);
                    if (app.Id == 0)
                    {
                        app.BookedOn = DateTime.Now;
                        if (this.dc.SaveChanges() <= 0)
                            throw new Exception(message);
                        this.Success("Appointment Saved Successfully", true);
                    }
                    else
                    {
                        this.dc.Entry<Appointment>(app).State = EntityState.Modified;
                        this.dc.SaveChanges();
                    }
                    return (ActionResult)this.Content("1");
                }
                catch (Exception ex)
                {
                    this.Danger(ex.Message, true);
                }
            }
            this.AppointmentInfo(app.Id);
            return (ActionResult)this.PartialView("AppointmentInfo", (object)app);
        }

        [HttpPost]
        public ActionResult CreateEvent(AppointmentDetail app)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    string message = "";
                    app.EventDate = DateTime.Now;
                    this.dc.AppointmentDetails.Add(app);
                    if (app.Id == 0)
                    {
                        if (this.dc.SaveChanges() <= 0)
                            throw new Exception(message);
                        this.Success("Event Created Successfully", true);
                    }
                    else
                    {
                        this.dc.Entry<AppointmentDetail>(app).State = EntityState.Modified;
                        this.dc.SaveChanges();
                    }
                    Appointment entity = this.dc.Appointment.Where(x => x.Id == app.AppointmentID).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.Status = app.AppointmentStatus;
                        this.dc.Appointment.Add(entity);
                        this.dc.Entry(entity).State = EntityState.Modified;
                        this.dc.SaveChanges();
                    }
                    return this.Content("1");
                }
                catch (Exception ex)
                {
                    this.Danger(ex.Message, true);
                }
            }
            this.EventInfo(app.AppointmentID, app.Id);
            return (ActionResult)this.PartialView("EventInfo", (object)app);
        }
        [HttpGet]
        public ActionResult AppointmentInfo(int Id = 0)
        {
            try
            {
                var Model = dc.Appointment.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new Appointment();
                    Model.Id = 0;
                    Model.BookedBy = AppUserData.Login;
                    Model.AppointmentDate = DateTime.Today.AddDays(1);
                    Model.BookedOn = DateTime.Now;
                    Model.Status = "Pending";
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Customer Recruiter
                var appointmentType = (from r in dc.AppointmentType.ToList() select new SelectListItem { Text = r.AppointmentTypeName, Value = r.Id.ToString() }).ToList();
                ViewBag.AppointmentTypes = appointmentType;
                return PartialView(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult EventInfo(int AppId,int Id = 0)
        {
            try
            {
                var Model = dc.AppointmentDetails.Where(x => x.Id == Id).FirstOrDefault();
                if (Model == null)
                {
                    Model = new AppointmentDetail();
                    Model.Id = 0;
                    Model.AppointmentID = AppId;
                    Model.DoneBy = AppUserData.Login;
                    Model.EventDate = DateTime.Today;
                }
                var eventTypes = (from r in dc.AppointmentEventType.ToList() select new SelectListItem { Text = r.AppointmentEventTypeName, Value = r.Id.ToString() }).ToList();
                ViewBag.EventTypes = eventTypes;
                var eventStatus = (from r in dc.AppointmentStatus.ToList() select new SelectListItem { Text = r.Status, Value = r.Status }).ToList();
                ViewBag.EventStatus = eventStatus;
                var employees = (from r in dc.eUser.Where(x=>x.eUserCategories.Category!="Bank" || x.eUserCategories.Category!="Insurer").ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();
                ViewBag.Employees = employees;
                return PartialView(Model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        public ActionResult AppointmentToExcel(string startDate, string endDate)
        {
            try
            {
                string fileName = "Appointments_" + DateTime.Today.ToShortDateString().Replace("/", "") + ".xls";
                string cmd = @"SELECT 
                          convert(varchar(25),[AppointmentDate],106) as [Date]
                          ,[AppointmentTime] as [Time]
                          ,[AppointmentVenue]
                          ,[ClientName] as [Client Name]
                          ,[ClientPhone] as [Client Phone]
                          ,[ClientEmail] as [E-mail]
                          ,case [SendReminderToClient] when 1 then 'True' else 'False' end [Reminder]
                          ,[AppointmentType] as [Type]
                          ,[Comments]
                          ,[Status]
                          ,convert(varchar(25),[BookedOn]) as [Booked On]
                          ,[BookedBy] as [Booked By]
                          ,[Language]
                          FROM [eBrokerage].[dbo].[Appointment] a, [eBrokerage].[dbo].[AppointmentType] b
                          where a.AppointmentTypeId=b.AppointmentTypeId ";
                if (startDate != null && startDate.Length > 10)
                    cmd = cmd + " and AppointmentDate > = '" + startDate + "'";
                if (endDate != null && endDate.Length > 10)
                    cmd = cmd + " and AppointmentDate < = '" + endDate + "'";
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
