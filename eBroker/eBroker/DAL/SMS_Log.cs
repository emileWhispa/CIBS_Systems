using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("SMS_Log")]
public class SMS_Log
{
[NotMapped]
public string TableName { get { return "SMS_Log"; } }
[Column("SMS_ID")]
[Key]
public int Id{get;set;}
[Column("client_name")]
[StringLength(50)]
public string client_name{get;set;}
[Column("phone")]
[StringLength(50)]
public string phone{get;set;}
[Column("policy_no")]
[StringLength(50)]
public string policy_no{get;set;}
[Column("insurer")]
[StringLength(50)]
public string insurer{get;set;}
[Column("expiry_date")]
public DateTime expiry_date{get;set;}
[Column("status")]
[StringLength(20)]
public string status{get;set;}
[Column("balance")]
[StringLength(20)]
public string balance{get;set;}
[Column("user_id")]
[StringLength(30)]
public string user_id { get; set; }
[Column("system_date")]
[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
public DateTime system_date { get; set; }

}
}
