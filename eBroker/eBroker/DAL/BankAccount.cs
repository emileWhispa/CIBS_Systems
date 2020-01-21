using eBroker;
using eBroker.DAL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("BankAccount")]
    public class BankAccount
    {
        [NotMapped]
        public string TableName => "BankAccount";

        [Column("UID")]
        [Key]
        public int Id { get; set; }

        [Column("BankID")]
        [Display(Name = "Bank")]
        public int BankId { get; set; }

        [ForeignKey("BankId")]
        public virtual AcBank Banks { get; set; }

        [Required]
        [Column("AccountNo")]
        [StringLength(30)]
        [Display(Name = "Account No")]
        public string AccountNo { get; set; }

        [Required]
        [Column("AccountDescription")]
        [StringLength(200)]
        [Display(Name = "Description")]
        public string AccountDescription { get; set; }

        [Column("CurrencyID")]
        [Display(Name = "Currency")]
        public int CurrencyID { get; set; }

        [ForeignKey("CurrencyID")]
        public virtual Currency Currencies { get; set; }

        [Column("LedgerNo")]
        [Display(Name = "Ledger No")]
        public int LedgerNo { get; set; }

    }
}