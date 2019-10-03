using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("LoanApplication_Property")]
    public class LoanApplication_Property
    {
        [NotMapped]
        public string TableName { get { return "LoanApplication_Property"; } }
        [Column("UID")]
        [Key]
        public int Id { get; set; }
        [Column("loan_application_id")]
        public int loan_application_id { get; set; }
        [ForeignKey("loan_application_id")]
        public virtual LoanApplication LoanApplications { get; set; }
        [Column("property_description"), Display(Name = "Property Description")]
        [StringLength(50)]
        public string property_description { get; set; }
        [Column("plot_number"), Display(Name = "Plot Number")]
        [StringLength(20)]
        public string plot_number { get; set; }
        [Column("physical_address"), Display(Name = "Physical Address")]
        [StringLength(100)]
        public string physical_address { get; set; }
        [Column("wall_material"), Display(Name = "Wall Material")]
        [StringLength(50)]
        public string wall_material { get; set; }
        [Column("roof_material"), Display(Name = "Roof Material")]
        [StringLength(50)]
        public string roof_material { get; set; }
        [Column("window_material"), Display(Name = "Window Material")]
        [StringLength(50)]
        public string window_material { get; set; }
        [Column("property_use"), Display(Name = "Property Use")]
        [StringLength(50)]
        public string property_use { get; set; }
        [Column("floors"), Display(Name = "Floors")]
        public int floors { get; set; }
        [Column("insured_value"), Display(Name = "Insured Value")]
        public double insured_value { get; set; }
        [Column("entry_date")]
        public DateTime entry_date { get; set; }
        [Column("entry_user")]
        [StringLength(30)]
        public string entry_user { get; set; }
        [Column("PremiumAmount"), Display(Name = "Premium")]
        public int? PremiumAmount { get; set; }
        [Column("PremiumUser")]
        [StringLength(30)]
        public string PremiumUser { get; set; }
        [Column("PremiumDate")]
        public DateTime? PremiumDate { get; set; }
        [Column("Insurer_Id"), Display(Name = "Insurance Company")]
        public int? Insurer_Id { get; set; }
        [ForeignKey("Insurer_Id")]
        public virtual Partner Partners { get; set; }
    }
}
