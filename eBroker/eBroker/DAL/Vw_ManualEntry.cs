using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Vw_Manual_Entries")]
    public class Vw_ManualEntry
    {
        [NotMapped]
        public string TableName => "Vw_Manual_Entries";

        [Column("UID")]
        [Key]
        public string Id { get; set; }

        [Display(Name = "Entry Date")]
        [Column("EntryDate")]
        public DateTime EntryDate { get; set; }

        [Display(Name = "Debit Ledger")]
        [Column("DebitLedgerNo")]
        public string DebitLedger { get; set; }

        [Column("CreditLedgerNo")]
        [Display(Name = "Credit Ledger")]
        public string CreditLedger { get; set; }

        [Column("Amount")]
        [Required(ErrorMessage = "Specify the amount")]
        public decimal Amount { get; set; }

        [Display(Name = "IsoCode")]
        [Column("IsoCode")]
        public string IsoCode { get; set; }

        [Display(Name = "Description")]
        [Column("Description")]
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
