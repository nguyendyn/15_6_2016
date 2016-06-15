using PM_ASVN.Common;
using PM_ASVN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PM_ASVN.Controllers
{
    public class WorkItemController : Controller
    {
        // GET: WorkItem
        public ActionResult List(ListItemModel model)
        {
            List<PM_ASVN.Common.Item> listWorkItem = new List<PM_ASVN.Common.Item>();
            int typeInt = (int)model.Type;
            if (model.FilterParentId == 0)
            {
                PM_ASVN.Common.Item itemProvider = new PM_ASVN.Common.Item();
                listWorkItem = itemProvider.GetWorkItemInWorkGroup(model.ParentId, typeInt);
                model.List = listWorkItem;
            }

            return View(model);
        }
        public ActionResult RemoveWorkItem(int ID)
        {
            WorkItem workItem = new WorkItem();
            workItem.ID = ID;
            workItem.Get();
            workItem.Status = 1;
            workItem.Update();
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
    }
}