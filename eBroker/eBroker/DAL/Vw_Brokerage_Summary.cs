using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Vw_Brokerage_Summary")]
    public class Vw_Brokerage_Summary
    {
        [NotMapped]
        public string TableName { get { return "Vw_Brokerage_Summary"; } }
        [Key]
        public int? Id { get; set; }
        public int? CountPolicies { get; set; }
        public int? TotalAmountPolicies { get; set; }
        public int? CountBankPolicies { get; set; }
        public int? TotalAmountBankPolicies { get; set; }
        public int? CountCustomers { get; set; }
        public int? MotorPolicies { get; set; }
        public int? FirePolicies { get; set; }
        public int? PersonalPolicies { get; set; }
        public int? TravelPolicies { get; set; }
        public int? OtherPolicies { get; set; }
        public int? UnpaidInvoices { get; set; }
        public int? UnpaidInvoiceAmount { get; set; }
    }
}