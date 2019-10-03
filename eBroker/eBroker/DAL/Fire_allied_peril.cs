using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Fire_allied_peril")]
public class Fire_allied_peril
{
[NotMapped]
public string TableName { get { return "Fire_allied_peril"; } }
[Column("peril_id")]
[Key]
public int Id{get;set;}
[Column("peril_description")]
[StringLength(50)]
public string peril_description{get;set;}
}
}
