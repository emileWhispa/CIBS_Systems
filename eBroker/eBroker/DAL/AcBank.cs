using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace eBroker.DAL
{

    [Table("AcBank")]
    public class AcBank
    {
        [NotMapped]
        public string TableName => "AcBank";

        [Column("UID")]
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("BankName")]
        [Display(Name = "Bank Name")]
        [StringLength(100)]
        public string BankName { get; set; }
    }
}