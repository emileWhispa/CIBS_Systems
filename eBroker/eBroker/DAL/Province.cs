using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Province")]
public class Province
{
[NotMapped]
public string TableName { get { return "Province"; } }
[Column("ProvinceID")]
[Key]
public int Id{get;set;}
[Column("Province")]
[StringLength(30)]
public string ProvinceName{get;set;}
}
}
