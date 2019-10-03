using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("AppointmentType")]
    public class AppointmentType
    {
        [NotMapped]
        public string TableName { get { return "AppointmentType"; } }
        [Column("AppointmentTypeID")]
        [Key]
        public int Id { get; set; }
        [Column("AppointmentType")]
        [StringLength(50)]
        [Required]
        public string AppointmentTypeName { get; set; }
    }
}
