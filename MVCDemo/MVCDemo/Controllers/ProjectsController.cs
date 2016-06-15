using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCDemo.Models;
using PagedList.Mvc;
using PagedList;
using System.Data.SqlClient;
using System.Web.UI.WebControls;


namespace MVCDemo.Controllers
{
    public class ProjectsController : Controller
    {
        private ModelDB db = new ModelDB();
        private const int pageSize=10;
        // GET: Projects
        
        public ActionResult Index(int?page)
        {
            int totalRecords = 0;
            int pageNum = page ?? 1;
            ViewBag.Page = pageNum;
            ViewBag.PageSize = pageSize;
            var parameter = new SqlParameter[]
            {
                new SqlParameter("@pageNum", pageNum),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@totalRecords", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };
            var dataProject = db.Database.SqlQuery<Project>("sp_Project_Paging @pageNum,@pageSize,@totalRecords out", parameter).ToList();
            var Output = parameter.FirstOrDefault(x => x.ParameterName == "@totalRecords");
            totalRecords = int.Parse(Output.Value.ToString());
            ViewBag.Total = totalRecords;
            return View(dataProject);
        }
        public ActionResult DetailsProject(int? id)
        {
            Project project = db.Projects.Find(id);
            return PartialView("_DetailsProject", project);

        }
        // GET: Projects/Details/5
        public ActionResult Details(int? id,int? page,string name)
        {
            Session["ID"] = id;
            Session["ProjectName"] = name;
            int totalRecords;
            int pageNum = page ?? 1;
            ViewBag.Page = pageNum;
            ViewBag.PageSize = pageSize;
            var parameter = new SqlParameter[]
            {   new SqlParameter("@id",id),
                new SqlParameter("@pageNum", pageNum),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@totalRecords", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };

            var dataProjectDetail = db.Database.SqlQuery<Module>("sp_Project_GetById @id,@pageNum,@pageSize,@totalRecords out", parameter).ToList();
            var Output = parameter.FirstOrDefault(x => x.ParameterName == "@totalRecords");
            totalRecords = int.Parse(Output.Value.ToString());
            ViewBag.Total = totalRecords;
            return View(dataProjectDetail);
        }
        // GET: Projects/Create
        public ActionResult Create()
        {
            return PartialView("_Create");
        }    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Project project)
        {
            var model = new StoredProcedures();
            if (ModelState.IsValid)
            {
                int result = model.ProjectCreate(project.ProjectName,project.ProjectDescription);
                if (result > 0)
                {
                    SetAlert("Add new item Project success", "success");
                }
                else
                {
                    SetAlert("Add new item Fail :  Project Name Has Exists", "error");
                }
            }
            return RedirectToAction("Index");
        }
        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            Project project = db.Projects.Find(id);
            return PartialView("_Edit",project);
        }      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Project project)
        {
            var model = new StoredProcedures();
            if (ModelState.IsValid)
            {
                int result = model.ProjectUpdate(project.ID, project.ProjectName, project.ProjectDescription,project.DateStart.Value);
                if (result>0)
                {
                    SetAlert("Update Item Success", "success");
                }
                else
                {
                    SetAlert("Update Item Fail", "error");
                }
            }
            return RedirectToAction("Index");
        }
        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {           
            Project project = db.Projects.Find(id);
            return PartialView("_Delete", project);
        }
        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Project project )
        {
            var model = new StoredProcedures();
            int result = model.ProjectDelete(project.ID);
            if (result > 0)
            {
                SetAlert("Delete Item Success", "success");
            }
            else
            {
                SetAlert("Delete Item Fail", "error");
            }
            return RedirectToAction("Index");
        }
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";

            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
