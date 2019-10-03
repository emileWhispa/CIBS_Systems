using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Vehicle_Age_Range")]
public class Vehicle_Age_Range
{
[NotMapped]
public string TableName { get { return "Vehicle_Age_Range"; } }
[Column("age_range_id")]
[Key]
public int Id{get;set;}
[Column("range_name")]
[StringLength(50)]
public string range_name{get;set;}
[Column("min_value")]
public int min_value{get;set;}
[Column("max_value")]
public int max_value{get;set;}
[Column("factor")]
public float factor{get;set;}
}
}
