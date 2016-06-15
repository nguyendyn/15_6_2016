using PM_ASVN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PM_ASVN.Controllers
{
    public class WebpageController : BaseController
    {
        //DatabaseEntities db = new DatabaseEntities();
        // GET: Webpage
        public ActionResult AddWebpage()
        {
            return View();
        }
        
    }
}