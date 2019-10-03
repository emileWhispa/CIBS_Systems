using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBroker
{
[Table("Profiles")]
public class Profile
{
[NotMapped]
public string TableName { get { return "Profiles"; } }
[Column("ProfileId")]
[Key]
public int Id{get;set;}
[Column("ProfileCode")]
[Required()]
public int ProfileCode{get;set;}
[Column("ProfileName")]
[Required()]
[StringLength(20)]
public string ProfileName{get;set;}
[Column("Description")]
[Required()]
[StringLength(50)]
public string Description{get;set;}
[Column("GroupMail")]
[StringLength(50)]
public string GroupMail{get;set;}
[Column("CreateDate")]
[Required()]
public DateTime CreateDate{get;set;}
[Column("StatusId")]
[Required()]
public int StatusId{get;set;}
}
}
