using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Vehicle_Guarantee")]
public class Vehicle_Guarantee
{
[NotMapped]
public string TableName { get { return "Vehicle_Guarantee"; } }
[Column("guarantee_id")]
[Key]
public int Id{get;set;}
[Column("guarantee_name")]
[StringLength(50)]
public string guarantee_name{get;set;}
}
}
