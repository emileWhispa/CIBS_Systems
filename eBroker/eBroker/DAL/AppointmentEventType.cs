using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("AppointmentEventType")]
public class AppointmentEventType
{
[NotMapped]
public string TableName { get { return "AppointmentEventType"; } }
[Column("AppointmentEventTypeID")]
[Key]
public int Id{get;set;}
[Column("AppointmentEventType")]
[StringLength(50)]
public string AppointmentEventTypeName{get;set;}
}
}
