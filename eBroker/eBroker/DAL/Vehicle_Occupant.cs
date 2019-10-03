using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Vehicle_Occupant")]
public class Vehicle_Occupant
{
[NotMapped]
public string TableName { get { return "Vehicle_Occupant"; } }
[Column("occupant_id")]
[Key]
public int Id{get;set;}
[Column("premium")]
public int premium{get;set;}
[Column("death_amount")]
public int death_amount{get;set;}
[Column("permanent_disability_amount")]
public int permanent_disability_amount{get;set;}
[Column("medical_fees")]
public int medical_fees{get;set;}
}
}
