using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Currency")]
    public class Currency
    {
        [NotMapped]
        public string TableName => "Currency";

        [Column("UID")]
        [Key]
        public int Id { get; set; }

        [Column("IsoCode")]
        [Required]
        [Display(Name = "Currency Name")]
        public string IsoCode { get; set; }

        [Column("CurrencyName")]
        [StringLength(50)]
        [Display(Name = "Currency Name")]
        public string CurrencyName { get; set; }

        [Column("Active")] public bool Active { get; set; }
        [Column("CreatedOn")] public DateTime CreatedOn { get; set; }

        [Column("CreatedBy")]
        [StringLength(50)]
        public string CreatedBy { get; set; }
    }
}