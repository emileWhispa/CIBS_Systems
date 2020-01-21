using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using eBroker.Models;
using System.Threading;

namespace eBroker.Controllers
{
    public class BaseController : Controller
    {
        protected UserModel AppUserData = new UserModel();
        public Exception GetInnerException(Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            return ex;
        }


       
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            UserModel userModel = null;
            if (base.User is ClaimsPrincipal)
            {
                ClaimsIdentity claimsIdentity = base.User.Identity as ClaimsIdentity;
                Claim c = claimsIdentity.Claims.Where(x => x.Type == "userData").FirstOrDefault();
                if (c != null)
                {
                    string claim = c.Value;
                    if (!string.IsNullOrEmpty(claim))
                    {
                        userModel = JsonConvert.DeserializeObject<UserModel>(claim);
                    }
                }
                base.ViewData["UserData"] = (this.AppUserData ?? new UserModel());
                Claim p = claimsIdentity.Claims.Where(x => x.Type == "expiredPolicies").FirstOrDefault();
                if (p != null)
                {
                    string claimVal = p.Value;
                    if(!string.IsNullOrEmpty(claimVal))
                    {
                        base.ViewData["expiredPolicies"]=p.Value;
                    }
                }
                //base.ViewData["expiredPolicies"] = BaseController.GetClaim((base.User as ClaimsPrincipal).Claims.ToList<Claim>(), "expiredPolicies");
            }
            this.AppUserData = userModel;
            base.ViewData["UserData"] = (this.AppUserData ?? new UserModel());
            //Reading the expired Insurances

            if (userModel == null)
            {
                //string url = base.Url.Action("Login", "Account");
                //requestContext.HttpContext.Response.Redirect(url);
            }
        }

        public void InitializeForced(RequestContext requestContext)
        {
            this.Initialize(requestContext);
        }

        public static string GetClaim(List<Claim> claims, string key)
        {
            Claim claim = claims.FirstOrDefault((Claim c) => c.Type == key);
            if (claim == null)
            {
                return null;
            }
            return claim.Value;
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            Exception exception = filterContext.Exception;
            //Util.LogError("SAgent", exception);
            filterContext.ExceptionHandled = true;
            ViewResult result = base.View("Error", new HandleErrorInfo(exception, filterContext.RouteData.Values["controller"].ToString(), filterContext.RouteData.Values["action"].ToString()));
            filterContext.Result = result;
        }

        public void Success(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Success, message, dismissable, "fa fa-check");
        }

        public void Information(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Information, message, dismissable, "fa fa-info-circle");
        }

        public void Warning(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Warning, message, dismissable, "fa fa-warning");
        }

        public void Danger(string message, bool dismissable = true)
        {
            AddAlert(AlertStyles.Danger, message, dismissable, "fa fa-times-circle");
        }

        private void AddAlert(string alertStyle, string message, bool dismissable, string iconClass)
        {
            var alerts = TempData.ContainsKey(Alert.TempDataKey)
                ? (List<Alert>)TempData[Alert.TempDataKey]
                : new List<Alert>();

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Message = message,
                Dismissable = dismissable,
                IconClass = iconClass
            });

            TempData[Alert.TempDataKey] = alerts;
        }
    }
}