using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Vehicle_Tariff_Base")]
public class Vehicle_Tariff_Base
{
[NotMapped]
public string TableName { get { return "Vehicle_Tariff_Base"; } }
[Column("tariff_id")]
[Key]
public int Id{get;set;}
[Column("class_id")]
public int class_id{get;set;}
[Column("usage_id")]
public int usage_id{get;set;}
[Column("guarantee_id")]
public int guarantee_id{get;set;}
[Column("rate")]
public float rate{get;set;}
}
}
