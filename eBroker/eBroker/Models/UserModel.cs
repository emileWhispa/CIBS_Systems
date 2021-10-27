using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
namespace eBroker.Models
{
    [Table("eUser")]
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public string Login { get; set; }
        public string Names { get; set; }
        public string Category { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyID { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public bool Locked { get; set; }
    }
}