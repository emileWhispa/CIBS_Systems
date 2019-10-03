using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Property_peril")]
public class Property_peril
{
[NotMapped]
public string TableName { get { return "Property_peril"; } }
[Column("property_id")]
[Required()]
[Key]
public int Id{get;set;}
[Column("peril_id")]
[Required()]
public int peril_id{get;set;}
}
}
