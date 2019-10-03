using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Insurance_Product")]
public class Insurance_Product
{
[NotMapped]
public string TableName { get { return "Insurance_Product"; } }
[Column("product_id")]
[Key]
public int Id{get;set;}
[Column("product_name")]
[StringLength(50)]
public string product_name{get;set;}
[Column("category_id")]
public int category_id{get;set;}
}
}
