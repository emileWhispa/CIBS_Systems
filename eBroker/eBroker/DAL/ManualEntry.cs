using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("ManualEntry")]
    public class ManualEntry
    {
        [NotMapped]
        public string TableName => "ManualEntry";

        [Column("UID")]
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide the date")]
        [Display(Name = "Entry Date")]
        [Column("EntryDate")]
        public DateTime EntryDate { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Specify the debit ledger Group")]
        [Display(Name = "Debit Ledger Group")]
        public int DebitLedgerGroup { get; set; }

        [Required(ErrorMessage = "Specify the debit ledger No")]
        [Display(Name = "Debit Ledger")]
        [Column("DebitLedgerNo")]
        public int DebitLedgerNo { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Specify the credit ledger Group")]
        [Display(Name = "Credit Ledger Group")]
        public int CreditLedgerGroup { get; set; }

        [Column("CreditLedgerNo")]
        [Required(ErrorMessage = "Specify the credit ledger No")]
        [Display(Name = "Credit Ledger")]
        public int CreditLedgerNo { get; set; }

        [Column("Amount")]
        [Required(ErrorMessage = "Specify the amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Currency")]
        [Column("CurrencyID")]
        [Required(ErrorMessage = "Specify the amount")]
        public int CurrencyID { get; set; }

        [ForeignKey("CurrencyID")]
        public virtual Currency Currencies { get; set; }

        [Display(Name = "Reason")]
        [Column("Description")]
        [Required(ErrorMessage = "Give the entry description")]
        [StringLength(200)]
        public string Description { get; set; }

        [Column("Reversal")]
        public bool Reversal { get; set; }

        [Column("UserID")]
        [StringLength(120)]
        public string UserID { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }


    }
}