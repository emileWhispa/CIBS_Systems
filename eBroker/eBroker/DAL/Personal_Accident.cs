using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Personal_Accident")]
public class Personal_Accident
{
[NotMapped]
public string TableName { get { return "Personal_Accident"; } }
[Column("personal_accident_id")]
[Key]
public int Id{get;set;}
[Column("height")]
public float height{get;set;}
[Column("weight")]
public float weight{get;set;}
[Column("occupation")]
[StringLength(50)]
public string occupation{get;set;}
[Column("work_place")]
[StringLength(50)]
public string work_place{get;set;}
[Column("tools_machinery")]
public bool tools_machinery{get;set;}
[Column("manual_work")]
public bool manual_work{get;set;}
[Column("manual_work_supervision")]
public bool manual_work_supervision{get;set;}
[Column("previous_insurance")]
public bool previous_insurance{get;set;}
[Column("previous_insurance_company")]
[StringLength(50)]
public string previous_insurance_company{get;set;}
[Column("prev_ins_declined_proposal")]
public bool prev_ins_declined_proposal{get;set;}
[Column("prev_ins_declined_renewal")]
public bool prev_ins_declined_renewal{get;set;}
[Column("prev_ins_increased_rate")]
public bool prev_ins_increased_rate{get;set;}
[Column("prev_ins_imposed_terms")]
public bool prev_ins_imposed_terms{get;set;}
[Column("medical_attention")]
[StringLength(50)]
public string medical_attention{get;set;}
[Column("prev_claim_injury")]
[StringLength(50)]
public string prev_claim_injury{get;set;}
[Column("medical_doctor")]
[StringLength(50)]
public string medical_doctor{get;set;}
[Column("alcohol_consumption_level")]
[StringLength(50)]
public string alcohol_consumption_level{get;set;}
[Column("alcohol_suffering")]
[StringLength(50)]
public string alcohol_suffering{get;set;}
[Column("gout")]
public bool gout{get;set;}
[Column("diabetes")]
public bool diabetes{get;set;}
[Column("paralysis")]
public bool paralysis{get;set;}
[Column("fit")]
public bool fit{get;set;}
[Column("rapture")]
public bool rapture{get;set;}
[Column("varicose_veins")]
public bool varicose_veins{get;set;}
[Column("treatment_details")]
[StringLength(50)]
public string treatment_details{get;set;}
[Column("good_health")]
public bool good_health{get;set;}
[Column("current_illness")]
[StringLength(50)]
public string current_illness{get;set;}
[Column("amt_death")]
public float amt_death{get;set;}
[Column("amt_permanent_disability")]
public float amt_permanent_disability{get;set;}
[Column("amt_medical_expense")]
public float amt_medical_expense{get;set;}
[Column("amt_temporary_disability")]
public float amt_temporary_disability{get;set;}
[Column("beneficiary_name_address")]
[StringLength(100)]
public string beneficiary_name_address{get;set;}
[Column("proposer_id")]
public int proposer_id{get;set;}
}
}
