using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Property_Wall_Material")]
public class Property_Wall_Material
{
[NotMapped]
public string TableName { get { return "Property_Wall_Material"; } }
[Column("UID")]
[Key]
public int Id{get;set;}
[Column("WallMaterial")]
[StringLength(50)]
public string WallMaterial{get;set;}
}
}
