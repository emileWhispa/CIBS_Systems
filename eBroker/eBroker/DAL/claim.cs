using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("claim")]
public class claim
{
[NotMapped]
public string TableName { get { return "claim"; } }
[Column("claim_id")]
[Key]
public int Id{get;set;}
[Column("policy_no")]
[StringLength(50)]
public string policy_no{get;set;}
[Column("insurance_company_id")]
public int insurance_company_id{get;set;}
[Column("product_id")]
public int product_id{get;set;}
[Column("expiry_dt")]
public DateTime expiry_dt{get;set;}
[Column("incident_dt")]
public DateTime incident_dt{get;set;}
[Column("our_client")]
public bool our_client{get;set;}
[Column("contact_phone")]
[StringLength(30)]
public string contact_phone{get;set;}
[Column("contact_email")]
[StringLength(100)]
public string contact_email{get;set;}
[Column("claim_form_path")]
[StringLength(500)]
public string claim_form_path{get;set;}
[Column("support_doc_path_1")]
[StringLength(500)]
public string support_doc_path_1{get;set;}
[Column("support_doc_path_2")]
[StringLength(500)]
public string support_doc_path_2{get;set;}
[Column("support_doc_path_3")]
[StringLength(500)]
public string support_doc_path_3{get;set;}
[Column("support_doc_path_4")]
[StringLength(500)]
public string support_doc_path_4{get;set;}
[Column("status")]
[StringLength(20)]
public string status{get;set;}
}
}
