using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker.DAL
{
    [Table("BankReconciliation")]
    public class BankReconciliation
    {
        [NotMapped]
        public string TableName => "BankReconciliation";

        [Column("ReconciliationID")]
        [Key]
        public int Id { get; set; }

        [Column("ReconciliationDate")]
        public DateTime ReconciliationDate { get; set; }

        [Column("LastReconciliationDate")]
        public DateTime LastReconciliationDate { get; set; }

        [Column("LedgerNo")]
        public int LedgerNo { get; set; }

        [ForeignKey("LedgerNo")]
        public BankAccount BankAccount { get; set; }


        [Column("BeginningBalance")]
        public double BeginningBalance { get; set; }

        [Column("EndingBalance")]
        public double EndingBalance { get; set; }

        [Column("UserID")]
        [StringLength(120)]
        public string UserID { get; set; }

        public string FormattedLastReconciliationDate => ReconciliationDate.ToString("yyyy-MM-dd");
    }
}