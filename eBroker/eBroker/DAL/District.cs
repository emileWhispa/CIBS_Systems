using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("District")]
public class District
{
[NotMapped]
public string TableName { get { return "District"; } }
[Column("DistrictID")]
[Key]
public int Id{get;set;}
[Column("ProvinceId")]
public int ProvinceId{get;set;}
[Column("District")]
[StringLength(30)]
public string DistrictName{get;set;}
}
}
