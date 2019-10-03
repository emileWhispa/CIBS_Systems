using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Vehicle")]
public class Vehicle
{
[NotMapped]
public string TableName { get { return "Vehicle"; } }
[Column("vehicle_id")]
[Key]
public int Id{get;set;}
[Column("plate_no")]
[StringLength(20)]
public string plate_no{get;set;}
[Column("make")]
[StringLength(50)]
public string make{get;set;}
[Column("model")]
[StringLength(50)]
public string model{get;set;}
[Column("class_id")]
public int class_id{get;set;}
[Column("engine_no")]
[StringLength(50)]
public string engine_no{get;set;}
[Column("chassis_no")]
[StringLength(50)]
public string chassis_no{get;set;}
[Column("manufacture_year")]
public int manufacture_year{get;set;}
[Column("value")]
public float value{get;set;}
[Column("product_id")]
public int product_id{get;set;}
[Column("vehicle_usage_id")]
public int vehicle_usage_id{get;set;}
[Column("insurance_days")]
public int insurance_days{get;set;}
[Column("territorial_cover_id")]
public int territorial_cover_id{get;set;}
[Column("number_occupants")]
public int number_occupants{get;set;}
[Column("occupant_id")]
public int occupant_id{get;set;}
[Column("family_extension")]
public bool family_extension{get;set;}
[Column("waive_excess")]
public bool waive_excess{get;set;}
[Column("firstname")]
[StringLength(50)]
public string firstname{get;set;}
[Column("lastname")]
[StringLength(50)]
public string lastname{get;set;}
[Column("mobile")]
[StringLength(30)]
public string mobile{get;set;}
[Column("email")]
[StringLength(50)]
public string email{get;set;}
[Column("physical_address")]
[StringLength(50)]
public string physical_address{get;set;}
[Column("language")]
[StringLength(20)]
public string language{get;set;}
[Column("start_date")]
public DateTime start_date{get;set;}
[Column("insurance_company_id")]
public int insurance_company_id{get;set;}
[Column("quotation_date")]
public DateTime quotation_date{get;set;}
}
}
