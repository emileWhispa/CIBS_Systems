using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Property")]
public class Property
{
[NotMapped]
public string TableName { get { return "Property"; } }
[Column("property_id")]
[Key]
public int Id{get;set;}
[Column("property_description")]
[StringLength(50)]
public string property_description{get;set;}
[Column("plot_number")]
[StringLength(20)]
public string plot_number{get;set;}
[Column("province")]
[StringLength(20)]
public string province{get;set;}
[Column("district")]
[StringLength(20)]
public string district{get;set;}
[Column("secteur")]
[StringLength(20)]
public string secteur{get;set;}
[Column("wall_material")]
[StringLength(20)]
public string wall_material{get;set;}
[Column("roof_material")]
[StringLength(20)]
public string roof_material{get;set;}
[Column("window_material")]
[StringLength(20)]
public string window_material{get;set;}
[Column("property_use")]
[StringLength(20)]
public string property_use{get;set;}
[Column("fire_protection_system")]
[StringLength(50)]
public string fire_protection_system{get;set;}
[Column("theft_protection_system")]
[StringLength(50)]
public string theft_protection_system{get;set;}
[Column("occupied")]
public bool occupied{get;set;}
[Column("floors")]
[StringLength(20)]
public string floors{get;set;}
[Column("insured_value")]
public decimal insured_value{get;set;}
[Column("volcanic_region")]
public bool volcanic_region{get;set;}
[Column("inflammable_neighborhood")]
public bool inflammable_neighborhood{get;set;}
[Column("insurance_days")]
public int insurance_days{get;set;}
[Column("proposer_id")]
public int proposer_id{get;set;}
}
}
