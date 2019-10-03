using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Vehicle_Class")]
public class Vehicle_Clas
{
[NotMapped]
public string TableName { get { return "Vehicle_Class"; } }
[Column("vehicle_class_id")]
[Key]
public int Id{get;set;}
[Column("vehicle_class")]
[StringLength(50)]
public string vehicle_class{get;set;}
}
}
