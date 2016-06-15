using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PM_ASVN.Common;
using PM_ASVN.Models;
using System.Web.Routing;

namespace PM_ASVN.Controllers
{
    public class BaseController : Controller
    {
        protected void SetAlert(string message, string type)
        {
            TempData["Message"] = message;
            if (type == "success")
            {
                TempData["Type"] = "success";

            }
            else if (type == "error")
            {
                TempData["Type"] = "error";
            }
        }

    }
}