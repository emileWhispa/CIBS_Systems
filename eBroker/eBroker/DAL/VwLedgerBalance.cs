using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker.DAL
{
    [Table("Vw_Ledger_Balance")]
    public class VwLedgerBalance
    {
        [Key]
        public Guid Guid { get; set; }

        public int LedgerNo { get; set; }
        public string LedgerDescription { get; set; }
        public string IsoCode { get; set; }
        public decimal Balance { get; set; }
        public DateTime BalanceDate { get; set; }
    }
}