using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Vehicle_Territorial_Limit")]
public class Vehicle_Territorial_Limit
{
[NotMapped]
public string TableName { get { return "Vehicle_Territorial_Limit"; } }
[Column("territorial_code")]
[Key]
public int Id{get;set;}
[Column("territorial_limit")]
[StringLength(50)]
public string territorial_limit{get;set;}
[Column("extension_rate")]
public double extension_rate{get;set;}
}
}
