using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Menus")]
    public class Menu
    {
        [NotMapped]
        public string TableName { get { return "Menus"; } }
        [Key]
        [Column("MenuId")]
        public int Id { get; set; }

        [Column("MenuCode")]
        [Required()]
        public int MenuCode { get; set; }

        [Column("MenuName")]
        [Required()]
        [StringLength(20)]
        public string MenuName { get; set; }

        [Column("Url")]
        [Required()]
        [StringLength(100)]
        public string Url { get; set; }

        [Column("MenuLevel")]
        [Required()]
        public int MenuLevel { get; set; }

        [Column("ParentId")]
        [Required()]
        public int ParentId { get; set; }

        [Column("Active")]
        public bool Active { get; set; }

        [Column("OrderNo")]
        [Required()]
        public int OrderNo { get; set; }

        [Column("IconName")]
        [StringLength(100)]
        public string IconName { get; set; }
    }
}
