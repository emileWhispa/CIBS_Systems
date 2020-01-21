using eBroker;
using eBroker.DAL;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("CashMovement")]
    public class CashMovement
    {
        [NotMapped]
        public string TableName => "CashMovement";

        [Column("UID")]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("TransactionDate")]
        [DataType(DataType.Text)]
        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }

        [Column("TransactionType")]
        [Required]
        [Display(Name = "Transaction Type")]
        [StringLength(50)]
        public string TransactionType { get; set; }

        [Required]
        [Column("BankID")]
        [Display(Name = "Bank")]
        public int BankID { get; set; }

        [ForeignKey("BankID")]
        public virtual AcBank Banks { get; set; }

        [Required]
        [Column("AccountNo")]
        [Display(Name = "Account No.")]
        [StringLength(20)]
        public string AccountNo { get; set; }

        [Required]
        [Column("Amount")]
        public int Amount { get; set; }

        [Required]
        [Column("CurrencyID")]
        [Display(Name = "Currency")]
        public int CurrencyID { get; set; }

        [ForeignKey("CurrencyID")]
        public virtual Currency Currencies { get; set; }

        [Column("SourceLedger")]
        public int SourceLedger { get; set; }

        [ForeignKey("SourceLedger")]
        public virtual GL_Account SourceLedgers { get; set; }

        [Column("DestinationLedger")]
        public int DestinationLedger { get; set; }

        [ForeignKey("DestinationLedger")]
        public virtual GL_Account DestinationLedgers { get; set; }

        [Column("Reason")]
        [StringLength(200)]
        [Required]
        public string Reason { get; set; }

        [Column("CheckNo")]
        [Required]
        [Display(Name = "Check No.")]
        [StringLength(50)]
        public string CheckNo { get; set; }

        [Column("Reversal")]
        public bool Reversal { get; set; }

        [Column("UserID")]
        [StringLength(120)]
        public string UserID { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }

        public string TransactionDateString => TransactionDate.ToShortDateString();
    }
}