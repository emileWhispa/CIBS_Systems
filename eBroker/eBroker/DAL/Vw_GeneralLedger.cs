using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker.DAL
{
    [Table("Vw_GeneralLedger")]
    public class Vw_GeneralLedger
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

        [Column("Debit")]
        public double Debit { get; set; }

        [Column("Credit")]
        public double Credit { get; set; }

        [Column("Rate")]
        public double? Rate { get; set; }

        [Column("LedgerDescription")]
        [StringLength(200)]
        public string LedgerDescription { get; set; }

        [Column("Description")]
        [StringLength(200)]
        public string Description { get; set; }

        [Column("IsoCode")]
        public string IsoCode { get; set; }

        [Column("Sign")]
        public char Sign { get; set; }

    }
}