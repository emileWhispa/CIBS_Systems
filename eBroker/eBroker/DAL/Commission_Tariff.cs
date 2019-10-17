using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Commission_Tariff")]
public class Commission_Tariff
{
[NotMapped]
public string TableName { get { return "Commission_Tariff"; } }
[Column("tariff_id")]
[Key]
public int Id{get;set;}
[Column("insurer_id"), Display(Name = "Insurance Company")]
public int insurer_id{get;set;}
[ForeignKey("insurer_id")]
public virtual Partner Insurers { get; set; }
[Column("product_id"), Display(Name = "Product")]
public int product_id{get;set;}
[ForeignKey("product_id")]
public virtual Insurance_Product InsuranceProducts { get; set; }
[Column("commission_percentage")]
public double commission_percentage {get;set;}

 public String iName { get { return Insurers != null ? Insurers.company_name : "..."; } }
}
}
