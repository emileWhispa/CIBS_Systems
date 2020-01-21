using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Vw_Expense_Voucher")]
    public class Vw_Expense_Voucher
    {
        [NotMapped]
        public string TableName => "Vw_Expense_Voucher";

        [Column("UID")]
        [Key]
        public string Id { get; set; }

        [Column("ExpenseDate")]
        public DateTime ExpenseDate { get; set; }

        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("IsoCode")]
        public string IsoCode { get; set; }

        [Column("Description")]
        [StringLength(200)]
        public string Description { get; set; }

        [Column("Reversal")]
        public bool Reversal { get; set; }

        [Column("ExpenseLedger")]
        public string ExpenseLedger { get; set; }

        [Column("PaymentLedger")]
        public string PaymentLedger { get; set; }

        [Column("UserID")]
        [StringLength(120)]
        public string UserID { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }
        

    }
}