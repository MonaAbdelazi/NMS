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
    public class EmpLoansController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /EmpLoans/
        public ActionResult Index()
        {
            var emp_loans = db.Emp_Loans.Include(e => e.Employess).Include(e => e.Tree);
            return View(emp_loans.ToList());
        }

        // GET: /EmpLoans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Emp_Loans emp_loans = db.Emp_Loans.Find(id);
            if (emp_loans == null)
            {
                return HttpNotFound();
            }
            return View(emp_loans);
        }

        [HttpPost]
        //
        public ActionResult Details(Emp_Loans emp_loans)
        {
            emp_loans.status = "Approved";
            emp_loans.LastUpdate = DateTime.Now;
            db.Entry(emp_loans).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
            
        }
        //
        public JsonResult Add(float Amount, int Emp_ID, DateTime date, int acc_no)
        {

            var Employloan = new List<Emp_Loans>();
            if (Session["Emp_Loans"] != null)
                Employloan = (List<Emp_Loans>)Session["Emp_Loans"];
            var empObj = new Emp_Loans();

            empObj.Emp_ID = Emp_ID;
             
            empObj.amount = Amount;

            empObj.date = date;
            empObj.acc_no = acc_no;

            Employloan.Add(empObj);
            Session["Emp_Loans"] = Employloan;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/EmpLoans/Partial/_EmployLoanAddPartial.cshtml", Employloan.ToList());
           
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // GET: /EmpLoans/Create
        public ActionResult Create()
        {
            ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name");
            ViewBag.acc_no = new SelectList(db.Trees, "Acc_No", "Acc_Name");
            return View();
        }

        // POST: /EmpLoans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="loan_id,amount,date,status,Emp_ID,acc_no")] Emp_Loans emp_loans)
        {
            try
            {
                if (Session["Emp_Loans"] != null)
                {
                    List<Emp_Loans> Emploanlist = (List<Emp_Loans>)Session["Emp_Loans"];
                    int id = 0;
                    if (db.Emp_Loans.Any())
                        id = db.Emp_Loans.Max(i => i.loan_id);

                    foreach (var item in Emploanlist)
                    {
                        var EmpObj = new Emp_Loans();
                        id += 1;
                        EmpObj.loan_id = id;
                        EmpObj.Emp_ID = item.Emp_ID;
                        EmpObj.amount = item.amount;
                        EmpObj.date = item.date;

                        EmpObj.acc_no = item.acc_no;
                        EmpObj.status = "Active";
                        EmpObj.LastUpdate = DateTime.Now;
                        EmpObj.Entered_By = CurrentUserName;

                      
                        db.Emp_Loans.Add(EmpObj);
                        db.SaveChanges();
                        Session["Emp_Loans"] = null;
                    }

                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }

                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name", emp_loans.Emp_ID);
                ViewBag.acc_no = new SelectList(db.Trees, "Acc_No", "Acc_Name", emp_loans.acc_no);
                return View(emp_loans);


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
         
         //   if (ModelState.IsValid)
         //   {
         //       db.Emp_Loans.Add(emp_loans);
         // emp_loans.status="Active";
         // emp_loans.LastUpdate=DateTime.Now;
         //emp_loans.Entered_By = CurrentUserName;
         //       db.SaveChanges();
         //       return RedirectToAction("Index");
         //   }

            ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name", emp_loans.Emp_ID);
            ViewBag.acc_no = new SelectList(db.Trees, "Acc_No", "Acc_Name", emp_loans.acc_no);
            return View(emp_loans);
        }

        // GET: /EmpLoans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Emp_Loans emp_loans = db.Emp_Loans.Find(id);
            if (emp_loans == null)
            {
                return HttpNotFound();
            }
            ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name", emp_loans.Emp_ID);
            ViewBag.acc_no = new SelectList(db.Trees, "Acc_No", "Acc_Name", emp_loans.acc_no);
            return View(emp_loans);
        }

        // POST: /EmpLoans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="loan_id,amount,date,status,Emp_ID,acc_no")] Emp_Loans emp_loans)
        {
            if (ModelState.IsValid)
            {
                db.Entry(emp_loans).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name", emp_loans.Emp_ID);
            ViewBag.acc_no = new SelectList(db.Trees, "Acc_No", "Acc_Name", emp_loans.acc_no);
            return View(emp_loans);
        }

        // GET: /EmpLoans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Emp_Loans emp_loans = db.Emp_Loans.Find(id);
            if (emp_loans == null)
            {
                return HttpNotFound();
            }
            return View(emp_loans);
        }

        // POST: /EmpLoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Emp_Loans emp_loans = db.Emp_Loans.Find(id);
            db.Emp_Loans.Remove(emp_loans);
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
