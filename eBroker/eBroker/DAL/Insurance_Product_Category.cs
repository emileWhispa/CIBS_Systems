using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Insurance_Product_Category")]
public class Insurance_Product_Category
{
[NotMapped]
public string TableName { get { return "Insurance_Product_Category"; } }
[Column("category_id")]
[Key]
public int Id{get;set;}
[Column("category_name")]
[StringLength(50)]
public string category_name{get;set;}
}
}
