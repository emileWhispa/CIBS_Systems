using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Appointment")]
    public class Appointment
    {
        [NotMapped]
        public string TableName { get { return "Appointment"; } }
        [Column("AppointmentID")]
        [Key]
        public int Id { get; set; }
        [Column("AppointmentDate")]
        [Display(Name = "Appointment Date")]
        [Required]
        public DateTime AppointmentDate { get; set; }
        [Column("AppointmentTime")]
        [Display(Name = "Appointment Time")]
        [StringLength(20)]
        [Required]
        public string AppointmentTime { get; set; }
        [Column("AppointmentVenue")]
        [Display(Name = "Appointment Venue")]
        [StringLength(20)]
        [Required]
        public string AppointmentVenue { get; set; }
        [Column("ClientName")]
        [Display(Name = "Client Name")]
        [StringLength(50)]
        [Required]
        public string ClientName { get; set; }
        [Column("ClientPhone")]
        [Display(Name = "Client Phone")]
        [StringLength(50)]
        [Required]
        public string ClientPhone { get; set; }
        [Column("ClientEmail")]
        [Display(Name = "Client E-mail")]
        [StringLength(100)]
        public string ClientEmail { get; set; }
        [Column("SendReminderToClient")]
        [Display(Name = "Send Reminder to Client")]
        public bool SendReminderToClient { get; set; }
        [Column("AppointmentTypeID")]
        [Display(Name = "Appointment Type")]
        [Required]
        public int AppointmentTypeID { get; set; }
        [Column("Comments")]
        [StringLength(2000)]
        public string Comments { get; set; }
        [Column("Status")]
        [StringLength(20)]
        public string Status { get; set; }
        [Column("BookedOn")]
        public DateTime BookedOn { get; set; }
        [Column("BookedBy")]
        [StringLength(30)]
        public string BookedBy { get; set; }
        [Column("Language")]
        [StringLength(20)]
        [Required]
        public string Language { get; set; }
        [ForeignKey("AppointmentTypeID")]
        public virtual AppointmentType appointmentType_ { get; set; }
    }
}
