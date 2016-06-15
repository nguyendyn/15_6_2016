using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PM_ASVN.Models;

namespace PM_ASVN.Controllers
{
    public class BugController : Controller
    {
        // GET: Bug
        public ActionResult Index(ItemModel model)
        {
            if (model.Data == null) model.Data = new PM_ASVN.Common.Item();
            model.Data.Type = (int)Types.Bug;
            if (ControllerContext.HttpContext.Request.UrlReferrer != null)
                model.ReturnUrl = ControllerContext.HttpContext.Request.UrlReferrer.ToString();
            else
                model.ReturnUrl = "~/";
            string urlPage = HttpContext.Request.Url.AbsoluteUri;
            Session["urlPre"] = urlPage;
            return View(model);
        }

        public ActionResult ChildList(ListItemModel model)
        {
            model.Type = Types.Bug;
            model.Edit = true;
            return View(model);
        }
    }
}