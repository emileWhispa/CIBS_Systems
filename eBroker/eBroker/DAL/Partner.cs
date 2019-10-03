using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Partner")]
    public class Partner
    {
        [NotMapped]
        public string TableName { get { return "Partner"; } }
        [Column("company_code")]
        [Key]
        public int Id { get; set; }
        [Column("company_name")]
        [Display(Name = "Company Name")]
        [Required()]
        [StringLength(200)]
        public string company_name { get; set; }
        [Column("company_short_name")]
        [StringLength(100)]
        [Display(Name = "Company Short Name")]
        [Required()]
        public string company_short_name { get; set; }
        [Required]
        [Column("partnership_type")]
        [StringLength(15)]
        [Display(Name = "Partnership Type")]
        public string partnership_type { get; set; }
        [Column("contact_person")]
        [StringLength(50)]
        [Display(Name = "Contact Person")]
        [Required]
        public string contact_person { get; set; }
        [Required]
        [Column("mobile1")]
        [StringLength(15)]
        [Display(Name = "Mobile Line 1")]
        public string mobile1 { get; set; }
        [Column("mobile2")]
        [StringLength(15)]
        [Display(Name = "Mobile Line 2")]
        public string mobile2 { get; set; }
        [Column("group_email")]
        [StringLength(50)]
        [Display(Name = "Group E-mail")]
        public string group_email { get; set; }
        [Column("physical_address")]
        [Display(Name = "Physical Address")]
        [StringLength(100)]
        public string physical_address { get; set; }
        [Column("TIN")]
        [Display(Name = "TIN (9 Chars. Max)")]
        [MaxLength(9)]
        public string TIN { get; set; }
        [Column("user_id")]
        [MaxLength(30)]
        public string user_id { get; set; }
        [Column("create_dt")]
        public DateTime? create_dt { get; set; }
    }
}
