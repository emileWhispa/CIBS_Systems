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
        private readonly eBroker.BrokerDataContext _dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);

        public ActionResult ListAppointment(string startDate, string endDate)
        {
            this.AppointmentInfo(0);
            try
            {
                var appointments = new List<Appointment>();
                if (String.IsNullOrEmpty(startDate) || String.IsNullOrEmpty(endDate))
                    appointments = _dc.Appointment.Where(x=>x.Status!="Closed").OrderByDescending(x=>x.Id).Take(20).ToList();
                else
                {
                    DateTime start = DateTime.Parse(startDate);
                    DateTime end = DateTime.Parse(endDate);
                    appointments = _dc.Appointment.Where(x => x.AppointmentDate >= start && x.AppointmentDate <= end).ToList();

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

        public ActionResult ListEvent(int id)
        {
            this.EventInfo(id, 0);
            try
            {
                var appointmentEvents = new List<AppointmentDetail>();
                appointmentEvents = _dc.AppointmentDetails.Where(x => x.AppointmentID == id).ToList();
                var appointment = _dc.Appointment.Where(x => x.Id == id).FirstOrDefault();
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
                    this._dc.Appointment.Add(app);
                    if (app.Id == 0)
                    {
                        app.BookedOn = DateTime.Now;
                        if (this._dc.SaveChanges() <= 0)
                            throw new Exception(message);
                        this.Success("Appointment Saved Successfully", true);
                    }
                    else
                    {
                        this._dc.Entry<Appointment>(app).State = EntityState.Modified;
                        this._dc.SaveChanges();
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
                    this._dc.AppointmentDetails.Add(app);
                    if (app.Id == 0)
                    {
                        if (this._dc.SaveChanges() <= 0)
                            throw new Exception(message);
                        this.Success("Event Created Successfully", true);
                    }
                    else
                    {
                        this._dc.Entry<AppointmentDetail>(app).State = EntityState.Modified;
                        this._dc.SaveChanges();
                    }
                    Appointment entity = this._dc.Appointment.Where(x => x.Id == app.AppointmentID).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.Status = app.AppointmentStatus;
                        this._dc.Appointment.Add(entity);
                        this._dc.Entry(entity).State = EntityState.Modified;
                        this._dc.SaveChanges();
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
        public ActionResult AppointmentInfo(int id = 0)
        {
            try
            {
                var model = _dc.Appointment.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new Appointment();
                    model.Id = 0;
                    model.BookedBy = AppUserData.Login;
                    model.AppointmentDate = DateTime.Today.AddDays(1);
                    model.BookedOn = DateTime.Now;
                    model.Status = "Pending";
                    //Model.recruited_by = AppUserData.Login;
                }
                //Reading Customer Recruiter
                var appointmentType = (from r in _dc.AppointmentType.ToList() select new SelectListItem { Text = r.AppointmentTypeName, Value = r.Id.ToString() }).ToList();
                ViewBag.AppointmentTypes = appointmentType;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                Danger(ex.Message, true);
            }
            return PartialView();
        }

        [HttpGet]
        public ActionResult EventInfo(int appId,int id = 0)
        {
            try
            {
                var model = _dc.AppointmentDetails.Where(x => x.Id == id).FirstOrDefault();
                if (model == null)
                {
                    model = new AppointmentDetail();
                    model.Id = 0;
                    model.AppointmentID = appId;
                    model.DoneBy = AppUserData.Login;
                    model.EventDate = DateTime.Today;
                }
                var eventTypes = (from r in _dc.AppointmentEventType.ToList() select new SelectListItem { Text = r.AppointmentEventTypeName, Value = r.Id.ToString() }).ToList();
                ViewBag.EventTypes = eventTypes;
                var eventStatus = (from r in _dc.AppointmentStatus.ToList() select new SelectListItem { Text = r.Status, Value = r.Status }).ToList();
                ViewBag.EventStatus = eventStatus;
                var employees = (from r in _dc.eUser.Where(x=>x.eUserCategories.Category!="Bank" || x.eUserCategories.Category!="Insurer").ToList() select new SelectListItem { Text = r.Names, Value = r.Names }).ToList();
                ViewBag.Employees = employees;
                return PartialView(model);
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
