using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("BankCollateral")]
    public class BankCollateral
    {
        [NotMapped]
        public string TableName { get { return "BankCollateral"; } }
        [Column("CollateralID")]
        [Key]
        public int Id { get; set; }
        [Column("Bank_ID")]
        public int Bank_ID { get; set; }
        [ForeignKey("Bank_ID")]
        public virtual Bank Banks { get; set; }
        [Column("AccountNumber")]
        [StringLength(30)]
        public string AccountNumber { get; set; }
        [ForeignKey("AccountNumber")]
        public virtual BankLoan BankLoans { get; set; }
        [Column("CollateralDescription")]
        [StringLength(500)]
        public string CollateralDescription { get; set; }
        [Column("MarketValue")]
        public int MarketValue { get; set; }
        [Column("CollateralType")]
        [StringLength(200)]
        public string CollateralType { get; set; }

        public string getAccName
        {
            get
            {
                return this.BankLoans == null ? "--" : this.BankLoans.AccountName;
            }
        }
    }
}
