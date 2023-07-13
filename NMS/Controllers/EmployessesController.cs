using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
    public class EmployessesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Employesses/
        public JsonResult Add(string Name, string Name_AR,int Branch_ID, string Job_Tittle,string IQAMA,string Passport_Id,DateTime Expdate_Pass,DateTime Expdate_IQAMA)
        {

            var Employess = new List<Employess>();
            if (Session["Employess"] != null)
                Employess = (List<Employess>)Session["Employess"];
            var empObj = new Employess();

            empObj.Name = Name;

            empObj.Name_AR = Name_AR;
            empObj.Branch_ID = Branch_ID;
            empObj.Job_Tittle = Job_Tittle;

            empObj.IQAMA = IQAMA;
            empObj.Passport_Id = Passport_Id;
            empObj.Expdate_Pass = Expdate_Pass;
            empObj.Expdate_IQAMA = Expdate_IQAMA;

            Employess.Add(empObj);
            Session["Employess"] = Employess;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Employesses/Partial/_EmployessAddPartial.cshtml", Employess.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            var employesses = db.Employesses.Include(e => e.Branch);
            return View(employesses.ToList());
        }

        // GET: /Employesses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employess employess = db.Employesses.Find(id);
            if (employess == null)
            {
                return HttpNotFound();
            }
            return View(employess);
        }

        // GET: /Employesses/Create
        public ActionResult Create()
        {
            ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name");
            return View();
        }

        // POST: /Employesses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Emp_ID,Name,Name_AR,Job_Tittle,Status,LastUpdate,Entered_By,Branch_ID")] Employess employess)
        {
            try
            {
                if (Session["Employess"] != null)
                {
                    List<Employess> Employessist = (List<Employess>)Session["Employess"];
                    int id = 0;
                    if (db.Trees.Any())
                        id = db.Employesses.Max(i => i.Emp_ID);

                    foreach (var item in Employessist)
                    {
                        var EmpObj = new Employess();
                        id += 1;
                        EmpObj.Emp_ID = id;
                        EmpObj.Name = item.Name;
                        EmpObj.Name_AR = item.Name_AR;
                        EmpObj.Job_Tittle = item.Job_Tittle;
                        EmpObj.Branch_ID = item.Branch_ID;

                        EmpObj.Status = "Active";
                        EmpObj.LastUpdate = DateTime.Now;
                        EmpObj.Entered_By = CurrentUserName;

                        EmpObj.IQAMA = item.IQAMA;
                        EmpObj.Passport_Id = item.Passport_Id;
                        EmpObj.Expdate_Pass = item.Expdate_Pass;
                        EmpObj.Expdate_IQAMA = item.Expdate_IQAMA;
                        db.Employesses.Add(EmpObj);
                        db.SaveChanges();
                        Session["Employess"] = null;
                    }

                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }

                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name", employess.Branch_ID);
                return View(employess);


            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                  CommonUtils.SetFeedback(e.Message, Feedback.Feedback_Error);
                throw e;
            }
         

        }

        // GET: /Employesses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employess employess = db.Employesses.Find(id);
            if (employess == null)
            {
                return HttpNotFound();
            }
            ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name", employess.Branch_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", employess.Status);
            return View(employess);
        }

        // POST: /Employesses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Emp_ID,Name,Name_AR,Job_Tittle,Status,LastUpdate,Entered_By,Branch_ID")] Employess employess)
        {
            if (ModelState.IsValid)
            {
                employess.LastUpdate = DateTime.Now;
                employess.Entered_By = CurrentUserName;
                db.Entry(employess).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name", employess.Branch_ID);
            return View(employess);
        }

        // GET: /Employesses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employess employess = db.Employesses.Find(id);
            if (employess == null)
            {
                return HttpNotFound();
            }
            ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name", employess.Branch_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", employess.Status);
            return View(employess);
        }

        // POST: /Employesses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employess employess = db.Employesses.Find(id);
            employess.LastUpdate = DateTime.Now;
            employess.Entered_By = CurrentUserName;
            employess.Status = "Deleted";
            db.Entry(employess).State = EntityState.Modified; ;
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
