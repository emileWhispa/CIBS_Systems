using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("eUser")]
    public class eUser
    {
        [NotMapped]
        public string TableName { get { return "eUser"; } }
        [Key]
        public int Id { get; set; }
        [Column("Login")]
        [Display(Name = "Login")]
        [Required()]
        [StringLength(20)]
        public string Login { get; set; }
        [Column("Names")]
        [Display(Name = "Full Names")]
        [Required]
        [StringLength(50)]
        public string Names { get; set; }
        [Column("Category")]
        [Display(Name = "Category")]
        public string Category { get; set; }
        [Column("CategoryId")]
        [Display(Name = "User Profile")]
        [Required()]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual eUserCategory eUserCategories { get; set; }
        [Column("CompanyID")]
        [Display(Name = "Company")]
        public int? CompanyID { get; set; }
        [Display(Name = "Bank")]
        [NotMapped]
        public int? BankID { get; set; }
        [Column("Phone")]
        [Display(Name = "Mobile Phone")]
        [Required()]
        [StringLength(15)]
        public string Phone { get; set; }
        [Column("Email")]
        [Display(Name = "E-mail")]
        [Required()]
        [StringLength(50)]
        public string Email { get; set; }
        [Column("Password")]
        [Required()]
        [StringLength(50)]
        public string Password { get; set; }
        [Column("Active")]
        [Required()]
        public bool Active { get; set; }
        [Column("CreatedOn")]
        [Required()]
        public DateTime CreatedOn { get; set; }
        [Column("CreatedBy")]
        [StringLength(20)]
        public string CreatedBy { get; set; }
        [Column("ChangePassword")]
        public bool? ChangePassword { get; set; }
        [Column("Locked")]
        public bool Locked { get; set; }
        [Column("PwdChangeDate")]
        public DateTime? PwdChangeDate { get; set; }
        [Column("LastLogin")]
        public DateTime? LastLogin { get; set; }
        [Column("Attempts")]
        public int? Attempts { get; set; }
    }
}
