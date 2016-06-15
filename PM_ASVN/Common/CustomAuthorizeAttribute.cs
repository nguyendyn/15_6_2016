using aperia.core.business;
using PM_ASVN.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PM_ASVN.Common
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public string Permission { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var session = (AccountModel)HttpContext.Current.Session[SessionAccount.ACCOUNT_SESSION];
            if (session != null)
            {
                 if (session.Permission.Exists(p => p.Name == Permission))
                {
                    return true;
                }
            }
            return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
             filterContext.Controller.TempData["Message"] = "Login Required";
            filterContext.Controller.TempData["Type"] = "error";
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary 
                { 
                    { "controller", "Login" }, 
                    { "action", "Index" } 
                });
        }
    }
}