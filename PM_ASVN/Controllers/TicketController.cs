using PM_ASVN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

using System.Configuration;
using PM_ASVN.Common;


namespace PM_ASVN.Controllers
{
    public class TicketController : BaseController
    {
        // GET: Ticket
        public ActionResult Index(ItemModel model)
        {
            if (model.Data == null) model.Data = new PM_ASVN.Common.Item();
            model.Data.Type = (int)Types.Ticket;
            if (ControllerContext.HttpContext.Request.UrlReferrer != null)
                model.ReturnUrl = ControllerContext.HttpContext.Request.UrlReferrer.ToString();
            else
                model.ReturnUrl = "~/";
            string urlPage = HttpContext.Request.Url.AbsoluteUri;
            Session["urlPre"] = urlPage;
            return View(model);
        }
        public ActionResult SelectChild(ListItemModel model, SelectedItemModel selectedItems, string iframe)
        {

            if (Request.HttpMethod.ToUpper() == "GET")
            {
                PM_ASVN.Common.Item itemProvider = new Common.Item();
                int typeInt = (int)model.Type;
                List<PM_ASVN.Common.Item> item = new List<PM_ASVN.Common.Item>();
                if (model.ParentId == 0)
                {
                    //item = db.Items.Where(p => p.Type == typeInt).ToList();
                }
                else
                {
                    if (model.ParentId > 0 && model.FilterParentId > 0)
                    {

                        List<PM_ASVN.Common.Item> lstItem;
                        lstItem = itemProvider.GetChildByParentIDAndFilterID(typeInt, model.ParentId, model.FilterParentId);

                        itemProvider.ID = model.ParentId;
                        itemProvider.Get();

                        if (itemProvider.Type == (int)Types.Ticket)
                        {
                            List<PM_ASVN.Common.Item> lisFeatureInTicket = new List<Common.Item>();
                            lisFeatureInTicket = itemProvider.GetDatabaseInProject(Convert.ToInt32(Session["IDTicket"]), (int)Types.Feature);

                            if (lstItem == null)
                            {
                                lstItem = new List<Common.Item>();

                            }
                            if(lisFeatureInTicket != null)
                            {
                                foreach (var itemFeature in lisFeatureInTicket)
                                {
                                    lstItem.Add(new PM_ASVN.Common.Item { ID = itemFeature.ID, Name = itemFeature.Name, Type = (int)Types.Feature });
                                }
                            }
                        }

                        model.List = lstItem;

                    }
                    if (model.ParentId > 0 && model.FilterParentId == 0)
                    {

                        List<PM_ASVN.Common.Item> lstItem = new List<Common.Item>();
                        lstItem = itemProvider.GetChildNotInParentID(typeInt, model.ParentId);

                        model.List = lstItem;

                    }
                }
                return View(model);

            }
            else
            {
                foreach (var item in selectedItems.Items)
                {
                    if (item.IDChild != 0)
                    {
                        Item itemEstimaton = new Item();

                        var itemTemp = item;
                        Item itemChild = new Item();
                        itemChild.ID = itemTemp.IDChild;
                        itemChild.Get();

                        Item itemParent = new Item();
                        itemParent.ID = selectedItems.ParentID;
                        itemParent.Get();
                        // check itemChild is Feature and itemParent is Ticket?
                        if (itemChild.Type == (int)Types.Feature && itemParent.Type == (int)Types.Ticket)
                        {
                            // Add new estimation
                            itemEstimaton.Name = "Estimation - " + itemParent.ID;
                            itemEstimaton.Type = (int)Types.Estimation;
                            itemEstimaton.Insert();

                            // Featurre in Ticket
                            PM_ASVN.Common.ItemRelation itemRelation = new Common.ItemRelation();
                            itemRelation.IDChild = item.IDChild;
                            itemRelation.IDParent = selectedItems.ParentID;
                            if (item.Description == null)
                            {
                                item.Description = "";
                            }
                            itemRelation.Description = item.Description;
                            itemRelation.Insert();

                            // Estimation in Ticket
                            itemRelation.IDChild = itemEstimaton.ID;
                            itemRelation.IDParent = selectedItems.ParentID;
                            if (item.Description == null)
                            {
                                item.Description = "";
                            }
                            itemRelation.Description = item.Description;
                            itemRelation.Insert();

                            // Estimation in Feature
                            itemRelation.IDChild = itemEstimaton.ID;
                            itemRelation.IDParent = item.IDChild;
                            if (item.Description == null)
                            {
                                item.Description = "";
                            }
                            itemRelation.Description = item.Description;
                            itemRelation.Insert();

                            // WorkGroup in Estimation
                            WorkGroup workGroupProvider = new WorkGroup();
                            List<WorkGroup> lstWorkGroup = workGroupProvider.GetListWorkGroup();
                            for (int i = 0; i < lstWorkGroup.Count; i++)
                            {
                                itemRelation.IDChild = lstWorkGroup[i].ID;
                                itemRelation.IDParent = itemEstimaton.ID;
                                itemRelation.Description = "0.0";
                                itemRelation.Insert();

                            }
                            // WorkItem in Estimation
                            WorkItem workItemProvider = new WorkItem();
                            List<WorkItem> lstWorkItem = workItemProvider.GetListWorkItem();
                            for (int i = 0; i < lstWorkItem.Count; i++)
                            {
                                if (lstWorkItem[i].Status == 0)
                                {
                                    itemRelation.IDChild = lstWorkItem[i].ID;
                                    itemRelation.IDParent = itemEstimaton.ID;
                                    itemRelation.Description = "0.0";
                                    itemRelation.Insert();
                                }
                            }

                        }
                        else
                        {
                            PM_ASVN.Common.ItemRelation itemRelation = new Common.ItemRelation();
                            itemRelation.IDChild = item.IDChild;
                            itemRelation.IDParent = selectedItems.ParentID;
                            if (item.Description == null)
                            {
                                item.Description = "";
                            }
                            itemRelation.Description = item.Description;
                            itemRelation.Insert();
                        }
                    }
                }
            }


            if (iframe == "false")
                return Redirect(Session["urlPre"].ToString());
            else
            {
                return Redirect("/Item/Temp");
            }
        }

        public ActionResult Config()
        {
            return View();
        }

    }
}