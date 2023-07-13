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

namespace NMS.Controllers
{
    public class AuxiliariesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Auxiliaries/
        public ActionResult Index()
        {
            var auxiliaries = db.Auxiliaries.Include(a => a.GL);
            return View(auxiliaries.ToList());
        }

        // GET: /Auxiliaries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auxiliary auxiliary = db.Auxiliaries.Find(id);
            if (auxiliary == null)
            {
                return HttpNotFound();
            }
            return View(auxiliary);
        }

        // GET: /Auxiliaries/Create
        public ActionResult Create()
        {
            Session["axiliaries"] = null;
            ViewBag.GL_ID = new SelectList(db.GLs, "GL_ID", "Genreal_Name");
            return View();
        }
        public JsonResult Add(string name, string namear, string GL_ID)
        {

            var axiliaries = new List<Auxiliary>();
            if (Session["axiliaries"] != null)
                axiliaries = (List<Auxiliary>)Session["axiliaries"];
            var auxObj = new Auxiliary();

            auxObj.Aux_Name = name;

            auxObj.Aux_Name_AR = namear;
            if(!string.IsNullOrEmpty(GL_ID))
            auxObj.GL_ID = int.Parse(GL_ID);


            axiliaries.Add(auxObj);
            Session["axiliaries"] = axiliaries;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Auxiliaries/Partial/_AuxTAddPartial.cshtml", axiliaries.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // POST: /Auxiliaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Auxiliary auxiliary)
        {
            try
            {
                if (Session["axiliaries"] != null)
                {
                    List<Auxiliary> auxlist = (List<Auxiliary>)Session["axiliaries"];
                    int id = 0;
                    if (db.Auxiliaries.Any())
                        id = db.Auxiliaries.Max(i => i.Aux_ID);

                    foreach (var item in auxlist)
                    {
                        var auxObj = new Auxiliary();
                        id += 1;
                        auxObj.Aux_ID = id;
                        auxObj.Aux_Name = item.Aux_Name;
                        auxObj.Aux_Name_AR = item.Aux_Name_AR;
                        auxObj.GL_ID = item.GL_ID;
                        auxObj.Status = "Active";
                        auxObj.LastUpdate = DateTime.Now;
                        auxObj.Entered_By = CurrentUserName;
                        db.Auxiliaries.Add(auxObj);
                        db.SaveChanges();
                        Session["axiliaries"] = null;
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
                return View(auxiliary);
            }


            return View(auxiliary);
        }

        // GET: /Auxiliaries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auxiliary auxiliary = db.Auxiliaries.Find(id);
            if (auxiliary == null)
            {
                return HttpNotFound();
            }
            ViewBag.GL_ID = new SelectList(db.GLs, "GL_ID", "Genreal_Name", auxiliary.GL_ID);
            return View(auxiliary);
        }

        // POST: /Auxiliaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Aux_ID,GL_ID,Aux_Name,Aux_Name_AR,Entered_By,LastUpdate,Status")] Auxiliary auxiliary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auxiliary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GL_ID = new SelectList(db.GLs, "GL_ID", "Genreal_Name", auxiliary.GL_ID);
            return View(auxiliary);
        }

        // GET: /Auxiliaries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auxiliary auxiliary = db.Auxiliaries.Find(id);
            if (auxiliary == null)
            {
                return HttpNotFound();
            }
            return View(auxiliary);
        }

        // POST: /Auxiliaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Auxiliary auxiliary = db.Auxiliaries.Find(id);
            db.Auxiliaries.Remove(auxiliary);
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
