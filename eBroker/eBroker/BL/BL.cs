using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eBroker.Models;
using System.Configuration;
using securityComponents;

namespace eBroker.BL
{
    public class BL
    {
        public UserModel UserLogin(string userName, string password, out string message)
        {
            message = "";
            //------ Get User
            eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
            var user = dc.eUser.Where(x => x.Login == userName).FirstOrDefault();
            if (user == null)
            {
                message = "Invalid Username and/or Password!";
                return null;
            }

            //----- Validate User Password
            if (user.Password == securityComponents.Cryptography.Encrypt(password))
            {
                string companyName = "";

                if (user.CategoryId == 3)
                {
                    companyName = dc.Bank.First(e => e.Id == user.CompanyID)?.BankName;
                }
                else
                {
                    companyName = dc.Partner.First(e => e.Id == user.CompanyID)?.company_name;
                }
                
                UserModel userModel = new UserModel
                {
                    Id = user.Id,
                    Names = user.Names,
                    Login = user.Login,
                    Category = user.CategoryId.ToString(),
                    CompanyID = user.CompanyID,
                    CompanyName = companyName,
                    Password = user.Password,
                    Active = user.Active,
                    Locked = user.Locked
                };
                //----- Check user status
                if (user.Locked == true)
                {
                    message = "Your Account has been Locked. Contact Administrator";
                    return null;
                }
                //----- Check user status
                if (user.Active != true)
                {
                    message = "Your Account is not active. Contact Administrator";
                    return null;
                }
                return userModel;
            }
            else
            {
                message = "Incorrect Username and/or Password!";
                return null;
            }
        }

        public int expiredPolicy(string bankName)
        {
            eBroker.BrokerDataContext dc = new eBroker.BrokerDataContext(ConfigurationManager.ConnectionStrings["eBrokerageEntities"].ConnectionString);
            if (string.IsNullOrEmpty(bankName))
                return dc.Vw_Policy_Report.Where(x => x.expiry_dt == DateTime.Today).Count();
            else
                return dc.Vw_Policy_Report.Where(x => x.expiry_dt == DateTime.Today && x.BankName == bankName).Count();
        }
    }
}