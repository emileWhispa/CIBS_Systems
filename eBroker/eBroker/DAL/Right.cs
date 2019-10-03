using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Rights")]
public class Right
{
[NotMapped]
public string TableName { get { return "Rights"; } }
[Column("RightId")]
[Key]
public int Id{get;set;}
[Column("MenuCode")]
[Required()]
public int MenuCode{get;set;}
[Column("ProfileCode")]
[Required()]
public int ProfileCode{get;set;}
[Column("AllowAccess")]
public bool AllowAccess{get;set;}
[Column("StatusId")]
[Required()]
public int StatusId{get;set;}
}
}
