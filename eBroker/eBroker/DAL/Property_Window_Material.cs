using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Property_Window_Material")]
public class Property_Window_Material
{
[NotMapped]
public string TableName { get { return "Property_Window_Material"; } }
[Column("UID")]
[Key]
public int Id{get;set;}
[Column("WindowMaterial")]
[StringLength(50)]
public string WindowMaterial{get;set;}
}
}
