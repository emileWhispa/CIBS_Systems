using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("insurance_policy")]
    public class InsurancePolicy
    {
        [NotMapped]
        public string TableName { get { return "insurance_policy"; } }
        [Column("contract_id")]
        [Key]
        public int Id { get; set; }
        [Column("policy_no"), Display(Name = "Policy Number")]
        [StringLength(50)]
        [Required]
        public string policy_no { get; set; }
        [Column("policy_type"), Display(Name = "Policy Type")]
        [StringLength(20)]
        [Required]
        public string policy_type { get; set; }
        [Column("amendment_no"), Display(Name = "Amendment Number")]
        [StringLength(50)]
        [Required]
        public string amendment_no { get; set; }
        [Column("insurance_id"), Display(Name = "Insurance Company")]
        [Required]
        public int insurance_id { get; set; }
        [ForeignKey("insurance_id")]
        public virtual Partner Partners { get; set; }
        [Column("client_id"), Display(Name = "Client Name")]
        [Required]
        public int client_id { get; set; }
        [ForeignKey("client_id")]
        public virtual Client Clients { get; set; }
        [Column("product_id"), Display(Name = "Insurance Product")]
        [Required]
        public int product_id { get; set; }
        [ForeignKey("product_id")]
        public virtual Insurance_Product InsuranceProducts { get; set; }
        [Column("effective_dt"), Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime effective_dt { get; set; }
        [DataType(DataType.Date)]
        [Column("expiry_dt"), Display(Name = "Expiry Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime expiry_dt { get; set; }
        [Column("net_premium"), Display(Name = "Net Premium")]
        [DisplayFormat(DataFormatString = "{0:0,0}")]
        public int net_premium { get; set; }
        [Column("total_paid"), Display(Name = "Total Paid")]
        [DisplayFormat(DataFormatString = "{0:0,0}")]
        [Required]
        public int? total_paid { get; set; }
        [Column("interest_transfer"), Display(Name = "Interest Transfer")]
        public bool interest_transfer { get; set; }
        [Column("interest_bank_id"), Display(Name = "Interest Transfer Bank")]
        public int? interest_bank_id { get; set; }
        [ForeignKey("interest_bank_id")]
        public virtual Bank Banks { get; set; }
        [Column("invoiceable"), Display(Name = "Invoiceable")]
        public bool invoiceable { get; set; }
        [Column("renewable"), Display(Name = "Renewable")]
        public bool renewable { get; set; }
        [Column("renewal_basis"), Display(Name = "Renewal Basis")]
        [StringLength(50)]
        public string renewal_basis { get; set; }
        [Column("invoice_status"), Display(Name = "Invoice Status")]
        public int invoice_status { get; set; }
        [Column("entry_date")]
        public DateTime entry_date { get; set; }
        [Column("entry_user")]
        [StringLength(30)]
        public string entry_user { get; set; }
        [Column("relationship_manager"), Display(Name = "Relationship Manager")]
        [StringLength(30)]
        [Required]
        public string relationship_manager { get; set; }
        [Column("insurer_branch"), Display(Name = "Insurance Branch")]
        [StringLength(30)]
        public string insurer_branch { get; set; }
        [Column("payment_mode"), Display(Name = "Payment Mode")]
        [StringLength(30)]
        [Required]
        public string payment_mode { get; set; }
        [Column("receipt_no"), Display(Name = "Receipt Number")]
        [StringLength(20)]
        public string receipt_no { get; set; }
        [Column("payment_date"), Display(Name = "Payment Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? payment_date { get; set; }
        [Column("client_code"), Display(Name = "Insurance Client Code")]
        [StringLength(20)]
        public string client_code { get; set; }
        [Column("contract_path")]
        public string contract_path { get; set; }
        [Column("guid")]
        public string guid { get; set; }
        [Column("renewed"), Display(Name = "Renewed")]
        public bool renewed { get; set; }
        [Column("renewal_policy_id")]
        [Display(Name = "Renewal Policy ID")]
        public int? renewal_policy_id { get; set; }

        public string BankOptionalName => Banks?.BankName ?? "--";

        public string PartnerOptionalName => Partners?.company_short_name ?? "--";

        public string ProductOptionalName => InsuranceProducts?.product_name ?? "--";

        public string ClientOptionalName => Clients?.client_name ?? "--";

        public string ClientOptionalMobile =>  Clients?.mobile ?? "";
    }
}
