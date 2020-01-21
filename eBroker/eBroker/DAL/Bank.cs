using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
    [Table("Bank")]
public class Bank
{
[NotMapped]
public string TableName { get { return "Bank"; } }
[Column("Bank_ID")]
[Key]
public int Id{get;set;}
[Column("BankName")]
[StringLength(100)]
public string BankName{get;set;}
}
}
