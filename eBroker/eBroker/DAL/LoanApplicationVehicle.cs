using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("LoanApplication_Vehicle")]
    public class LoanApplication_Vehicle
    {
        [NotMapped]
        public string TableName { get { return "LoanApplication_Vehicle"; } }
        [Column("UID")]
        [Key]
        public int Id { get; set; }
        [Column("loan_application_id")]
        public int loan_application_id { get; set; }
        [ForeignKey("loan_application_id")]
        public virtual LoanApplication LoanApplications { get; set; }
        [Column("plate_no"), Display(Name = "Plate Number")]
        [StringLength(20)]
        public string plate_no { get; set; }
        [Column("make"), Display(Name = "Make")]
        [StringLength(50)]
        public string make { get; set; }
        [Column("model"), Display(Name = "Model")]
        [StringLength(50)]
        public string model { get; set; }
        [Column("class_id"), Display(Name = "Type")]
        public int class_id { get; set; }
        [Column("chassis_no"), Display(Name = "Chassis Number")]
        [StringLength(50)]
        public string chassis_no { get; set; }
        [Column("manufacture_year"), Display(Name = "Year of Manufacture")]
        public int manufacture_year { get; set; }
        [Column("value"), Display(Name = "Value")]
        public double value { get; set; }
        [Column("vehicle_usage_id"), Display(Name = "Vehicle Usage")]
        public int vehicle_usage_id { get; set; }
        [ForeignKey("vehicle_usage_id")]
        public virtual Vehicle_Usage VehicleUsages { get; set; }
        [Column("number_occupants"), Display(Name = "Occupants")]
        public int number_occupants { get; set; }
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
