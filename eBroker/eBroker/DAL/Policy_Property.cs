using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Policy_Property")]
public class Policy_Property
{
[NotMapped]
public string TableName { get { return "Policy_Property"; } }
[Column("UID")]
[Key]
public int Id{get;set;}
[Column("contract_id")]
[Required]
public int contract_id { get; set; }
[ForeignKey("contract_id")]
public virtual InsurancePolicy InsurancePolicies { get; set; }
[Column("property_description"), Display(Name = "Property Description")]
[StringLength(50)]
[Required]
public string property_description{get;set;}
[Column("plot_number"), Display(Name = "Plot Number")]
[StringLength(20)]
[Required]
public string plot_number { get; set; }
[Column("physical_address"), Display(Name = "Physical Address")]
[StringLength(100)]
[Required]
public string physical_address { get; set; }
[Column("wall_material"), Display(Name = "Wall Material")]
[StringLength(50)]
[Required]
public string wall_material { get; set; }
[Column("roof_material"), Display(Name = "Roof Material")]
[StringLength(50)]
[Required]
public string roof_material { get; set; }
[Column("window_material"), Display(Name = "Window Material")]
[StringLength(50)]
[Required]
public string window_material { get; set; }
[Column("property_use"), Display(Name = "Property Use")]
[StringLength(50)]
[Required]
public string property_use { get; set; }
[Column("floors"), Display(Name = "Floors")]
public int floors{get;set;}
[Required]
[Column("insured_value"), Display(Name = "Insured Value")]
public double insured_value{get;set;}
[Required]
[Column("volcanic_region"), Display(Name = "Volcano Region?")]
public bool volcanic_region{get;set;}
[Column("inflammable_neighborhood"), Display(Name = "Inflammable Neighborhood?")]
public bool inflammable_neighborhood{get;set;}
[Column("entry_date")]
public DateTime entry_date { get; set; }
[Column("entry_user")]
[StringLength(30)]
public string entry_user { get; set; }
}
}
