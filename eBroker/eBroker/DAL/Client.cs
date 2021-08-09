using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Client")]
    public class Client
    {
        [NotMapped]
        public string TableName { get { return "Client"; } }
        [Column("client_id")]
        [Key]
        public int Id { get; set; }
        [Display(Name = "Client Name")]
        [Column("client_name")]
        [StringLength(100)]
        [Required]
        public string client_name { get; set; }

        [Display(Name = "National ID,TIN")]
        [Column("client_national_id")]
        [StringLength(100)]
        public string client_national_id { get; set; }
        [Display(Name = "Client Type")]
        [Column("client_type")]
        [StringLength(20)]
        [Required]
        public string client_type { get; set; }
        [Display(Name = "Contact Person")]
        [Column("contact_person")]
        [StringLength(50)]
        public string contact_person { get; set; }
        [Display(Name = "Physical Address")]
        [Column("physical_address")]
        [StringLength(50)]
        public string physical_address { get; set; }
        [Display(Name = "Primary Mobile Phone")]
        [Column("mobile")]
        [StringLength(30)]
        [Required]
        public string mobile { get; set; }
        [Display(Name = "Alternate Mobile Phone")]
        [Column("mobile2")]
        [StringLength(30)]
        public string mobile2 { get; set; }
        [Display(Name = "E-mail")]
        [Column("email")]
        [StringLength(100)]
        public string email { get; set; }
        [Display(Name = "Language")]
        [Column("language")]
        [StringLength(50)]
        [Required]
        public string language { get; set; }
        [Display(Name = "Recruited By")]
        [Column("recruited_by")]
        [StringLength(30)]
        [Required]
        public string recruited_by { get; set; }
        [Column("user_id")]
        [MaxLength(30)]
        public string user_id { get; set; }
        [Column("create_dt")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? create_dt { get; set; }

    }
}
