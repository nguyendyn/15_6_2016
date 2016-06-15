using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PM_ASVN.Common;

namespace PM_ASVN.Controllers
{
    [CustomAuthorizeAttribute(Permission = "View_System")]
    public class SystemController : Controller
    {
        // GET: System
        public ActionResult Index()
        {
            return View();
        }
    }
}