using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker.DAL
{
    [Table("GL_Balance")]
    public class GlBalance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LedgerNo { get; set; }

        [Column("CurrencyID")]
        public int CurrencyId { get; set; }
        
        public Currency Currency { get; set; }

        [Column]
        public decimal Balance { get; set; }

        public DateTime BalanceDate { get; set; }
        public decimal MiddleRate { get; set; }
        public int ForexId { get; set; }
    }
}