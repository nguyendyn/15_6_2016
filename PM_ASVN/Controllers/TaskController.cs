using PM_ASVN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PM_ASVN.Controllers
{
    public class TaskController : Controller
    {
        // GET: Task
        public ActionResult Index(ItemModel model)
        {
            if (model.Data == null) model.Data = new PM_ASVN.Common.Item();
            model.Data.Type = (int)Types.Task;
            if (ControllerContext.HttpContext.Request.UrlReferrer != null)
                model.ReturnUrl = ControllerContext.HttpContext.Request.UrlReferrer.ToString();
            else
                model.ReturnUrl = "~/";
            string urlPage = HttpContext.Request.Url.AbsoluteUri;
            Session["urlPre"] = urlPage;
            return View(model);
        }
    }
}