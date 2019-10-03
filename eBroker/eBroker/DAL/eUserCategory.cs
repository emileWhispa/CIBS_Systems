using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("eUserCategories")]
    public class eUserCategory
    {
        [NotMapped]
        public string TableName { get { return "eUserCategories"; } }
        [NotMapped]
        public bool insert { get; set; }
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Display(Name = "User Category")]
        [Column("Category")]
        [Required()]
        [StringLength(50)]
        public string Category { get; set; }
        [Column("Description")]
        [StringLength(300)]
        public string Description { get; set; }
    }
}
