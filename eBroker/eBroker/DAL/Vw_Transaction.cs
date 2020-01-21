using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker.DAL
{
    [Table("Vw_Transactions")]
    public class Vw_Transaction
    {
        [NotMapped]
        public string TableName => "Vw_Transactions";

        [Column("UID")]
        [Key]
        public int Id { get; set; }

        [Column("IsoCode")]
        [Display(Name = "Currency")]
        public string Currency { get; set; }

        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("Rate")]
        public decimal Rate { get; set; }

        [Column("RWF")]
        public decimal RWF { get; set; }

        [Column("Reversed")]
        public bool Reversed { get; set; }

        [Column("CustomerName")]
        [Display(Name = "Customer Name")]
        [StringLength(250)]
        public string CustomerName { get; set; }

        [Column("CustomerNationality")]
        [Display(Name = "CustomerNationality")]
        [StringLength(50)]
        public string CustomerNationality { get; set; }

        [Column("CustomerIDNumber")]
        [Display(Name = "ID Number")]
        [StringLength(50)]
        public string CustomerIDNumber { get; set; }

        [Column("IsResident")]
        [Display(Name = "Resident?")]
        public bool IsResident { get; set; }

        [Column("TranDate")]
        [Display(Name = "Date")]
        public DateTime TranDate { get; set; }

        [Column("TranType")]
        [Display(Name = "Transaction Type")]
        [StringLength(50)]
        public string TranType { get; set; }

        [Display(Name = "Exchange type")]
        public string TransactionType { get; set; }

        [Column("Employee")]
        [StringLength(120)]
        public string Employee { get; set; }

        [Column("ReferenceNo")]
        [StringLength(25)]
        public string ReferenceNo { get; set; }

        [Column("SerialNumber")]
        public int SerialNumber { get; set; }

        [Column("Mobile")]
        [StringLength(15)]
        public string Mobile { get; set; }
        public string BranchName { get; set; }

        public string GetTransactionType => string.IsNullOrWhiteSpace(TransactionType) ? "Cash" : TransactionType;
    }
}