using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker.DAL
{
    [Table("GL_History")]
    public class GL_History
    {
        [NotMapped]
        public string TableName => "GL_History";

        [Column("UID")]
        [Key]
        public int Id { get; set; }

        [Column("LedgerNo")]
        public int LedgerNo { get; set; }

        [Column("EffectiveDate")]
        public DateTime EffectiveDate { get; set; }

        [NotMapped]
        [Column("Debit")]
        public float Debit { get; set; }

        [NotMapped]
        [Column("Credit")]
        public float Credit { get; set; }

        [Column("Rate")]
        public double Rate { get; set; }

        [NotMapped]
        [Column("LedgerDescription")]
        [StringLength(200)]
        public string LedgerDescription { get; set; }

        [NotMapped]
        [Column("IsoCode")]
        public string IsoCode { get; set; }

        [Column("CurrencyId")]
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }
        public int ReferenceNo { get; set; }
        public string Description { get; set; }

        public double Amount { get; set; }

        [StringLength(1)]
        [Column("Sign")]
        public string Sign { get; set; }
    }
}