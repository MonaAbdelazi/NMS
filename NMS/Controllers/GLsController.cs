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
    public class GLsController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /GLs/
        public ActionResult Index()
        {
            return View(db.GLs.ToList());
        }

        // GET: /GLs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GL gL = db.GLs.Find(id);
            if (gL == null)
            {
                return HttpNotFound();
            }
            return View(gL);
        }

        // GET: /GLs/Create
        public ActionResult Create()
        {
            Session["gls"] = null;
            return View();
        }
        public JsonResult Add(string name, string namear)
        {

            var gls = new List<GL>();
            if (Session["gls"] != null)
                gls = (List<GL>)Session["gls"];
            var glObj = new GL();
        
            glObj.Genreal_Name = name;

            glObj.Genreal_Name_AR = namear;
            gls.Add(glObj);
            Session["gls"] = gls;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/GLs/Partial/_GLTAddPartial.cshtml", gls.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // POST: /GLs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( GL gL)
        {
            try
            {
                if (Session["gls"] != null)
                {
                    List<GL> gllist = (List<GL>)Session["gls"];
                    int id = 0;
                    if (db.GLs.Any())
                        id = db.GLs.Max(i => i.GL_ID);

                    foreach (var item in gllist)
                    {
                        var glObj = new GL();
                        id += 1;
                        glObj.GL_ID = id;
                        glObj.Genreal_Name = item.Genreal_Name;
                        glObj.Genreal_Name_AR = item.Genreal_Name_AR;
                        glObj.GL_ID = item.GL_ID;
                        glObj.Status = "Active";
                        glObj.LastUpdate = DateTime.Now;
                        glObj.Entered_By = CurrentUserName;
                        db.GLs.Add(glObj);
                        db.SaveChanges();
                        Session["gls"] = null;
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
                return View(gL);
            }


            return View(gL);
        }

        // GET: /GLs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GL gL = db.GLs.Find(id);
            if (gL == null)
            {
                return HttpNotFound();
            }
            return View(gL);
        }

        // POST: /GLs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="GL_ID,Genreal_Name,Genreal_Name_AR,Status,LastUpdate,Entered_By")] GL gL)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gL).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gL);
        }

        // GET: /GLs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GL gL = db.GLs.Find(id);
            if (gL == null)
            {
                return HttpNotFound();
            }
            return View(gL);
        }

        // POST: /GLs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GL gL = db.GLs.Find(id);
            db.GLs.Remove(gL);
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
