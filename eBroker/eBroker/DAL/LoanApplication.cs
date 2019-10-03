using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("LoanApplication")]
    public class LoanApplication
    {
        [NotMapped]
        public string TableName { get { return "LoanApplication"; } }
        [Column("ApplicationId")]
        [Required()]
        [Key]
        public int Id { get; set; }
        [Column("BankId")]
        [Required()]
        public int BankId { get; set; }
        [ForeignKey("BankId")]
        public virtual Bank Banks { get; set; }
        [Column("CustomerNames"), Display(Name = "Customer Name")]
        [Required()]
        [StringLength(200)]
        public string CustomerNames { get; set; }
        [Column("BirthDate"), Display(Name = "Date of Birth")]
        [Required()]
        public DateTime BirthDate { get; set; }
        [Column("PhoneNumber"), Display(Name = "Phone Number")]
        [Required()]
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        [Column("ApplicationAmount"), Display(Name = "Application Amount")]
        [Required()]
        public int ApplicationAmount { get; set; }
        [Column("Branch")]
        [Required()]
        [StringLength(50)]
        public string Branch { get; set; }
        [Column("ApplicationDate"), Display(Name = "Application Date")]
        [Required()]
        public DateTime ApplicationDate { get; set; }
        [Column("Term")]
        [Required()]
        public int Term { get; set; }
        [Column("RM"), Display(Name = "Relationship Manager")]
        [StringLength(50)]
        public string RM { get; set; }
        [Column("Status")]
        [StringLength(50)]
        public string Status { get; set; }
        [Column("UserId")]
        [StringLength(50)]
        public string User { get; set; }

        [Column("LifeInsuranceRequired"), Display(Name = "Life Insurance Required?")]
        public bool LifeInsuranceRequired { get; set; }
        [Column("CollateralType"), Display(Name = "Collateral Type")]
        [StringLength(30)]
        public string CollateralType { get; set; }
    }
}
