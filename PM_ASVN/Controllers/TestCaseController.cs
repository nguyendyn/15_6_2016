using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PM_ASVN.Models;
using PM_ASVN.Common;

namespace PM_ASVN.Controllers
{
    public class TestCaseController : Controller
    {
        //
        // GET: /TestCase/
        public ActionResult Index(ItemModel model)
        {
            if (model.Data == null) model.Data = new PM_ASVN.Common.Item();
            model.Data.Type = (int)Types.TestCase;
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
            model.Type = Types.TestCase;
            model.Edit = true;
            return View(model);
        }
        public ActionResult SelectChild(ListItemModel model, SelectedItemModel selectedItems)
        {
            model.Type = Types.TestCase;
            return View(model);
        }
        public ActionResult Statistics(ItemModel model)
        {
            Item itemProvider = new Item();
            List<Item> lstFeature = new List<Item>();
            lstFeature = itemProvider.GetFeatureInProject(model.Data.ID, (int)Types.Feature);

            List<Item> lstType = new List<Item>();
            lstType = itemProvider.GetListType((int)Types.TypeTestCase);

            ViewData["lstType"] = lstType;
            List<TestCaseModel> lstFeatureModel = new List<TestCaseModel>();
            if (lstFeature != null && lstType != null)
            {
                for (int i = 0; i < lstFeature.Count; i++)
                {
                    TestCaseModel testCaseModel = new TestCaseModel();
                    testCaseModel.ID = lstFeature[i].ID;
                    testCaseModel.Feature = lstFeature[i].Name;
                    testCaseModel.TotalType = new List<TypeTestCase>();
                    for (int j = 0; j < lstType.Count; j++)
                    {
                        List<TypeTestCase> typeTestCase = new List<TypeTestCase>();
                        typeTestCase = itemProvider.CountTestCaseInType(lstFeature[i].ID, lstType[j].ID);
                        if (typeTestCase != null)
                        {
                            testCaseModel.TotalType.Add(typeTestCase[0]);
                        }
                        else
                        {
                            TypeTestCase typeTestCaseTemp = new TypeTestCase();
                            typeTestCaseTemp.ID = 0;
                            typeTestCaseTemp.TotalTC = 0;
                            typeTestCaseTemp.Name = "N/A";
                            testCaseModel.TotalType.Add(typeTestCaseTemp);
                        }
                    }
                    lstFeatureModel.Add(testCaseModel);
                }
            }
            return View(lstFeatureModel);
        }
        public ActionResult TypeTestCase(ItemModel model)
        {
            if (model.Data == null) model.Data = new PM_ASVN.Common.Item();
            model.Data.Type = (int)Types.TypeTestCase;
            if (ControllerContext.HttpContext.Request.UrlReferrer != null)
                model.ReturnUrl = ControllerContext.HttpContext.Request.UrlReferrer.ToString();
            else
                model.ReturnUrl = "~/";
            string urlPage = HttpContext.Request.Url.AbsoluteUri;
            Session["urlPre"] = urlPage;
            return View(model);
        }
        public ActionResult PriorityTestCase(ItemModel model)
        {
            if (model.Data == null) model.Data = new PM_ASVN.Common.Item();
            model.Data.Type = (int)Types.TCPriority;
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