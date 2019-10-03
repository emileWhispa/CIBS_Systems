using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Vehicle_Usage")]
public class Vehicle_Usage
{
[NotMapped]
public string TableName { get { return "Vehicle_Usage"; } }
[Column("usage_id")]
[Key]
public int Id{get;set;}
[Column("usage")]
[StringLength(50)]
public string usage{get;set;}
}
}
