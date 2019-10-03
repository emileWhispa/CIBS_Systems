using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("eCategoryMenus")]
    public class eCategoryMenu
    {
        [NotMapped]
        public string TableName { get { return "eCategoryMenus"; } }
        [Column("Id")]
        [Key]
        public int Id { get; set; }
        [Column("CategoryId")]
        [Display(Name = "User Group")]
        [Required()]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual eUserCategory eUserCategories { get; set; }
        [Column("MenuID")]
        [Display(Name = "Menu Name")]
        [Required()]
        public int MenuID { get; set; }
        [ForeignKey("MenuID")]
        public virtual Menu Menus { get; set; }
    }
}
