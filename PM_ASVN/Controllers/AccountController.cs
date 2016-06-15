using PM_ASVN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PM_ASVN.Common;
using System.Configuration;
namespace PM_ASVN.Controllers
{
    [CustomAuthorizeAttribute(Permission = "View_Account")]
    public class AccountController : BaseController
    {
        private Account Account = new Account();
        private Role Role = new Role();
        private Permission permission = new Permission();
        private Pagination pagination = new Pagination();
        private aperia.core.business.ObjectParameter op = new aperia.core.business.ObjectParameter();
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
            var data = Account.GetListAccount(ref op, pagination.pageNum, pagination.pageSize);
            var output = op.FirstOrDefault(x => x.Key == "totalRecords");
            pagination.totalRecords = (int)output.Value;
            ViewBag.Page = pagination.pageNum;
            ViewBag.PageSize = pagination.pageSize;
            ViewBag.Total = pagination.totalRecords;
            return View(data);
        }
        [CustomAuthorizeAttribute(Permission = "ListPermission_Account")]
        public ActionResult ListPermission(int IDRole)
        {
            return PartialView(permission.GetPermissionInRole(IDRole));
        }
        [CustomAuthorizeAttribute(Permission = "Add_Account")]
        public ActionResult Add()
        {
            ViewBag.IDRole = new SelectList(Role.GetListRole(), "ID", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult AddAccount(Account model, int IDRole)
        {
            if (ModelState.IsValid)
            {
                if (Account.CheckAccount(model.Username) > 0)
                {
                    SetAlert("Username has exists!", "error");
                    return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
                }
                else
                {
                    var pass = HashPassword.MD5Hash(model.Password);
                    if (Account.AddAccount(model.Username, pass, IDRole) > 0)
                    {
                        SetAlert("Add New Account Success", "success");
                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("Index");
        }
        [CustomAuthorizeAttribute(Permission = "Edit_Account")]
        public ActionResult Edit(AccountModel model)
        {
            var selectedRole = Account.GetRoleInAccount(model.ID);
            Session["IDRole"] = selectedRole;
            ViewBag.IDRole = new SelectList(Role.GetListRole(), "ID", "Name", selectedRole);
            if (model.ID > 0)
            {
                return View(Account.GetAccountID(model.ID));
            }
            return Redirect("/Home/Index");
        }
        [HttpPost]
        public ActionResult EditAccount(int IDAccount, string Username, int IDRole)
        {
            if (ModelState.IsValid)
            {
                Account.EditAccount(IDAccount, Username, IDRole);
                SetAlert("Update Account Success", "success");
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult DeleteAccount(int IDAccount)
        {
            if (ModelState.IsValid)
            {
                Account.DeleteAccount(IDAccount);
                SetAlert("Delete Account Success", "success");
            }
            return RedirectToAction("Index");
        }
    }
}