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
namespace MVCDemo.Controllers
{
    public class FeaturesController : Controller
    {
        private ModelDB db = new ModelDB();
        private const int pageSize = 10;
        // GET: Features
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
            var dataFeature = db.Database.SqlQuery<Feature>("sp_Feature_Paging @pageNum,@pageSize,@totalRecords out", parameter).ToList();
            var Output = parameter.FirstOrDefault(x => x.ParameterName == "@totalRecords");
            totalRecords = int.Parse(Output.Value.ToString());
            ViewBag.Total = totalRecords;
            return View(dataFeature);
        }
       
        protected void SetAlert(string message, string type)
        {
            TempData["AlertMessage"] = message;
            if (type == "success")
            {
                TempData["AlertType"] = "alert-success";

            }
            else if (type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }
        }
        public ActionResult DetailsFeature(int?id)
        {

            Feature feature = db.Features.Find(id);
            return PartialView("_DetailsFeature", feature);
        }
        public ActionResult Details(int?id)
        {

            Feature feature = db.Features.Find(id);
            return View(feature);
        }
        // GET: Features/Create
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        // POST: Features/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Feature feature)
        {
            var model = new StoredProcedures();
            if (ModelState.IsValid)
            {
                int result = model.FeatureInsert(feature.FeatureName, feature.FeatureDescription);
                if (result > 0) 
                { 
                    SetAlert("Add item Feature Success", "success");
                }
                else
                {
                    SetAlert("Add item Feature Fail : Feature Name Has Exists", "error");
                }
            }
            return RedirectToAction("Index");
        }
        // GET: Features/Edit/5
        public ActionResult Edit(int? id)
        {
            Feature feature = db.Features.Find(id);
            return PartialView("_Edit",feature);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Feature feature)
        {
            var model = new StoredProcedures();
            if (ModelState.IsValid)
            {
                int result = model.FeatureUpdate(feature.ID, feature.FeatureName, feature.FeatureDescription);
                if (result > 0)
                {
                    SetAlert("Update item Feature Success", "success");
                }
                else
                {
                    SetAlert("Update item Feature Fail", "error");
                }
            }
            return RedirectToAction("Index");
          }
        // GET: Features/Delete/5
        public ActionResult Delete(int? id)
        {            
            Feature feature = db.Features.Find(id);
            return PartialView("_Delete", feature);
        }
        // POST: Features/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Feature feature)
        {
            var model = new StoredProcedures();
             
             int result = model.FeatureDelete(feature.ID);
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
        public ActionResult AddFeature(int? page,int?id,int?selected=0)
        {
            Session["ModuleId"] = id;
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
            var dataFeature = db.Database.SqlQuery<Feature>("sp_Feature_GetByIdNull @selected,@pageNum,@pageSize,@totalRecords out", parameter).ToList();
            var Output = parameter.FirstOrDefault(x => x.ParameterName == "@totalRecords");
            totalRecords = int.Parse(Output.Value.ToString());
            ViewBag.Total = totalRecords;
            return PartialView("_AddFeature", dataFeature);
        }
        public ActionResult FeatureModule(int? page, int? id)
        {

            Session["ModuleId"] = id;
            int totalRecords=0;
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
            var dataFeature = db.Database.SqlQuery<Feature>("sp_Feature_GetById @id,@pageNum,@pageSize,@totalRecords out", parameter).ToList();
            var Output = parameter.FirstOrDefault(x => x.ParameterName == "@totalRecords");
            totalRecords = int.Parse(Output.Value.ToString());
            ViewBag.Total = totalRecords;
            return PartialView("_FeatureModule", dataFeature);
        }
        [HttpPost]
        public ActionResult FeatureModule(int? page, int[] ids, int id, Feature feature)
        {
            var model = new StoredProcedures();
            int pageNumber = page ?? 1;
            if (ids != null)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    int idModule = ids[i];
                    int result = model.FeatureModule(idModule, feature.ModuleId = id);
                }
                SetAlert("Add Feature in Module Success", "success");

            }
            else
            {
                SetAlert("Add  Feature in Module Fail", "error");
            }
            return RedirectToAction("Details", "Modules", new { id =@Session["ModuleId"], name = @Session["ModuleName"] });

        }
       
        [HttpPost]
        public ActionResult DropModule(int id, int? page, int[] ids,Feature feature)
        {
           var model=new StoredProcedures();
           int pageNumber = page ?? 1;
           if (ids != null)
            {
                for (int i = 0; i < ids.Length; i++)
                {
                    int idFeature = ids[i];
                    int result = model.DropModule(idFeature,feature.ModuleId=id);
                }
                SetAlert("Delete Feature in Module Success", "success");
            }
           else
           {
                SetAlert("Delete Feature in Module Fail", "error");
           }
           return RedirectToAction("Details", "Modules", new { id = @Session["ModuleId"], name = @Session["ModuleName"] });
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
