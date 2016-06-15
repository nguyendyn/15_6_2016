using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PM_ASVN.Common;
using PM_ASVN.Models;

namespace PM_ASVN.Controllers
{
   
    public class LoginController : BaseController
    {
        private Permission permission = new Permission();
        // GET: Login
        public ActionResult Index()
        {
            var session = (AccountModel)Session[SessionAccount.ACCOUNT_SESSION];
            if (session != null)
            {
                SetAlert("Your account has been logged", "error");
                return RedirectToAction("Index", "Home");
            }
            if (Request.Cookies["UserName"] != null && Request.Cookies["Password"] != null)
            {
                Account model = new Account();
                model.Username = Request.Cookies["UserName"].Value;
                model.Password = Request.Cookies["Password"].Value;
                return View(model);
            }
            return View();
        }
        public ActionResult Login(Account model)
        {
            if (ModelState.IsValid)
            {
                var DAL = new Account();
                var result = DAL.Login(model.Username, HashPassword.MD5Hash(model.Password));
                if (result == 1)
                {
                    var account=DAL.GetAccountByName(model.Username);
                    var sessionAccount= new AccountModel();
                    var lstPermission=permission.GetPermissionInRole(account.IDRole);
                    sessionAccount.Username =account.Username;
                    sessionAccount.ID = account.ID;
                    sessionAccount.Name = account.Name;
                    sessionAccount.Permission = lstPermission;
                    Session.Add(SessionAccount.ACCOUNT_SESSION, sessionAccount);
	    if (model.RememberMe)
                    {
                        Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(30);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(30);
                    }
                    else
                    {
                        Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);

                    }
                    Response.Cookies["UserName"].Value = model.Username;
                    Response.Cookies["Password"].Value = model.Password;

	    if (lstPermission == null)
                    {
                        SetAlert("You don't have permissions to access this Application ! Please contact administrator", "error");
                        return RedirectToAction("Logout");
                    }
                    if (account.Name.Equals("Admin"))
                    {
                        return RedirectToAction("Index", "Account");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    SetAlert("Username or Password is not correct", "error"); 
                }
            }
            return View("Index");
        }
        public ActionResult Logout()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}