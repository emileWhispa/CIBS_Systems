using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker.DAL
{
    [Table("CurrencyList")]
    public class CurrencyList
    {
        [NotMapped]
        public string TableName => "CurrencyList";

        [Column("ISO_Currency_Code")]
        [Required]
        [Key]
        [Display(Name = "Currency Code")]
        public string ISO_Currency_Code { get; set; }
        [Column("CurrencyName")]
        [StringLength(100)]
        [Display(Name = "Currency Name")]
        public string CurrencyName { get; set; }
    }
}
