using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("AppointmentStatus")]
public class AppointmentStatu
{
[NotMapped]
public string TableName { get { return "AppointmentStatus"; } }
[Column("StatusID")]
[Key]
public int Id{get;set;}
[Column("Status")]
[StringLength(30)]
public string Status{get;set;}
}
}
