using PM_ASVN.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PM_ASVN.Controllers
{
    [CustomAuthorizeAttribute(Permission = "View_Permission")]
    public class PermissionController : BaseController
    {
        private Permission permission = new Permission();
        private aperia.core.business.ObjectParameter op = new aperia.core.business.ObjectParameter();
        private Pagination pagination = new Pagination();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List(int? Page)
        {
            pagination.pageSize = Int32.Parse(ConfigurationManager.AppSettings["PageSize"] ?? "10");
            ViewBag.Link = "Index";
            pagination.pageNum = Page ?? 1;
            var data = permission.GetListPermission(ref op, pagination.pageNum, pagination.pageSize);
            var output = op.FirstOrDefault(x => x.Key == "totalRecords");
            pagination.totalRecords = (int)output.Value;
            ViewBag.Page = pagination.pageNum;
            ViewBag.PageSize = pagination.pageSize;
            ViewBag.Total = pagination.totalRecords;
            return View(data);
        }
        public ActionResult Add()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddPermission(Permission model)
        {
            if (ModelState.IsValid)
            {
                if (permission.CheckPermission(model.Name) > 0)
                {
                    SetAlert("Permission Name has exists!", "error");
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
                else
                {
                    if (permission.AddPermission(model.Name, model.Description) > 0)
                    {
                        SetAlert("Add New Permission Success", "success");
                        return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                    }
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int IDPermission)
        {
            return PartialView(permission.GetPermissionID(IDPermission));
        }
        [HttpPost]
        public ActionResult Edit(Permission model)
        {
            if (ModelState.IsValid)
            {
                permission.EditPermission(model);
                SetAlert("Edit Permission Success", "success");
            }
            return RedirectToAction("Index", model);
        }
        public ActionResult Delete(int IDPermission)
        {
            permission.DeletePermission(IDPermission);
            SetAlert("Delete Permission Success", "success");
            return RedirectToAction("Index");
        }
    }
}