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
    public class ModulesController : Controller
    {
        private ModelDB db = new ModelDB();
        private const int pageSize = 10;
        // GET: Modules
        public ActionResult Index(int?page)
        {
           
            int totalRecords;
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
           
            var dataModule = db.Database.SqlQuery<Module>("sp_Module_Paging @pageNum,@pageSize,@totalRecords out", parameter).ToList();
            var Output = parameter.FirstOrDefault(x => x.ParameterName == "@totalRecords");
            totalRecords = int.Parse(Output.Value.ToString());
            ViewBag.Total = totalRecords;
            return View(dataModule);
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
        // GET: Modules/Details/5
        public ActionResult Details(int?page,int? id,string name)
        {
            Session["ID"] = id;
            Session["ModuleName"] = name;
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

            var dataModuleDetail = db.Database.SqlQuery<Feature>("sp_Feature_GetById @id,@pageNum,@pageSize,@totalRecords out", parameter).ToList();
            var Output = parameter.FirstOrDefault(x => x.ParameterName == "@totalRecords");
            totalRecords = int.Parse(Output.Value.ToString());
            ViewBag.Total = totalRecords;
            return View(dataModuleDetail);
        }
        public ActionResult DetailsModule(int?id)
        {
            Module module = db.Modules.Find(id);
            return PartialView("_DetailsModule",module);

        }
        // GET: Modules/Create
        public ActionResult Create()
        {
          return PartialView("_Create");
        }   
        [HttpPost]
        [ValidateAntiForgeryToken] // Ngăn chặn các yêu cầu giả
        public ActionResult Create( Module module)
        {
            if (ModelState.IsValid) // kiểm lỗi các tham số của model
            {
                var model = new StoredProcedures();
                int result = model.ModuleInsert(module.ModuleName, module.ModuleDescription);
                if (result > 0) 
                {
                    SetAlert("Add item Module Success", "success");
                }
                else
                {
                   SetAlert("Add item Module Fail: Module Name Has Exists", "error");
                }
               
            }
            return RedirectToAction("Index");
        }
        // GET: Modules/Edit/5
        public ActionResult Edit(int? id)
        {
        
            Module module = db.Modules.Find(id);
            return PartialView("_Edit",module);
        }    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Module module)
        {
            if (ModelState.IsValid)
            {
                var model = new StoredProcedures();
                int result = model.ModuleUpate(module.ID,module.ModuleName, module.ModuleDescription);
                if (result > 0)
                {
                    SetAlert("Update item Module Success", "success");
                }
                else
                {
                    SetAlert("Update item Module Fail", "error");
                }
               
            }
            return RedirectToAction("Index");
        }
        // GET: Modules/Delete/5
        public ActionResult Delete(int?id)
        {
            Module module = db.Modules.Find(id);
            return PartialView("_Delete",module);
        }
        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id,Module module)
        {
                var model = new StoredProcedures();
                int result = model.ModuleDelete(module.ID);
                if (result > 0)
                {
                    SetAlert("Delete item Module Success", "success");
                }
                else
                {
                    SetAlert("Delete item Module Fail", "error");
                }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult AddModule(int? page, int? id,int?selected=0)
        {
            Session["ProjectId"] = id;
            int totalRecords = 0;
            int pageNum = page ?? 1;
            ViewBag.Page = pageNum;
            ViewBag.PageSize = pageSize;
            var parameter = new SqlParameter[]
            {
                new SqlParameter("@selected", selected),
                new SqlParameter("@pageNum", pageNum),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@totalRecords", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };
            var dataFeature = db.Database.SqlQuery<Module>("sp_Module_GetByIdNull @selected,@pageNum,@pageSize,@totalRecords out", parameter).ToList();
            var Output = parameter.FirstOrDefault(x => x.ParameterName == "@totalRecords");
            totalRecords = int.Parse(Output.Value.ToString());
            ViewBag.Total = totalRecords;
            return PartialView("_AddModule", dataFeature);
        }
        public ActionResult ModuleProject(int? page, int ?id)
        {
            Session["ProjectId"] = id;
            int totalRecords = 0;
            int pageNum = page ?? 1;
            ViewBag.Page = pageNum;
            ViewBag.PageSize = pageSize;
            var parameter = new SqlParameter[]
            {
                new SqlParameter("@id",id),
                new SqlParameter("@pageNum", pageNum),
                new SqlParameter("@pageSize", pageSize),
                new SqlParameter("@totalRecords", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };
            var dataFeature = db.Database.SqlQuery<Module>("sp_Module_GetById @id,@pageNum,@pageSize,@totalRecords out", parameter).ToList();
            var Output = parameter.FirstOrDefault(x => x.ParameterName == "@totalRecords");
            totalRecords = int.Parse(Output.Value.ToString());
            ViewBag.Total = totalRecords;
            return PartialView("_ModuleProject", dataFeature);
        }
        [HttpPost]
        public ActionResult ModuleProject( int? page, int[] ids,int id,Module module)
        {
           var model = new StoredProcedures();
           int pageNumber = page ?? 1;
            if (ids != null)
            {              
                for (int i = 0; i < ids.Length; i++)
                {
                     int idModule = ids[i];
                     int result = model.ModuleProject(idModule,module.ProjectId=id);
                }
                SetAlert("Add Module in Project Success", "success");
                     
            }
            else
            {
                SetAlert("Add Module in Project Fail", "error");
            }
            return  RedirectToAction("Details", "Projects", new { id = @Session["ProjectId"], name = @Session["ProjectName"] });
            
        }
       
        [HttpPost]
        public ActionResult DropProject(int id, int? page, int[] ids,Module module)
        {
            var model = new StoredProcedures();
            int pageNumber = page ?? 1;         
            if (ids != null)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    int idModule = ids[i];
                    int result = model.DropProject(idModule,module.ProjectId=id);             
            
                }
                SetAlert("Delete Module in Project Success", "success");
            }
            else
            {
                SetAlert("Delete Module in Project Fail", "error");
            }
            return RedirectToAction("Details", "Projects", new { id = @Session["ProjectId"], name = @Session["ProjectName"] });
            
        }
                
    }
}
