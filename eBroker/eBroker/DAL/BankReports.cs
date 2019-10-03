using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("BankReports")]
    public class BankReports
    {
        [NotMapped]
        public string TableName { get { return "BankReports"; } }
        [Key]
        [Column("ReportId")]
        public int Id { get; set; }
        [Column("ReportName")]
        public string ReportName { get; set; }
        [Column("Datasource")]
        public string Datasource { get; set; }
        [Column("ShowReport")]
        public bool ShowReport { get; set; }
        [Column("SortBy")]
        public string SortBy { get; set; }
        [Column("ExcelColumnList")]
        public string ExcelColumnList { get; set; }
        [Column("SearchFilter")]
        public string SearchFilter { get; set; }

        [Column("SearchFilterDescription")]
        public string SearchFilterDescription { get; set; }
        [Column("DateFilter")]
        public bool DateFilter { get; set; }
        [Column("DateFilterDescription")]
        public string DateFilterDescription { get; set; }
        [Column("DateFilterColumn")]
        public string DateFilterColumn { get; set; }

    }

}
