using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("GL_Account")]
    public class GL_Account
    {
        [NotMapped]
        public string TableName => "GL_Account";

        [Column("ID")]
        [Key]
        public int ID { get; set; }

        [Column("LedgerNo")]
        [Display(Name = "Ledger No")]
        public int LedgerNo { get; set; }

        [Column("LedgerDescription")]
        [StringLength(250)]
        [Display(Name = "Ledger Description")]
        public string LedgerDescription { get; set; }

        [Column("LedgerType")]
        [Display(Name = "Ledger Type")]

        public string LedgerType { get; set; }

        [Column("ParentLedgerNo")]
        [Display(Name = "Parent Ledger")]
        public int? ParentLedgerNo { get; set; }

        [Column("GL_Level")]
        [Display(Name = "GL Level")]
        public int GL_Level { get; set; }

        [Column("Active")]
        public bool Active { get; set; }


        [Column("Vision_GL")]
        [Display(Name = "Vision GL")]
        public string Vision_GL { get; set; }
    }
}