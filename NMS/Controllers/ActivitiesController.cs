using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NMS.Core.Resources;
using NMS.Tools;
using NMS.Data;
using NMS.Utility;

namespace NMS.Controllers
{
    [Authorize(Roles="Admins")]
    public class ActivitiesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        //Jsin-Ajax
        public JsonResult Add(string Activity_Name, string Activity_AR)
        {

            var Activity = new List<Activity>();
            if (Session["Activity"] != null)
                Activity = (List<Activity>)Session["Activity"];
            var ActivityObj = new Activity();

            ActivityObj.Activity_Name = Activity_Name;

            ActivityObj.Activity_AR = Activity_AR;



            Activity.Add(ActivityObj);
            Session["Activity"] = Activity;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Activities/Partial/_ActivitiesAddPartial.cshtml", Activity.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }

        // GET: /Activities/
        public ActionResult Index()
        {
            return View(db.Activities.ToList());
        }

        // GET: /Activities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            return View(activity);
        }

        // GET: /Activities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Activities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Activity_ID,Activity_Name,Activity_AR,Status,LastUpdate,Entered_By")] Activity activity)
        {
            try
            {
                if (Session["Activity"] != null)
                {
                    List<Activity> Actlist = (List<Activity>)Session["Activity"];
                    int id = 0;
                    if (db.Activities.Any())
                        id = db.Activities.Max(i => i.Activity_ID);

                    foreach (var item in Actlist)
                    {
                        var ActlistObj = new Activity();
                        id += 1;
                        ActlistObj.Activity_ID = id;
                        ActlistObj.Activity_Name = item.Activity_Name;
                        ActlistObj.Activity_AR = item.Activity_AR;
                        ActlistObj.Status = "Active";
                        ActlistObj.LastUpdate = DateTime.Now;
                        ActlistObj.Entered_By = CurrentUserName;
                        db.Activities.Add(ActlistObj);
                        db.SaveChanges();
                        Session["Activity"] = null;
                    }
                  //  TempData["AlertMessage"] = Feedback.SavedSuccessfully + "Activity_ID  " + Actlist.Select(i=>i.Activity_ID).ToString();
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                 
                return View();
            }
            catch (Exception e)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                TempData["AlertMessage"] =  Feedback.NotSavedSuccessfully + "There is an Error " + e.Message;
              

                // CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(activity);
            }

        }

        // GET: /Activities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", activity.Status);
            return View(activity);
        }

        // POST: /Activities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Activity_ID,Activity_Name,Activity_AR,Status,LastUpdate,Entered_By")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                activity.LastUpdate = DateTime.Now;
                activity.Entered_By = CurrentUserName;
                db.Entry(activity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", activity.Status);
            return View(activity);
        }

        // GET: /Activities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Activity activity = db.Activities.Find(id);
            if (activity == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", activity.Status);
            return View(activity);
        }

        // POST: /Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Activity activity = db.Activities.Find(id);
            activity.LastUpdate = DateTime.Now;
            activity.Entered_By = CurrentUserName;
            activity.Status = "Deleted";
            db.Entry(activity).State = EntityState.Modified;
           // db.Activities.Remove(activity);
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
