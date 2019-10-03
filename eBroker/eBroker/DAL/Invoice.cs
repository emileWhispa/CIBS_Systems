using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Invoice")]
    public class Invoice
    {
        [NotMapped]
        public string TableName { get { return "Invoice"; } }
        [Column("invoice_id")]
        [Key]
        public int Id { get; set; }
        [Column("insurer_id")]
        [Display(Name = "Insurance Company")]
        [Required]
        public int insurer_id { get; set; }
        [ForeignKey("insurer_id")]
        public virtual Partner Insurers { get; set; }
        [Column("invoice_dt")]
        [Required]
        public DateTime invoice_dt { get; set; }
        [Column("invoice_due_dt")]
        [Display(Name = "Invoice Due Date")]
        [Required]
        public DateTime invoice_due_dt { get; set; }
        [Column("invoice_until_dt")]
        [Display(Name = "Invoice Until")]
        [Required]
        public DateTime invoice_until_dt { get; set; }
        [Column("PreparedBy")]
        [Display(Name = "Prepared By")]
        [Required]
        public string PreparedBy { get; set; }
        [Column("CheckedBy")]
        [Display(Name = "Checked By")]
        [Required]
        public string CheckedBy { get; set; }
        [Column("user_id")]
        [StringLength(30)]
        public string user_id { get; set; }
        [Column("Status")]
        [StringLength(50)]
        public string Status { get; set; }
        [Column("vatable")]
        [Display(Name = "Vatable")]
        public bool vatable { get; set; }
    }
}
