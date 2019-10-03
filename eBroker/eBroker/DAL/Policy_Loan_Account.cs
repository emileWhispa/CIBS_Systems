using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Policy_Loan_Account")]
    public class Policy_Loan_Account
    {
        [NotMapped]
        public string TableName { get { return "Policy_Loan_Account"; } }
        [Key]
        [Column("policy_loan_id")]
        public int Id { get; set; }
        [Column("contract_id")]
        [Required]
        public int contract_id { get; set; }
        [ForeignKey("contract_id")]
        public virtual InsurancePolicy InsurancePolicies { get; set; }
        [Column("loan_type_id"), Display(Name = "Loan Type")]
        public int? loan_type_id { get; set; }
        [ForeignKey("loan_type_id")]
        public virtual LoanType LoanTypes { get; set; }
        [Column("loan_account"), Display(Name = "Loan Account")]
        [StringLength(50)]
        public string loan_account { get; set; }
        [Column("loan_disbursement_date"), Display(Name = "Disbursement Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime loan_disbursement_date { get; set; }
        [Column("loan_expiry_date"), Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime loan_expiry_date { get; set; }
        [Column("entered_by")]
        public string entered_by { get; set; }
        [Column("entry_date")]
        public DateTime entry_date { get; set; }

    }
}
