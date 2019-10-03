using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace eBroker
{
    public class AntiForgeryTokenFilter : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception.GetType() == typeof(HttpAntiForgeryException))
            {
                filterContext.Result = new RedirectResult("~/Account/Login"); // whatever the url that you want to redirect to
                 
                filterContext.ExceptionHandled = true;
            }
        }
    }
}