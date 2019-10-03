using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Invoice_Detail")]
    public class Invoice_Detail
    {
        [NotMapped]
        public string TableName { get { return "Invoice_Detail"; } }
        [Column("detail_invoice_id")]
        [Key]
        public int Id { get; set; }
        [Column("invoice_id")]
        [Required]
        public int invoice_id { get; set; }
        [Column("contract_id")]
        [Required]
        public int contract_id { get; set; }
        [ForeignKey("contract_id")]
        public virtual InsurancePolicy InsurancePolicies { get; set; }
        [Column("commission_percentage")]
        [Required]
        public double commission_percentage { get; set; }
        [Column("commission_amt")]
        public double commission_amt { get; set; }
    }
}
