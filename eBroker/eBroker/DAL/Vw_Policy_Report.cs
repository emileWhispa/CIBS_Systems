using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Vw_Policy_Report")]
    public class Vw_Policy_Report
    {
        [NotMapped]
        public string TableName { get { return "Vw_Policy_Report"; } }
        [Key]
        [Column("contract_id")]
        public int Id { get; set; }
        public string policy_no { get; set; }
        public string policy_type { get; set; }
        public DateTime effective_dt { get; set; }
        public DateTime expiry_dt { get; set; }
        public int? net_premium { get; set; }
        public string insurer { get; set; }
        public string product_name { get; set; }
        public string client_name { get; set; }
        public bool? renewable { get; set; }

        public bool renewed { get; set; }
        public int? renewal_policy_id { get; set; }
        public string BankName { get; set; }
        public bool? interest_transfer { get; set; }
    }
}
