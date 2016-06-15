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
using System.IO;

namespace PM_ASVN.Controllers
{
    public class ItemController : BaseController
    {
        // GET: Item
        public ActionResult ManageItem(ItemModel model)
        {
            PM_ASVN.Common.Item item = model.Data;
            if (Request.HttpMethod.ToUpper() == "POST")
            {
                switch (model.Action)
                {
                    case "Save":
                        {
                            // edit item
                            if (item.ID > 0)
                            {
                                PM_ASVN.Common.Item itemEdited = new Common.Item();
                                itemEdited.ID = model.Data.ID;
                                itemEdited.Get();

                                itemEdited.Name = model.Data.Name;
                                if(model.Data.Description == null)
                                {
                                    itemEdited.Description = "";
                                }
                                else
                                {
                                    itemEdited.Description = model.Data.Description;
                                }
                                itemEdited.CreateDate = DateTime.Now;
                                itemEdited.Update();

                                if (model.Data.Type == (int)Types.Project)
                                {
                                    model.ProjectData.ID = itemEdited.ID;
                                    ManageProject(model);
                                }
                                if (model.Data.Type == (int)Types.TestCase)
                                {
                                    model.TestCaseData.ID = itemEdited.ID;
                                    ManageTestCase(model);
                                }
                                if (model.Data.Type == (int)Types.Task)
                                {
                                    model.TaskData.ID = itemEdited.ID;
                                    ManageTask(model);
                                }
                            }
                            // add item and item relation
                            else
                            {
                                item.Insert();
                                if (model.ParentId > 0)
                                {
                                    PM_ASVN.Common.ItemRelation itemRelation = new Common.ItemRelation();
                                    itemRelation.IDChild = item.ID;
                                    itemRelation.IDParent = model.ParentId;
                                    itemRelation.Description = "";
                                    itemRelation.Insert();
                                }
                                if (model.Data.Type == (int)Types.WorkItem)
                                {
                                    ManageWorkItem(model);
                                }
                                if (model.Data.Type == (int)Types.TestCase)
                                {
                                    ManageTestCase(model);
                                }
                                if (model.Data.Type == (int)Types.Project)
                                {
                                    ManageProject(model);
                                }
                                if (model.Data.Type == (int)Types.Task)
                                {
                                    ManageTask(model);
                                }
                            }
                        }
                        return Redirect(model.ReturnUrl);

                    case "Remove":
                        {
                            if (item.ID > 0)
                            {
                                RemoveItem(item.ID);
                                if (model.Data.Type == (int)Types.Project)
                                {
                                    ManageProject(model);
                                }
                                if (model.Data.Type == (int)Types.TestCase)
                                {
                                    ManageTestCase(model);
                                }

                            }
                            return Redirect(Session["removeUrl"].ToString());
                        }
                }
            }
            else
            {
                // get item (edit)
                if (item.ID > 0)
                {
                    // get url for delete
                    if (ControllerContext.HttpContext.Request.UrlReferrer != null)
                    {
                        string preID = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["Data.ID"];
                        string currentID = Request.QueryString["Data.ID"];
                        if (preID != currentID)
                            Session["removeUrl"] = ControllerContext.HttpContext.Request.UrlReferrer.ToString();
                    }
                    else
                    {
                        Session["removeUrl"] = "/Home/Index";
                    }

                    PM_ASVN.Common.Item itemSelected = new Common.Item();
                    itemSelected.ID = item.ID;
                    itemSelected.Get();
                    model.Data.Name = itemSelected.Name;
                    model.Data.Description = itemSelected.Description;

                    //create breadcrumb
                    if (itemSelected.Type == (int)Types.Project | itemSelected.Type == (int)Types.Project)
                    {
                        string ID = "";
                        if (ControllerContext.HttpContext.Request.UrlReferrer != null)
                        {
                            ID = Request.QueryString["Data.ID"];
                            itemSelected.ID = Convert.ToInt32(ID);
                            itemSelected.Get();
                            Session["Breadcrumb"] = itemSelected.Name;
                        }
                    }
                    if (itemSelected.Type != (int)Types.Project && itemSelected.Type != (int)Types.Project)
                    {
                        string ID = "";
                        if (ControllerContext.HttpContext.Request.UrlReferrer != null)
                        {
                            ID = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["Data.ID"];
                            itemSelected.ID = Convert.ToInt32(ID);
                            itemSelected.Get();
                            Session["Breadcrumb"] = itemSelected.Name;
                        }
                        ViewBag.Breadcrumb = Session["Breadcrumb"];
                    }
                }
                if (model.Data.Type == (int)Types.Project)
                {

                    ManageProject(model);

                }
                if (model.Data.Type == (int)Types.Ticket)
                {
                    Session["IDTicket"] = model.Data.ID;

                }

                List<PM_ASVN.Common.Item> listFeatureInTicket = new List<Common.Item>();
                listFeatureInTicket = item.GetDatabaseInProject(Convert.ToInt32(Session["IDTicket"]), (int)Types.Feature);

                if (listFeatureInTicket != null)
                {
                    Session["IDFeature"] = listFeatureInTicket[0].ID;
                }
                List<PM_ASVN.Common.Item> listDatabaseInProject = new List<Common.Item>();
                listDatabaseInProject = item.GetDatabaseInProject(Convert.ToInt32(Session["IDProject"]), (int)Types.Database);

                if (listDatabaseInProject != null)
                {
                    Session["IDDatabase"] = listDatabaseInProject[0].ID;
                }
                else
                {
                    Session["IDDatabase"] = "";
                }
               

            }

            return View(model);
        }
        public ActionResult ManageFile(ItemModel model)
        {
            // get file attach in project
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/App_Data/Document/"));

            Item itemProvider = new Item();
            List<Item> lstFile = new List<Item>();
            lstFile = itemProvider.GetFileInProject(model.Data.ID, (int)Types.AttachFile);
            if (lstFile != null)
            {
                model.ListFile = lstFile;
            }
            else
            {
                model.ListFile = new List<Item>();
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file, ItemModel model)
        {

            if (file.ContentLength > 0)
            {
                var dirPath = Server.MapPath("~/App_Data/Document/" + model.Data.ID);
                if(!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var fileName = Path.GetFileName(file.FileName);
                
                Item itemProvider = new Item();
                itemProvider.Name = fileName;
                itemProvider.Type = (int)Types.AttachFile;
                itemProvider.Description = Path.GetExtension(fileName);
                itemProvider.CreateDate = DateTime.Now;
                itemProvider.Insert();

                fileName = itemProvider.ID.ToString() + Path.GetExtension(fileName);
                var filePath = Path.Combine(Server.MapPath("~/App_Data/Document/" + model.Data.ID), fileName);
                file.SaveAs(filePath);
                
                ItemRelation itemRelation = new ItemRelation();
                itemRelation.IDChild = itemProvider.ID;
                itemRelation.IDParent = model.Data.ID;
                itemRelation.Description = "File in Project";
                itemRelation.Insert();


            }

            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public FileResult DownloadFile(int ID, int IDFile)
        {
            Item itemProvider = new Item();
            itemProvider.ID = IDFile;
            itemProvider.Get();

            string fileName = IDFile.ToString() + itemProvider.Description;
            var filepath = System.IO.Path.Combine(Server.MapPath("/App_Data/Document/" + ID), fileName);
            return File(filepath, MimeMapping.GetMimeMapping(filepath), itemProvider.Name);
        }
        [HttpPost]
        public string DeleteFile(ItemModel model)
        {

            Item itemProvider = new Item();
            itemProvider.ID = model.Data.ID;
            itemProvider.Get();


            string fullPath = Request.MapPath("/App_Data/Document/" + model.ProjectData.ID + "/" + model.Data.ID + itemProvider.Description);
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                RemoveItem(model.Data.ID);
                return "OK";
            }
            else
            {
                RemoveItem(model.Data.ID);
                return "ER";
            }

        }
        [HttpPost]
        public string SaveItemContinue(ItemModel model)
        {
            PM_ASVN.Common.Item item = model.Data;
            item.Insert();
            if (model.ParentId > 0)
            {
                PM_ASVN.Common.ItemRelation itemRelation = new Common.ItemRelation();
                itemRelation.IDChild = item.ID;
                itemRelation.IDParent = model.ParentId;
                itemRelation.Description = "";
                itemRelation.Insert();
            }
            return "OK";

        }
        protected void RemoveItem(int ID)
        {
            PM_ASVN.Common.Item item = new Common.Item();
            item.RemoveItem(ID);
        }
        // return list item by type, parentid
        public ActionResult ChildList(ListItemModel model)
        {
            List<PM_ASVN.Common.Item> listItem = new List<PM_ASVN.Common.Item>();
            int typeInt = (int)model.Type;
            if (model.FilterParentId == 0)
            {
                if (model.PageIndex == 0)
                    model.PageIndex = 1;
                model.PageSize = Int32.Parse(ConfigurationManager.AppSettings["PageSize"] ?? "10");

                aperia.core.business.ObjectParameter op = new aperia.core.business.ObjectParameter();
                int TotalRow = 0;
                PM_ASVN.Common.Item itemProvider = new PM_ASVN.Common.Item();
                listItem = itemProvider.GetChildList(ref op, model.PageIndex, model.PageSize, typeInt, model.ParentId, TotalRow);
                model.TotalRows = Convert.ToInt32(op["TotalRow"]);

                if ((model.TotalRows % model.PageSize) > 0)
                {
                    model.TotalPage = (model.TotalRows / model.PageSize) + 1;
                }
                else
                {
                    model.TotalPage = (model.TotalRows / model.PageSize);
                }

                model.List = listItem;

            }

            if (model.Edit == true)
            {
                model.Remove = false;
            }
            else
            {
                model.Remove = true;
                model.Description = new List<string>();
                if (listItem != null)
                {
                    for (int i = 0; i < listItem.Count; i++)
                    {
                        int ID = listItem[i].ID;
                        PM_ASVN.Common.ItemRelation itemRelation = new Common.ItemRelation();
                        itemRelation.IDChild = ID;
                        itemRelation.IDParent = model.ParentId;
                        itemRelation.Get();

                        model.Description.Add(itemRelation.Description);
                    }
                }
            }
            return View(model);
        }
        // return list child item by type , not in parent id
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

                        if (itemProvider.Type == (int)Types.Feature | itemProvider.Type == (int)Types.BackendJob | itemProvider.Type == (int)Types.Component)
                        {
                            List<PM_ASVN.Common.Item> listDatabaseInProject = new List<Common.Item>();
                            listDatabaseInProject = itemProvider.GetDatabaseInProject(Convert.ToInt32(Session["IDProject"]), (int)Types.Database);

                            if (lstItem == null)
                            {
                                lstItem = new List<Common.Item>();

                            }
                            if (listDatabaseInProject != null)
                            {
                                foreach (var itemDatabase in listDatabaseInProject)
                                {
                                    lstItem.Add(new PM_ASVN.Common.Item { ID = itemDatabase.ID, Name = itemDatabase.Name, Type = (int)Types.Database });
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
                            // Add new item estimation
                            itemEstimaton.Name = "Estimation - " + itemParent.ID;
                            itemEstimaton.Type = (int)Types.Estimation;
                            itemEstimaton.Insert();

                            // Add detail estimation
                            Estimation estimationDetail = new Estimation();
                            estimationDetail.ID = itemEstimaton.ID;
                            estimationDetail.DateCreate = DateTime.Today.ToString("MM/dd/yyyy");
                            if (item.Description == null)
                                estimationDetail.Description = "";
                            else
                                estimationDetail.Description = item.Description;
                            estimationDetail.Cost = 0;
                            estimationDetail.Insert();

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
        public ActionResult Temp()
        {
            return View();
        }
        // Project Detail (Add/Update/Delete)
        public ActionResult ManageProject(ItemModel model)
        {
            if (Request.HttpMethod.ToUpper() == "POST")
            {
                switch (model.Action)
                {
                    case "Save":
                        {
                            PM_ASVN.Common.Project project = new Common.Project();
                            project.ID = model.Data.ID;
                            project.Get();

                            if (model.ProjectData.ID == 0) // not exists, add new 
                            {
                                model.ProjectData.ID = project.ID;
                                model.ProjectData.Insert();
                            }
                            else // exist, just edit
                            {
                                project = model.ProjectData;
                                project.Update();
                            }
                        }

                        break;
                    case "Remove":
                        {
                            PM_ASVN.Common.Project itemProvider = new Common.Project();
                            itemProvider.ID = model.Data.ID;
                            itemProvider.Delete();
                        }
                        break;
                }
                // reset
            }
            // here, request method is GET 
            else
            {
                if (model.Data.ID > 0)
                {
                    PM_ASVN.Common.Project current = new Common.Project();
                    current.ID = model.Data.ID;
                    current.Get();

                    model.ProjectData = new Common.Project();
                    model.ProjectData = current;

                    Session["IDProject"] = model.Data.ID;
                }
                Item itemProvider = new Item();
                List<Item> lstUser = new List<Item>();
                lstUser = itemProvider.GetListType((int)Types.User);
                ViewData["lstUser"] = lstUser;
                
            }
            return View(model);
        }
        public ActionResult ManageTask(ItemModel model)
        {
            Item itemProvider = new Item();
            List<Item> lstPriority = itemProvider.GetListType((int)Types.TCPriority);
            ViewData["lstPriority"] = lstPriority;

            List<Item> lstStatus = itemProvider.GetListType((int)Types.StatusTask);
            ViewData["lstStatus"] = lstStatus;
            if (Request.HttpMethod.ToUpper() == "POST")
            {
                switch (model.Action)
                {
                    case "Save":
                        {
                            
                            Task task = new Task();
                            task.ID = model.Data.ID;
                            task.Get();

                            if(model.TaskData.ID == 0)
                            {
                                model.TaskData.ID = task.ID;
                                model.TaskData.Insert();

                                ItemRelation itemRelation = new ItemRelation();
                                itemRelation.IDChild = model.Priority;
                                itemRelation.IDParent = model.TaskData.ID;
                                itemRelation.Description = "Task - Priority";
                                itemRelation.Insert();

                                itemRelation.IDChild = model.Status;
                                itemRelation.IDParent = model.TaskData.ID;
                                itemRelation.Description = "Task - Status";
                                itemRelation.Insert();
                            }
                            else
                            {
                                task = model.TaskData;
                                task.Update();

                                int Priority = itemProvider.GetTaskProperties(model.TaskData.ID, (int)Types.TCPriority).ID;
                                int Status = itemProvider.GetTaskProperties(model.TaskData.ID, (int)Types.StatusTask).ID;

                                ItemRelation itemRelation = new ItemRelation();
                                itemRelation.IDChild = Priority;
                                itemRelation.IDParent = model.TaskData.ID;
                                itemRelation.Delete();

                                itemRelation.IDChild = Status;
                                itemRelation.IDParent = model.TaskData.ID;
                                itemRelation.Delete();

                                itemRelation.IDChild = model.Priority;
                                itemRelation.IDParent = model.TaskData.ID;
                                itemRelation.Description = "Task - Priority";
                                itemRelation.Insert();

                                itemRelation.IDChild = model.Status;
                                itemRelation.IDParent = model.TaskData.ID;
                                itemRelation.Description = "Task - Status";
                                itemRelation.Insert();
                            }
                        }

                        break;
                    case "Remove":
                        {
                            Task taskProvider = new Task();
                            taskProvider.ID = model.TaskData.ID;
                            taskProvider.Delete();
                        }
                        break;
                }
                // reset
            }
            // here, request method is GET 
            else
            {
                if (model.Data.ID > 0)
                {

                    Task current = new Task();
                    current.ID = model.Data.ID;
                    current.Get();

                    model.TaskData = new Task();
                    model.TaskData = current;

                    model.Priority = itemProvider.GetTaskProperties(model.Data.ID, (int)Types.TCPriority).ID;
                    model.Status = itemProvider.GetTaskProperties(model.Data.ID, (int)Types.StatusTask).ID;

                }
                List<Item> lstUser = new List<Item>();
                lstUser = itemProvider.GetListType((int)Types.User);
                ViewData["lstUser"] = lstUser;

            }
            return View(model);
        }
        public ActionResult ManageTestCase(ItemModel model)
        {
            Item itemProvider = new Item();
            List<Item> lstTypeTest = itemProvider.GetListType((int)Types.TypeTestCase);
            ViewData["lstTypeTest"] = lstTypeTest;

            List<Item> lstPriority = itemProvider.GetListType((int)Types.TCPriority);
            ViewData["lstPriority"] = lstPriority;

            List<Item> lstStatus = itemProvider.GetListType((int)Types.StatusTestCase);
            ViewData["lstStatus"] = lstStatus;

            if (Request.HttpMethod.ToUpper() == "POST")
            {
                switch (model.Action)
                {
                    case "Save":
                        {
                            TestCase testCase = new TestCase();
                            testCase.ID = model.Data.ID;
                            testCase.Get();

                            if (model.TestCaseData.ID == 0)
                            {
                                model.TestCaseData.ID = testCase.ID;

                                if (model.TestCaseData.TestDate == DateTime.Parse("01/01/0001"))
                                {
                                    model.TestCaseData.TestDate = DateTime.Parse("01/01/2000");
                                }

                                model.TestCaseData.Insert();


                                ItemRelation itemRelation = new ItemRelation();
                                itemRelation.IDChild = model.TestType;
                                itemRelation.IDParent = model.TestCaseData.ID;
                                itemRelation.Description = "TestCase - Type";
                                itemRelation.Insert();

                                itemRelation.IDChild = model.Priority;
                                itemRelation.IDParent = model.TestCaseData.ID;
                                itemRelation.Description = "TestCase - Priority";
                                itemRelation.Insert();

                            }
                            else
                            {
                                testCase = model.TestCaseData;
                                if (model.TestCaseData.TCExpectedResult == null)
                                {
                                    testCase.TCExpectedResult = "";
                                }
                                testCase.Update();

                                //get type testcase

                                int TestType = itemProvider.GetTestCaseProperties(model.TestCaseData.ID, (int)Types.TypeTestCase).ID;
                                int Priority = itemProvider.GetTestCaseProperties(model.TestCaseData.ID, (int)Types.TCPriority).ID;
                                int Status = itemProvider.GetTestCaseProperties(model.TestCaseData.ID, (int)Types.StatusTestCase).ID;

                                ItemRelation itemRelation = new ItemRelation();
                                itemRelation.IDChild = TestType;
                                itemRelation.IDParent = model.TestCaseData.ID;
                                itemRelation.Delete();

                                itemRelation.IDChild = Priority;
                                itemRelation.IDParent = model.TestCaseData.ID;
                                itemRelation.Delete();

                                itemRelation.IDChild = Status;
                                itemRelation.IDParent = model.TestCaseData.ID;
                                itemRelation.Delete();

                                itemRelation.IDChild = model.TestType;
                                itemRelation.IDParent = model.TestCaseData.ID;
                                itemRelation.Description = "TestCase - Type";
                                itemRelation.Insert();

                                itemRelation.IDChild = model.Priority;
                                itemRelation.IDParent = model.TestCaseData.ID;
                                itemRelation.Description = "TestCase - Priority";
                                itemRelation.Insert();

                                itemRelation.IDChild = model.Status;
                                itemRelation.IDParent = model.TestCaseData.ID;
                                itemRelation.Description = "TestCase - Status";
                                itemRelation.Insert();


                            }
                        }

                        break;
                    case "Remove":
                        {
                            TestCase testCaseProvider = new TestCase();
                            testCaseProvider.ID = model.TestCaseData.ID;
                            testCaseProvider.Delete();

                        }
                        break;
                }
            }
            // here, request method is GET 
            else
            {
                if (model.Data.ID > 0)
                {
                    TestCase current = new TestCase();
                    current.ID = model.Data.ID;
                    current.Get();

                    model.TestCaseData = new TestCase();
                    model.TestCaseData = current;

                    //get type & priority testcase
                    model.TestType = itemProvider.GetTestCaseProperties(current.ID, (int)Types.TypeTestCase).ID;
                    model.Priority = itemProvider.GetTestCaseProperties(current.ID, (int)Types.TCPriority).ID;
                    model.Status = itemProvider.GetTestCaseProperties(current.ID, (int)Types.StatusTestCase).ID;

                    // get step in testcase
                    StepInTestCase stepProvider = new StepInTestCase();
                    List<StepInTestCaseModel> lstStep = new List<StepInTestCaseModel>();
                    lstStep = stepProvider.GetStepInTestCase(model.Data.ID);
                    model.StepData = lstStep;

                }
                List<Item> lstUser = new List<Item>();
                lstUser = itemProvider.GetListType((int)Types.User);
                ViewData["lstUser"] = lstUser;
            }
            return View(model);
        }
        public ActionResult ManageUsersTestCase(ItemModel model, int[] lstUsersSelected, int[] lstBrowsersSelected, string Action)
        {
            Item item = new Item();
            List<Item> lstBrowsers = new List<Item>();
            lstBrowsers = item.GetListType((int)Types.Browser);
            if (Request.HttpMethod.ToUpper() == "GET")
            {

                List<Item> lstUsers = new List<Item>();
                lstUsers = item.GetUserNotInTestCase(model.Data.ID, (int)Types.User);

                List<Item> lstUserInTestCase = new List<Item>();
                lstUserInTestCase = item.GetUserInTestCase(model.Data.ID, (int)Types.User);

                List<TestCaseModel> lstTestCase = new List<TestCaseModel>();
                if (lstUserInTestCase != null)
                {
                    for (int i = 0; i < lstUserInTestCase.Count; i++)
                    {
                        TestCaseModel testCaseModel = new TestCaseModel();
                        testCaseModel.ID = lstUserInTestCase[i].ID;
                        testCaseModel.Name = lstUserInTestCase[i].Name;
                        testCaseModel.Browsers = new List<bool>();
                        for (int j = 0; j < lstBrowsers.Count; j++)
                        {
                            TestCaseRelation testCaseRelation = new TestCaseRelation();
                            List<TestCaseRelation> lstTestCaseRelation = testCaseRelation.CheckUserInBrowser(lstUserInTestCase[i].ID, lstBrowsers[j].ID, model.Data.ID);
                            if (lstTestCaseRelation != null)
                                testCaseModel.Browsers.Add(true);
                            else
                                testCaseModel.Browsers.Add(false);
                        }
                        lstTestCase.Add(testCaseModel);
                    }
                }

                ViewData["lstUsers"] = lstUsers;
                ViewData["lstBrowsers"] = lstBrowsers;
                return View(lstTestCase);
            }
            else
            {
                switch (Action)
                {
                    case "Show":
                        {
                            TestCaseModel testCaseModel = new TestCaseModel();
                            testCaseModel.Browsers = new List<bool>();
                            for (int j = 0; j < lstBrowsers.Count; j++)
                            {
                                TestCaseRelation testCaseRelation = new TestCaseRelation();
                                List<TestCaseRelation> lstTestCaseRelation = testCaseRelation.CheckUserInBrowser(lstUsersSelected[0], lstBrowsers[j].ID, model.Data.ID);
                                if (lstTestCaseRelation != null)
                                    testCaseModel.Browsers.Add(true);
                                else
                                    testCaseModel.Browsers.Add(false);
                            }
                            var JsonResult = new
                            {
                                result = testCaseModel,
                            };
                            return Json(JsonResult);
                        };
                    case "Add":
                        {
                            int IDTestCase = model.Data.ID;
                            if (lstUsersSelected != null)
                            {
                                for (int i = 0; i < lstUsersSelected.Count(); i++)
                                {
                                    for (int j = 0; j < lstBrowsers.Count(); j++)
                                    {
                                        TestCaseRelation testCaseRelation = new TestCaseRelation();
                                        testCaseRelation.ID_TestCase = IDTestCase;
                                        testCaseRelation.ID_Browser = lstBrowsers[j].ID;
                                        testCaseRelation.ID_User = lstUsersSelected[i];
                                        testCaseRelation.Insert();
                                    }
                                }
                                var JsonResult = new
                                {
                                    result = "OK",
                                };
                                return Json(JsonResult);

                            }
                        }; break;
                    case "Remove":
                        {
                            for (int i = 0; i < lstBrowsers.Count; i++)
                            {
                                TestCaseRelation testCaseRelation = new TestCaseRelation();
                                testCaseRelation.ID_TestCase = model.Data.ID;
                                testCaseRelation.ID_Browser = lstBrowsers[i].ID;
                                testCaseRelation.ID_User = lstUsersSelected[0];
                                testCaseRelation.Delete();
                            }
                            var JsonResult = new
                            {
                                result = "OK",
                            };
                            return Json(JsonResult);
                        };
                    case "Edit":
                        {
                            if (lstBrowsersSelected != null)
                            {
                                for (int i = 0; i < lstBrowsers.Count; i++)
                                {
                                    TestCaseRelation testCaseRelation = new TestCaseRelation();
                                    testCaseRelation.ID_TestCase = model.Data.ID;
                                    testCaseRelation.ID_Browser = lstBrowsers[i].ID;
                                    testCaseRelation.ID_User = lstUsersSelected[0];
                                    testCaseRelation.Delete();
                                }
                                for (int i = 0; i < lstBrowsersSelected.Count(); i++)
                                {
                                    TestCaseRelation testCaseRelation = new TestCaseRelation();
                                    testCaseRelation.ID_TestCase = model.Data.ID;
                                    testCaseRelation.ID_Browser = lstBrowsers[lstBrowsersSelected[i]].ID;
                                    testCaseRelation.ID_User = lstUsersSelected[0];
                                    testCaseRelation.Insert();
                                }
                                var JsonResult = new
                                {
                                    result = "OK",
                                };
                                return Json(JsonResult);
                            }

                        } break;
                }
                return View();
            }
        }
        public ActionResult ManageComment(ItemModel model, Comment comment)
        {
            Comment commentProvider = new Comment();
            List<CommentModel> lstComment = new List<CommentModel>();
            if (Request.HttpMethod.ToUpper() == "GET")
            {

                lstComment = commentProvider.GetListComment(model.Data.ID);
                return View(lstComment);
            }
            else
            {
                comment.IDUser = 1749;
                comment.CreateDate = DateTime.Now;
                comment.Insert();
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
            
        }
        [HttpPost]
        public string ManageStepInTestCase(StepInTestCaseModel step, int IDTestCase, string Action)
        {
            StepInTestCase StepProvider = new StepInTestCase();
            Item itemProvider = new Item();
            ItemRelation itemRelation = new ItemRelation();
            if (Action == "Save")
            {

                if (step.ID == 0)
                {
                    itemProvider.Type = (int)Types.StepInTestCase;
                    itemProvider.Name = step.Name;
                    itemProvider.Description = "";
                    itemProvider.Insert();

                    itemRelation.IDChild = itemProvider.ID;
                    itemRelation.IDParent = IDTestCase;
                    itemRelation.Description = "";
                    itemRelation.Insert();

                    StepInTestCase stepTest = new StepInTestCase();
                    stepTest.ID = itemProvider.ID;
                    if (step.ExpectedResult == null)
                        stepTest.ExpectedResult = "";
                    else
                        stepTest.ExpectedResult = step.ExpectedResult;

                    if (step.TestData == null)
                        stepTest.TestData = "";
                    else
                        stepTest.TestData = step.TestData;
                    stepTest.Insert();

                    return itemProvider.ID.ToString();
                }
                else
                {
                    itemProvider.ID = step.ID;
                    itemProvider.Get();
                    itemProvider.Name = step.Name;
                    itemProvider.Description = "Test Step";
                    itemProvider.Update();

                    StepInTestCase stepTest = new StepInTestCase();
                    stepTest.ID = step.ID;
                    stepTest.Get();

                    if (step.ExpectedResult == null)
                        stepTest.ExpectedResult = "";
                    else
                        stepTest.ExpectedResult = step.ExpectedResult;

                    if (step.TestData == null)
                        stepTest.TestData = "";
                    else
                        stepTest.TestData = step.TestData;
                    stepTest.Update();

                    return "OK";
                }
            }
            else
            {
                RemoveItem(step.ID);
                StepInTestCase stepProvider = new StepInTestCase();
                stepProvider.ID = step.ID;
                stepProvider.Delete();
                return "OK";
            }


        }
        public void ManageWorkItem(ItemModel model)
        {
            PM_ASVN.Common.WorkItem workItem = new Common.WorkItem();
            workItem.ID = model.Data.ID;
            workItem.Get();
            workItem.Status = 0;
            workItem.Insert();
        }
        public ActionResult RemoveItemRelation(int ID, int ParentID)
        {
            PM_ASVN.Common.ItemRelation itemRelationProvider = new Common.ItemRelation();
            itemRelationProvider.IDChild = ID;
            itemRelationProvider.IDParent = ParentID;
            itemRelationProvider.Delete();

            Item itemProvider = new Item();
            Item itemChild = new Item();
            Item itemParent = new Item();

            itemChild.ID = ID;
            itemChild.Get();

            itemParent.ID = ParentID;
            itemParent.Get();

            if (itemParent.Type == (int)Types.Ticket && itemChild.Type == (int)Types.Feature)
            {
                itemProvider.RemoveFeatureInTicket(ID, ParentID);
            }

            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult ParentList(ListItemModel model)
        {
            List<PM_ASVN.Common.Item> listItem = new List<PM_ASVN.Common.Item>();
            int typeInt = (int)model.Type;
            if (model.ChildId > 0)
            {
                if (model.PageIndex == 0)
                    model.PageIndex = 1;
                model.PageSize = Int32.Parse(ConfigurationManager.AppSettings["PageSize"] ?? "10");

                aperia.core.business.ObjectParameter op = new aperia.core.business.ObjectParameter();
                int TotalRow = 0;
                PM_ASVN.Common.Item itemProvider = new PM_ASVN.Common.Item();
                listItem = itemProvider.GetParentList(ref op, model.PageIndex, model.PageSize, typeInt, model.ChildId, TotalRow);
                model.TotalRows = Convert.ToInt32(op["TotalRow"]);

                if ((model.TotalRows % model.PageSize) > 0)
                {
                    model.TotalPage = (model.TotalRows / model.PageSize) + 1;
                }
                else
                {
                    model.TotalPage = (model.TotalRows / model.PageSize);
                }

                model.List = listItem;

            }

            return View(model);
        }
        public ActionResult EstimationDetail(EstimationModel model)
        {
            if (Request.HttpMethod.ToUpper() == "GET")
            {
                if (model.ID != 0)
                {
                    // get list workgroup
                    WorkGroup workGroupProvider = new WorkGroup();
                    List<WorkGroup> lstWorkGroup = workGroupProvider.GetListWorkGroup();
                    ViewData["lstWorkGroup"] = lstWorkGroup;

                    // get detail list workgroup
                    Estimation estimationProvider = new Estimation();
                    List<EstimationDetail> lstEstimation = new List<EstimationDetail>();
                    lstEstimation = estimationProvider.GetEstimateDetail(model.ID);
                    return View(lstEstimation);
                }

            }
            return Redirect("/Home/Index");

        }
        // send form from EstimationDetail with (Key, Value)
        // Form[0] ("IDEstimation", Value)
        // Form[1..n] (IDWorkGroup, Value)
        [HttpPost]
        public string UpdateTimeWorkGroup(FormCollection form)
        {
            int IDEstimation = Convert.ToInt32(form[0]);
            Item itemProvider = new Item();
            for (int i = 1; i < form.AllKeys.Count(); i++)
            {
                ItemRelation itemRelation = new ItemRelation();
                itemRelation.IDChild = Convert.ToInt32(form.AllKeys[i]);
                itemRelation.IDParent = IDEstimation;
                itemRelation.Get();
                if (form[i] != "")
                    itemRelation.Description = form[i];
                else
                    itemRelation.Description = "0";
                itemRelation.Update();
            }
            return "OK";
        }
        // send form from EstimationDetail with (Key, Value)
        // Form[0] ("IDEstimation", Value)
        // Form[1..n] (IDWorkItem, Value)
        [HttpPost]
        public string UpdateTimeWorkItem(FormCollection form)
        {
            int IDEstimation = Convert.ToInt32(form[0]);
            for (int i = 1; i < form.AllKeys.Count(); i++)
            {
                ItemRelation itemRelation = new ItemRelation();
                itemRelation.IDChild = Convert.ToInt32(form.AllKeys[i]);
                itemRelation.IDParent = IDEstimation;
                itemRelation.Get();
                if (form[i] != "")
                    itemRelation.Description = form[i];
                else
                    itemRelation.Description = "0";
                itemRelation.Update();
            }
            return "OK";
        }
        [HttpPost]
        public ActionResult GetWorkItemInEstimationByWorkGroup(int IDEstimation, int IDWorkGroup)
        {
            aperia.core.business.ObjectParameter op = new aperia.core.business.ObjectParameter();
            PM_ASVN.Common.WorkItem workItemProvider = new PM_ASVN.Common.WorkItem();
            PM_ASVN.Common.WorkGroup workGroupProvider = new PM_ASVN.Common.WorkGroup();
            PM_ASVN.Common.Estimation estimationProvider = new PM_ASVN.Common.Estimation();

            // get detail workitem and real time
            double Temp = 0;
            List<WorkItemModel> listWorkItem = new List<WorkItemModel>();
            listWorkItem = workItemProvider.GetDetailWorkItem(ref op, IDEstimation, IDWorkGroup, (int)Types.WorkItem, Temp);
            double realTime = Convert.ToDouble(op["SumWG"]);

            // get expect time
            List<WorkGroup> lstWorkGroup = new List<WorkGroup>();
            lstWorkGroup = workGroupProvider.GetTimeWorkGroup(IDEstimation, IDWorkGroup, (int)Types.WorkGroup);

            // get description
            List<Estimation> lstEstimation = new List<Estimation>();
            lstEstimation = estimationProvider.GetDescriptionEstimation(IDEstimation);

            var JsonResult = new
            {
                lstWorkItem = listWorkItem.ToArray(),
                realTime = realTime,
                timePlan = lstWorkGroup[0].PercentGroup,
                description = lstEstimation[0].Description
            };
            return Json(JsonResult, JsonRequestBehavior.AllowGet);
        }
        [CustomAuthorizeAttribute(Permission = "Search_Project")]
        public ActionResult SearchProject(string query)
        {
            Item itemProvider = new Item();
            List<Item> lstProject = new List<Item>();
            lstProject = itemProvider.SearchProject(query, (int)Types.Project);
            if (lstProject != null)
            {
                if (lstProject.Count == 1)
                    return Redirect("/Project?Data.ID=" + lstProject[0].ID);
                else
                    return View(lstProject);
            }
            else
            {
                return View(lstProject);
            }
        }
        [HttpPost]
        public ActionResult GetUserAndBrowserInTestCase(int IDFeature, int IDTypeTest)
        {
            Item itemProvider = new Item();
            List<Item> lstTestCase = new List<Item>();
            lstTestCase = itemProvider.GetListTestCaseByFeatureAndTypeTest(IDFeature, IDTypeTest);

            List<Item> lstBrowsers = new List<Item>();
            lstBrowsers = itemProvider.GetListType((int)Types.Browser);

            List<Item> lstUsers = new List<Item>();
            lstUsers = itemProvider.GetListType((int)Types.User);

            List<TestCaseModel> lstResult = new List<TestCaseModel>();
            for (int i = 0; i < lstTestCase.Count; i++)
            {
                TestCaseModel testCaseModel = new TestCaseModel();
                testCaseModel.ID = lstTestCase[i].ID;
                testCaseModel.Name = lstTestCase[i].Name;
                testCaseModel.Users = new List<string>();
                for (int j = 0; j < lstBrowsers.Count; j++)
                {
                    List<Item> lstUser = new List<Item>();
                    lstUser = itemProvider.GetUsersByTestCaseAndBrowser(lstTestCase[i].ID, lstBrowsers[j].ID);
                    string userTemp = "";
                    if (lstUser != null)
                    {

                        for (int h = 0; h < lstUser.Count; h++)
                        {
                            userTemp += " " + lstUser[h].Name + " -";
                        }
                        if (userTemp.EndsWith("-"))
                        {
                            userTemp = userTemp.Remove(userTemp.Length - 1);
                        }
                        testCaseModel.Users.Add(userTemp);
                    }
                    else
                    {
                        testCaseModel.Users.Add(" ");
                    }

                }
                lstResult.Add(testCaseModel);
            }
            var JsonResult = new
            {
                IDFeature = IDFeature,
                IDTypeTest = IDTypeTest,
                lstUsers = lstUsers.ToArray(),
                lstResult = lstResult.ToArray(),
                lstBrowsers = lstBrowsers.ToArray(),
            };
            return Json(JsonResult);
        }
        [HttpPost]
        public ActionResult GetTestCaseAndBrowserByUser(int[] lstUsers, int IDFeature, int IDTypeTest)
        {
            List<ItemModel> lstResult = new List<ItemModel>();

            Item itemProvider = new Item();
            List<Item> lstBrowsers = new List<Item>();
            lstBrowsers = itemProvider.GetListType((int)Types.Browser);

            List<Item> lstAllUser = new List<Item>();
            lstAllUser = itemProvider.GetListType((int)Types.User);

            for (int k = 0; k < lstUsers.Count(); k++)
            {
                List<TestCaseModel> lstTestCaseFinal = new List<TestCaseModel>();

                List<Item> lstTestCase = new List<Item>();
                lstTestCase = itemProvider.GetListTestCaseByUser(lstUsers[k], IDFeature, IDTypeTest);
                if (lstTestCase != null)
                {
                    for (int i = 0; i < lstTestCase.Count; i++)
                    {
                        TestCaseModel testCaseModel = new TestCaseModel();
                        testCaseModel.ID = lstTestCase[i].ID;
                        testCaseModel.Name = lstTestCase[i].Name;
                        testCaseModel.Users = new List<string>();
                        for (int j = 0; j < lstBrowsers.Count; j++)
                        {
                            List<Item> lstUser = new List<Item>();
                            lstUser = itemProvider.GetUsersByTestCaseAndBrowser(lstTestCase[i].ID, lstBrowsers[j].ID);
                            if (lstUser != null)
                            {
                                bool flag = false;
                                for (int h = 0; h < lstUser.Count; h++)
                                {
                                    if (lstUser[h].ID == lstUsers[k])
                                    {
                                        testCaseModel.Users.Add(lstUser[h].Name);
                                        flag = true;
                                        break;
                                    }
                                }
                                if (flag == false)
                                {
                                    testCaseModel.Users.Add(" ");
                                }
                            }
                            else
                            {
                                testCaseModel.Users.Add(" ");
                            }

                        }
                        lstTestCaseFinal.Add(testCaseModel);
                    }
                    lstResult.Add(new ItemModel() { TestCaseModel = lstTestCaseFinal });
                }

            }

            var JsonResult = new
            {
                lstUsers = lstAllUser.ToArray(),
                lstResult = lstResult.ToArray(),
                lstBrowsers = lstBrowsers.ToArray(),
            };
            return Json(JsonResult);
        }
        public ActionResult PermissionInRole(int IDRole)
        {
            Session["Role_Id"] = IDRole;
            Permission permission = new Permission();
            List<Permission> lstPermission = new List<Permission>();
            lstPermission = permission.GetPermissionInRole(IDRole);
            return View(lstPermission);
        }

        public ActionResult ChildListPermissionInRole(int IDRole)
        {
            Session["Role_Id"] = IDRole;
            Permission permission = new Permission();
            List<Permission> lstPermission = new List<Permission>();
            lstPermission = permission.GetPermissionNotInRole(IDRole);
            return PartialView("ChildListPermissionInRole", lstPermission);
        }
        [HttpPost]
        public ActionResult AddPermissionInRole(int IDRole, int[] IDPermission)
        {
            var DAL = new Permission();
            if (IDPermission != null)
            {
                for (int i = 0; i < IDPermission.Length; i++)
                {
                    int idPermission = IDPermission[i];
                    DAL.AddPermissionInRole(IDRole, idPermission);
                }
                SetAlert("Add Permission Success", "success");
            }
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }
        public ActionResult DeletePermissionInRole(int IDRole, int IDPermission)
        {
            var DAL = new Permission();
            DAL.DeletePermissionInRole(IDRole, IDPermission);
            SetAlert("Delete Permission Success", "success");
            return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
        }

    }
}