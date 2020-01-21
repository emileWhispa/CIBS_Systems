using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Expense")]
    public class Expense
    {
        [NotMapped]
        public string TableName => "Expense";

        [Column("UID")]
        [Key]
        public int Id { get; set; }

        [Display(Name = "Expense Date")]
        [Column("ExpenseDate")]
        public DateTime ExpenseDate { get; set; }

        [Column("Amount")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Currency")]
        [Column("CurrencyID")]
        public int? CurrencyID { get; set; }

        [ForeignKey("CurrencyID")]
        public virtual Currency Currencies { get; set; }

        [Required]
        [Column("Description")]
        [StringLength(200)]
        public string Description { get; set; }

        [Column("Reversal")]
        public bool Reversal { get; set; }

        [Display(Name = "Expense Ledger")]
        [Column("ExpenseLedgerNo")]
        public int ExpenseLedgerNo { get; set; }

        [NotMapped]
        public int ExpenseCategory { get; set; }


        [Display(Name = "Payment Ledger")]
        [Column("PaymentLedgerNo")]
        public int PaymentLedgerNo { get; set; }

        [Column("UserID")]
        [StringLength(120)]
        public string UserID { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}