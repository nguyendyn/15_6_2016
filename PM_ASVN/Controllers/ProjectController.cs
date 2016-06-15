using PM_ASVN.Common;
using PM_ASVN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PM_ASVN.Controllers
{
    [CustomAuthorize(Permission = "View_Project")]
    public class ProjectController : BaseController
    {
        public ActionResult Index(ItemModel model)
        {
            if (model.Data == null) model.Data = new PM_ASVN.Common.Item();
            model.Data.Type = (int)Types.Project;
            model.ReturnUrl = "/Project/List";
            string urlPage = HttpContext.Request.Url.AbsoluteUri;
            Session["urlPre"] = urlPage;

            return View(model);
        }
        public ActionResult List(ListItemModel model)
        {
            if (model.List == null)
                model.List = new List<PM_ASVN.Common.Item>();
            model.Type = Types.Project;
            model.Edit = true;
            return View(model);
        }
   

    }
}