using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Vw_Bank_Collateral")]
    public class Vw_Bank_Collateral
    {
        [NotMapped]
        public string TableName { get { return "Vw_Bank_Collateral"; } }
        [Column("CollateralID")]
        [Key]
        public int CollateralID { get; set; }
        [Column("AccountNumber")]
        [StringLength(30)]
        public string AccountNumber { get; set; }
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
        [Column("CollateralDescription")]
        [StringLength(500)]
        public string CollateralDescription { get; set; }
        [Column("MarketValue")]
        [Required()]
        public int MarketValue { get; set; }
        [Column("CollateralType")]
        [StringLength(200)]
        public string CollateralType { get; set; }
        [Column("BankName")]
        [StringLength(100)]
        public string BankName { get; set; }
        [Column("InsuranceStatus")]
        [StringLength(20)]
        public string InsuranceStatus { get; set; }
    }
}
