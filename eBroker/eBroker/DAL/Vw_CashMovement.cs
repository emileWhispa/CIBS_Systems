using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Vw_CashMovement")]
    public class Vw_CashMovement
    {
        [NotMapped]
        public string TableName => "Vw_CashMovement";

        [Column("UID")]
        [Key]
        public string Id { get; set; }

        [Column("TransactionDate")]
        public DateTime TransactionDate { get; set; }

        [Column("TransactionType")]
        [StringLength(50)]
        public string TransactionType { get; set; }

        [Column("BankName")]
        public string BankName { get; set; }

        [Column("AccountNo")]
        [StringLength(20)]
        public string AccountNo { get; set; }

        [Column("Amount")]
        public int Amount { get; set; }

        [Column("IsoCode")]
        public string IsoCode { get; set; }

        [Column("SourceLedger")]
        public string SourceLedger { get; set; }

        [Column("DestinationLedger")]
        public string DestinationLedger { get; set; }

        [Column("Reason")]
        [StringLength(200)]
        public string Reason { get; set; }

        [Column("Reversal")]
        public bool Reversal { get; set; }

        [Column("Inputer")]
        [StringLength(120)]
        public string Inputer { get; set; }

        [Column("CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}