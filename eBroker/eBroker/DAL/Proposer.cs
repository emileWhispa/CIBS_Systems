using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Proposer")]
public class Proposer
{
[NotMapped]
public string TableName { get { return "Proposer"; } }
[Column("proposer_id")]
[Key]
public int Id{get;set;}
[Column("proposer_name")]
[StringLength(100)]
public string proposer_name{get;set;}
[Column("proposer_type")]
[StringLength(20)]
public string proposer_type{get;set;}
[Column("dob")]
public DateTime dob{get;set;}
[Column("gender")]
public char gender{get;set;}
[Column("ID_No")]
[StringLength(50)]
public string ID_No{get;set;}
[Column("mobile")]
[StringLength(15)]
public string mobile{get;set;}
[Column("email")]
[StringLength(50)]
public string email{get;set;}
[Column("physical_address")]
[StringLength(200)]
public string physical_address{get;set;}
[Column("prefered_language")]
[StringLength(20)]
public string prefered_language{get;set;}
[Column("insurance_company_id")]
public int insurance_company_id{get;set;}
[Column("proposal_date")]
public DateTime proposal_date{get;set;}
}
}
