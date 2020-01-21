using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker.DAL
{
    [Table("Transactions")]
    public class Transaction
    {
        [NotMapped]
        public string TableName => "Transactions";

        [Column("UID")]
        [Key]
        public int Id { get; set; }

        [Column("CurrencyID")]
        [Display(Name = "Currency")]
        [Required]
        public int CurrencyID { get; set; }

        [ForeignKey("CurrencyID")]
        public virtual Currency Currencies { get; set; }

        [Required]
        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("Rate")]
        [Required]
        public decimal Rate { get; set; }

        [Column("RWF")]
        [NotMapped]
        public decimal RWF { get; set; }

        [Column("Reversed")]
        public bool Reversed { get; set; }

        [Required]
        [Column("CustomerName")]
        [Display(Name = "Customer name")]
        [StringLength(250)]
        public string CustomerName { get; set; }


        [Required(ErrorMessage ="This field is required")]
        [Column("CustomerNationality")]
        [Display(Name = "Customer nationality")]
        [StringLength(50)]
        public string CustomerNationality { get; set; }

        [Required]
        [Column("CustomerIDNumber")]
        [Display(Name = "ID Number")]
        [StringLength(50)]
        public string CustomerIDNumber { get; set; }

        [Column("IDType")]
        [Display(Name = "ID Type")]
        public int IDType { get; set; }

        [Column("NatureID")]
        public int NatureID { get; set; }

        [Column("Description")]
        [StringLength(250)]
        public string Description { get; set; }

        [Column("SupportDocument")]
        [StringLength(250)]
        public string SupportDocument { get; set; }

        [NotMapped]
        [Display(Name = "Purpose Category")]
        public int UseOfFundSource { get; set; }


        [Required(ErrorMessage ="This field is required")]
        [Column("IsResident")]
        [Display(Name = "Resident?")]
        public bool IsResident { get; set; }

        [Required]
        [Column("TranCountry")]
        [Display(Name = "Destination Country")]
        [StringLength(50)]
        public string TranCountry { get; set; }

        [Column("TranDate")]
        [Display(Name = "Transaction Date")]
        public DateTime TranDate { get; set; }

        [Column("TranType")]
        [Required(ErrorMessage ="This field is required")]
        [Display(Name = "Transaction Type")]
        [StringLength(50)]
        public string TranType { get; set; }

        [Column("Employee")]
        [StringLength(120)]
        public string Employee { get; set; }

        [Column("SystemDate")]
        public DateTime SystemDate { get; set; }

        [Column("ReferenceNo")]
        [StringLength(25)]
        public string ReferenceNo { get; set; }

        //[Column("SerialNumber")]
        //[Display(Name = "Serial Number")]
        //public int SerialNumber { get; set; }


        [Column("Mobile")]
        [StringLength(15)]
        public string Mobile { get; set; }

        [Required]
        [Column("CustomerType")]
        [Display(Name = "Customer Type")]
        [StringLength(50)]
        public string CustomerType { get; set; }

        [NotMapped]
        public string PurposeCategoryId { get; set; }

        [Display(Name = "Reason")]
        public string ReverseReason { get; set; }

        public decimal GetRwf => Amount * Rate;

        [Display(Name="Exchange type")]
        public string TransactionType { get; set; }

        [Display(Name = "Bank Name")]
        public int? BankId { get; set; }

        [Display(Name = "Foreign Account")]
        public string ForeignAccount { get; set; }

        [Display(Name = "Rwf Account")]
        public string RwfAccount { get; set; }

        public string GetTransactionType => string.IsNullOrWhiteSpace(TransactionType) ? "Cash" : TransactionType;
    }
}