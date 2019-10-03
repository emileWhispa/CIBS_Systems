using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Brokerage_Company")]
public class Brokerage_Company
{
[NotMapped]
public string TableName { get { return "Brokerage_Company"; } }
[Column("company_code")]
[Key]
public int Id{get;set;}
[Column("company_name")]
[Required()]
[StringLength(200)]
public string company_name{get;set;}
[Column("contact_person")]
[StringLength(50)]
public string contact_person{get;set;}
[Column("mobile")]
[StringLength(15)]
public string mobile{get;set;}
[Column("office_phone")]
[StringLength(15)]
public string office_phone{get;set;}
[Column("email")]
[StringLength(50)]
public string email{get;set;}
[Column("physical_address")]
[StringLength(100)]
public string physical_address{get;set;}
[Column("create_dt")]
public DateTime create_dt{get;set;}
[Column("created_by")]
[StringLength(20)]
public string created_by{get;set;}
}
}
