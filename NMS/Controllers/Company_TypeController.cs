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
   [Authorize(Roles = "Admins")]
    public class Company_TypeController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Company_Type/
        public ActionResult Index()
        {
            return View(db.Company_Type.ToList());
        }

        // GET: /Company_Type/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company_Type company_Type = db.Company_Type.Find(id);
            if (company_Type == null)
            {
                return HttpNotFound();
            }
            return View(company_Type);
        }
        //
        public JsonResult Add(string name,string namear)
        {

            var companyTypes = new List<Company_Type>();
            if (Session["Company_Type"] != null)
                companyTypes = (List<Company_Type>)Session["Company_Type"];
            var comptObj = new Company_Type();

            comptObj.Name = name; 

            comptObj.Name_AR = namear;
           


            companyTypes.Add(comptObj);
            Session["Company_Type"] = companyTypes;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Company_Type/Partial/_CompTAddPartial.cshtml", companyTypes.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // GET: /Company_Type/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Company_Type/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Company_Type company_Type)//[Bind(Include="Comp_Type_ID,Name,Name_AR,Status,LastUpdate,Entered_By")]
        {
            try
            {
                if (Session["Company_Type"] != null)
                {
                    List<Company_Type> comptlist = (List<Company_Type>)Session["Company_Type"];
                    int id = 0;
                    if (db.Company_Type.Any())
                        id = db.Company_Type.Max(i => i.Comp_Type_ID);
                   
                    foreach (var item in comptlist)
                    {
                        var comptObj = new Company_Type();
                        id += 1;
                        comptObj.Comp_Type_ID = id;
                        comptObj.Name = item.Name;
                        comptObj.Name_AR = item.Name_AR;
                        comptObj.Status = "Active";
                        comptObj.LastUpdate=DateTime.Now;
                        comptObj.Entered_By = CurrentUserName;
                        db.Company_Type.Add(comptObj);
                        db.SaveChanges();
                        Session["Company_Type"] = null;
                    }
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View();
            }
            catch (Exception)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(company_Type);
            }
 
        }

        // GET: /Company_Type/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company_Type company_Type = db.Company_Type.Find(id);
            if (company_Type == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", company_Type.Status);
            return View(company_Type);
        }

        // POST: /Company_Type/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Comp_Type_ID,Name,Name_AR,Status,LastUpdate,Entered_By")] Company_Type company_Type)
        {
            if (ModelState.IsValid)
            {
                company_Type.Entered_By = CurrentUserName;
                company_Type.LastUpdate = DateTime.Now;
                db.Entry(company_Type).State = EntityState.Modified;
                db.SaveChanges();
                CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);

                return RedirectToAction("Index");
            }
            CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", company_Type.Status);
            return View(company_Type);
        }

        // GET: /Company_Type/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company_Type company_Type = db.Company_Type.Find(id);
            if (company_Type == null)
            {
                return HttpNotFound();
            }
            return View(company_Type);
        }

        // POST: /Company_Type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company_Type company_Type = db.Company_Type.Find(id);
          //  db.Company_Type.Remove(company_Type);
            company_Type.Status = "Deleted";
            company_Type.Entered_By = CurrentUserName;
            company_Type.LastUpdate = DateTime.Now;
            db.Entry(company_Type).State = EntityState.Modified;
            db.SaveChanges();
            CommonUtils.SetFeedback(Feedback.Saved, Feedback.Feedback_Success);
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
