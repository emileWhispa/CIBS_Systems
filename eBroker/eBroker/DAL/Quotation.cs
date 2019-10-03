using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Quotation")]
public class Quotation
{
[NotMapped]
public string TableName { get { return "Quotation"; } }
[Column("quotation_id")]
[Key]
public int Id{get;set;}
[Column("insurance_id")]
public int insurance_id{get;set;}
[Column("proposer_id")]
public int proposer_id{get;set;}
[Column("quotation_date")]
public DateTime quotation_date{get;set;}
[Column("quotation_number")]
[StringLength(15)]
public string quotation_number{get;set;}
[Column("net_premium")]
public decimal net_premium{get;set;}
[Column("vat")]
public decimal vat{get;set;}
[Column("special_condition")]
[StringLength(500)]
public string special_condition{get;set;}
[Column("attachment_name")]
[StringLength(50)]
public string attachment_name{get;set;}
}
}
