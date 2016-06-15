using PM_ASVN.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PM_ASVN.Controllers
{
    public class TableController : BaseController
    {
        // GET: Table
        public ActionResult Index(ItemModel model)
        {
            if (model.Data == null) model.Data = new PM_ASVN.Common.Item();
            model.Data.Type = (int)Types.Table;
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
            model.Type = Types.Table;
            model.Edit = true;
            return View(model);
        }
        public ActionResult SelectChild(ListItemModel model, SelectedItemModel selectedItems)
        {
            model.Type = Types.Table;
            return View(model);
        }
        
    }
}