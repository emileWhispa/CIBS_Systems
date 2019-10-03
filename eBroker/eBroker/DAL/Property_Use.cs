using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Property_Use")]
public class Property_Use
{
[NotMapped]
public string TableName { get { return "Property_Use"; } }
[Column("UID")]
[Key]
public int Id{get;set;}
[Column("PropertyUse")]
[StringLength(50)]
public string PropertyUse{get;set;}
}
}
