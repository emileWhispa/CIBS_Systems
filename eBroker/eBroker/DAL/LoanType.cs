using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("LoanType")]
public class LoanType
{
[NotMapped]
public string TableName { get { return "LoanType"; } }
[Column("UID")]
[Key]
public int Id{get;set;}
[Column("loan_type")]
[StringLength(50)]
public string loan_type{get;set;}
}
}
