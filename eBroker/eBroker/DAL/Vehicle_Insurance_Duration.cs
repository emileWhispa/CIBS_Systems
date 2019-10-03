using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Vehicle_Insurance_Duration")]
public class Vehicle_Insurance_Duration
{
[NotMapped]
public string TableName { get { return "Vehicle_Insurance_Duration"; } }
[Column("duration_id")]
[Required()]
[Key]
public int Id{get;set;}
[Column("duration_rate")]
public float duration_rate{get;set;}
[Column("duration_name")]
[StringLength(50)]
public string duration_name{get;set;}
}
}
