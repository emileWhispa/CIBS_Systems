using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Policy_Vehicle")]
    public class Policy_Vehicle
    {
        [NotMapped]
        public string TableName { get { return "Policy_Vehicle"; } }
        [Column("UID")]
        [Key]
        public int Id { get; set; }
        [Column("contract_id")]
        [Required]
        public int contract_id { get; set; }
        [ForeignKey("contract_id")]
        public virtual InsurancePolicy InsurancePolicies { get; set; }
        [Column("plate_no"), Display(Name = "Plate Number")]
        [Required]
        [StringLength(20)]
        public string plate_no { get; set; }
        [Column("make"), Display(Name = "Make")]
        [StringLength(50)]
        [Required]
        public string make { get; set; }
        [Column("model"), Display(Name = "Model")]
        [StringLength(50)]
        [Required]
        public string model { get; set; }
        [Column("class_id"), Display(Name = "Type")]
        [Required]
        public int class_id { get; set; }
        [Column("engine_no"), Display(Name = "Engine Number")]
        [StringLength(50)]
        public string engine_no { get; set; }
        [Column("chassis_no"), Display(Name = "Chassis Number")]
        [StringLength(50)]
        public string chassis_no { get; set; }
        [Column("manufacture_year"), Display(Name = "Year of Manufacture")]
        [Required]
        public int manufacture_year { get; set; }
        [Column("value"), Display(Name = "Value")]
        [Required]
        public double value { get; set; }
        [Column("product_id")]
        [Required]
        public int product_id { get; set; }
        [Column("vehicle_usage_id"), Display(Name = "Vehicle Usage")]
        [Required]
        public int vehicle_usage_id { get; set; }
        [ForeignKey("vehicle_usage_id")]
        public virtual Vehicle_Usage VehicleUsages { get; set; }
        [Column("territorial_cover_id"), Display(Name = "Territory Cover")]
        [Required]
        public int territorial_cover_id { get; set; }
        [Column("number_occupants"), Display(Name = "Occupants")]
        public int number_occupants { get; set; }
        [Column("occupant_id"), Display(Name = "Occupant Option")]
        public int occupant_id { get; set; }
        [Column("family_extension"), Display(Name = "Family Extension?")]
        public bool family_extension { get; set; }
        [Column("waive_excess"), Display(Name = "Wave Excess?")]
        public bool waive_excess { get; set; }
        [Column("entry_date")]
        public DateTime entry_date { get; set; }
        [Column("entry_user")]
        [StringLength(30)]
        public string entry_user { get; set; }
    }
}
