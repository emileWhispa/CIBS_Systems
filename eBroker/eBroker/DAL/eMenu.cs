using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("eMenus")]
public class eMenu
{
[NotMapped]
public string TableName { get { return "eMenus"; } }
[Column("id")]
[Key]
public int Id{get;set;}
[Column("parentid")]
public int parentid{get;set;}
[Column("menu")]
[StringLength(50)]
public string menu{get;set;}
[Column("url")]
[StringLength(200)]
public string url{get;set;}
}
}
