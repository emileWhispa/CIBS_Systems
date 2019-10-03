using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("BankLoans")]
    public class BankLoan
    {
        [NotMapped]
        public string TableName { get { return "BankLoans"; } }
        [Column("AccountNumber")]
        [Key]
        [StringLength(30)]
        public string AccountNumber { get; set; }
        [Column("Bank_ID")]
        public int Bank_ID { get; set; }
        [ForeignKey("Bank_ID")]
        public virtual Bank Banks { get; set; }
        [Column("AccountName")]
        [StringLength(200)]
        public string AccountName { get; set; }
        [Column("PhoneNumber")]
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        [Column("ProductName")]
        [StringLength(100)]
        public string ProductName { get; set; }
        [Column("DisbursementDate")]
        public DateTime DisbursementDate { get; set; }
        [Column("MaturityDate")]
        public DateTime MaturityDate { get; set; }
        [Column("Status")]
        [StringLength(20)]
        public string Status { get; set; }
        [Column("Branch")]
        [StringLength(50)]
        public string Branch { get; set; }
        [Column("AccountManager")]
        [StringLength(50)]
        public string AccountManager { get; set; }
        [Column("InsuranceStatus")]
        [StringLength(20)]
        public string InsuranceStatus { get; set; }
        [Column("VerifiedBy")]
        [StringLength(50)]
        public string VerifiedBy { get; set; }
    }
}
