using PM_ASVN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PM_ASVN.Controllers
{
    [CustomAuthorizeAttribute(Permission = "View_Role")]
    public class RoleController : BaseController
    {
        private Role Role = new Role();
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult List()
        {
            return View(Role.GetListRole());
        }
        public ActionResult Add()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult AddRole(Role model)
        {
            if (ModelState.IsValid)
            {
                if (Role.CheckRole(model.Name) > 0)
                {
                    SetAlert("Role Name has exists!", "error");
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
                else
                {
                    if (Role.AddRole(model.Name, model.Description) > 0)
                    {
                        SetAlert("Add New Role Success", "success");
                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int IDRole)
        {
            return PartialView(Role.GetRoleID(IDRole));
        }
        [HttpPost]
        public ActionResult Edit(Role model)
        {
            if (ModelState.IsValid)
            {
                Role.EditRole(model);
                SetAlert("Edit Role Success", "success");
            }
            return RedirectToAction("Index", model);
        }
        public ActionResult Delete(int IDRole)
        {
            if (Role.CheckRoleExists(IDRole) == 0)
            {
                SetAlert("Delete Role Fail,Please Check Role In Account", "error");
            }
            else
            {
                Role.DeleteRole(IDRole);
                SetAlert("Delete Role Success", "success");
            }
            return RedirectToAction("Index");
        }
    }
}