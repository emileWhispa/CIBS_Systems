using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Sector")]
public class Sector
{
[NotMapped]
public string TableName { get { return "Sector"; } }
[Column("SectorID")]
[Key]
public int Id{get;set;}
[Column("DistrictID")]
public int DistrictID{get;set;}
[Column("Sector")]
[StringLength(30)]
public string SectorName{get;set;}
}
}
