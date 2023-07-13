using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NMS.Tools;
using NMS.Data;
using NMS.Utility;
using NMS.Core.Resources;
using System.Data.Entity.Validation;

namespace NMS.Controllers
{
    public class UnitsController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Units/
        public ActionResult Index()
        {
            return View(db.Units.Where(i=>i.Status==CommonUtils.STATUS_ACTIVE).ToList());
        }

        // GET: /Units/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // GET: /Units/Create
        public ActionResult Create()
        {
            Session["units"] = null;

            return View();
        }

        // POST: /Units/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Unit unit)
        {
            try
            {
                if (Session["units"] != null)
                {
                    List<Unit> unitlist = (List<Unit>)Session["units"];
                    int id = 0;
                    if (db.Units.Any())
                        id = db.Units.Max(i => i.Unit_ID);

                    foreach (var item in unitlist)
                    {
                        var unitObj = new Unit();
                        id += 1;
                        unitObj.Unit_ID = id;
                        unitObj.Unit_Name = item.Unit_Name;
                        unitObj.Unit_Name_AR = item.Unit_Name_AR;
                        unitObj.Comment = item.Comment;
                        unitObj.Status = "Active";
                        unitObj.size = item.size;
                        unitObj.LastUpdate = DateTime.Now;
                        unitObj.Entered_By = CurrentUserName;
                        db.Units.Add(unitObj);
                        db.SaveChanges();
                        Session["units"] = null;
                    }
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View();
            }
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(unit);
            }


            return View(unit);
        }
        public JsonResult Add(string name, string namear, string Comment,string size)
        {

            var units = new List<Unit>();
            if (Session["units"] != null)
                units = (List<Unit>)Session["units"];
            var unitObj = new Unit();

            unitObj.Unit_Name = name;

            unitObj.Unit_Name_AR = namear;
            unitObj.Comment = Comment;
            unitObj.size = size;


            units.Add(unitObj);
            Session["units"] = units;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Units/Partial/_UnitTAddPartial.cshtml", units.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }

        // GET: /Units/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", unit.Status);

            return View(unit);
        }

        // POST: /Units/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Unit_ID,Unit_Name,Unit_Name_AR,Comment,Status,LastUpdate,Entered_By")] Unit unit)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    unit.LastUpdate = DateTime.Now;
                    unit.Entered_By = CurrentUserName;
                    db.Entry(unit).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException e)
                {

                    // throw;
                }
               
                
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", unit.Status);

            return View(unit);
        }

        // GET: /Units/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Unit unit = db.Units.Find(id);
            if (unit == null)
            {
                return HttpNotFound();
            }
            return View(unit);
        }

        // POST: /Units/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Unit unit = db.Units.Find(id);
            unit.LastUpdate = DateTime.Now;
            unit.Entered_By = CurrentUserName;
            unit.Status = "Deleted";
            db.Entry(unit).State = EntityState.Modified;
            db.SaveChanges();
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
    }
}
