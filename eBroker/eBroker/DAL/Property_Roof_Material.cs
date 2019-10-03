using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Property_Roof_Material")]
public class Property_Roof_Material
{
[NotMapped]
public string TableName { get { return "Property_Roof_Material"; } }
[Column("UID")]
[Key]
public int Id{get;set;}
[Column("RoofMaterial")]
[StringLength(50)]
public string RoofMaterial{get;set;}
}
}
