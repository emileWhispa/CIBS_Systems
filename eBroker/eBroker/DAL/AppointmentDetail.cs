using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("AppointmentDetails")]
    public class AppointmentDetail
    {
        [NotMapped]
        public string TableName { get { return "AppointmentDetails"; } }
        [Column("AppointmentDetailsID")]
        [Key]
        public int Id { get; set; }
        [Column("AppointmentID")]
        public int AppointmentID { get; set; }
        [ForeignKey("AppointmentID")]
        public virtual Appointment Appointments { get; set; }
        [Column("EventTypeID")]
        [Display(Name = "Event Type")]
        [Required]
        public int EventTypeID { get; set; }
        [ForeignKey("EventTypeID")]
        public virtual AppointmentEventType AppointmentEventTypes { get; set; }
        [Column("Comments")]
        [StringLength(2000)]
        public string Comments { get; set; }
        [Column("EventDate")]
        [Display(Name = "Event Date")]
        [Required]
        public DateTime EventDate { get; set; }
        [Column("AssignedTo")]
        [Display(Name = "Assigned To")]
        [StringLength(30)]
        public string AssignedTo { get; set; }
        [Column("DoneBy")]
        [StringLength(30)]
        public string DoneBy { get; set; }
        [Column("AppointmentStatus")]
        [Display(Name = "Appointment Status")]
        [StringLength(20)]
        [Required]
        public string AppointmentStatus { get; set; }
    }
}
