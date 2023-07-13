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
using System.Data.Entity.Validation;
using NMS.Core.Resources;

namespace NMS.Controllers
{
    public class ExpController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        public partial class VMExp
        {

            public decimal Amount { get; set; }
            public string Naration { get; set; }

             
        }
        // GET: /Exp/
        public ActionResult Index()
        {
            var exps = db.Exps.Include(e => e.DailyActivity).Include(e => e.WareHouse);
            return View(exps.ToList());
        }

        // GET: /Exp/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exp exp = db.Exps.Find(id);
            if (exp == null)
            {
                return HttpNotFound();
            }
            return View(exp);
        }

        // GET: /Exp/Create
        public ActionResult Create()
        {
            int warehouse = Convert.ToInt32(Session["WareHouse_ID"]);
            int branch = Convert.ToInt32(Session["Branch_ID"]);
            var time = DateTime.Now.Date;

            var dailyActivity = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).ToList();
            if (dailyActivity == null)
            {
                return HttpNotFound();
            }
            var sumPay = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).Sum(i => i.paymentAmount);
            var sumExp = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).Sum(i => i.Expenses);
            var sumOp = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).Sum(i => i.OpenningBal);
            if (dailyActivity.Count > 0)
            {
                ViewBag.OpeningBalance = (sumOp + sumPay) - sumExp;

            }
            else
            {
                ViewBag.OpeningBalance = 0;

            }
            ViewBag.ID = new SelectList(db.DailyActivities, "ID", "Entered_By");
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
            return View();
        }
        public JsonResult check(int? Amount)
        {
            int warehouse = Convert.ToInt32(Session["WareHouse_ID"]);
            int branch = Convert.ToInt32(Session["Branch_ID"]);
            var time = DateTime.Now.Date;
            string data = "true";
            var open = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).Sum(i => i.OpenningBal);
            if (open == null)
            {
                open = 0;
            }
            if (Amount > open)
            {
                data = "false";
                return Json(data, JsonRequestBehavior.AllowGet);
            }

            return Json(data, JsonRequestBehavior.AllowGet);


        }
        public JsonResult Add(decimal Amount, string Naration)
        {

            var Exp = new List<VMExp>();
            if (Session["Exp"] != null)
                Exp = (List<VMExp>)Session["Exp"];
            var ExpObj = new VMExp();

          ExpObj.Amount = Amount;

            ExpObj.Naration = Naration;




            Exp.Add(ExpObj);
            Session["Exp"] = Exp;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Exp/Partial/_ExpAddPartial.cshtml", Exp.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // POST: /Exp/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Expid,Naration,date,Warehouse_ID,ID,Entered_By,Status,LastUpdate")] Exp exp)
        {
            try
            {
                int warehouse = Convert.ToInt32(Session["WareHouse_ID"]);
                int branch = Convert.ToInt32(Session["Branch_ID"]);
                var time = DateTime.Now.Date;
                
                var cond = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).SingleOrDefault();
                if (cond != null)
                {

              
                if (Session["Exp"] != null)
                {
                    List<VMExp> Explist = (List<VMExp>)Session["Exp"];
                    int id = 0;
                    if (db.Exps.Any())
                        id = db.Exps.Max(i => i.Expid);
                    //
                    var tot = Explist.Sum(i => i.Amount);

                   var TestDetiles=
                 db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).SingleOrDefault();
                   int findid = TestDetiles.ID;
                   if (findid != null)
                   {
                       var SoldAmount = db.Invoices.Where(i => i.Approved_By == CurrentUserName && i.LastUpdate ==time).ToList().Sum(i => i.Total_Price);
                       if (SoldAmount == null)
                       {
                       SoldAmount = 0; 

                       }
                       TestDetiles.SoldAmount = SoldAmount;
                       TestDetiles.LastUpdated = DateTime.Now;
                      TestDetiles.Expenses =tot;
                      TestDetiles.OpenningBal = TestDetiles.OpenningBal - tot;
                       
                       db.Entry(TestDetiles).State = EntityState.Modified;

                   }
 
               
                    //
                    foreach (var item in Explist)
                    {
                        var expObj = new Exp();
                        id += 1;
                        expObj.Expid = id;
                        expObj.Naration = item.Naration;
                     expObj.Amount = item.Amount;
                        expObj.Warehouse_ID = Convert.ToInt32(Session["Warehouse_ID"]);
                        expObj.Status = "Active";
                        expObj.LastUpdate = DateTime.Now;
                        expObj.Entered_By = CurrentUserName;
                        expObj.date = DateTime.Now.Date;
                        expObj.dailyID = TestDetiles.ID;
                        expObj.Total = tot;
                        db.Exps.Add(expObj);
                        db.SaveChanges();
                        Session["Exp"] = null;
                    }
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View();
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
                throw;
            }

            ViewBag.ID = new SelectList(db.DailyActivities, "ID", "Entered_By", exp.dailyID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", exp.Warehouse_ID);
            return View(exp);
        }

        // GET: /Exp/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exp exp = db.Exps.Find(id);
            if (exp == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.DailyActivities, "ID", "Entered_By", exp.dailyID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", exp.Warehouse_ID);
            return View(exp);
        }

        // POST: /Exp/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Expid,Naration,date,Warehouse_ID,ID,Entered_By,Status,LastUpdate")] Exp exp)
        {
            if (ModelState.IsValid)
            {
                db.Entry(exp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.DailyActivities, "ID", "Entered_By", exp.dailyID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", exp.Warehouse_ID);
            return View(exp);
        }

        // GET: /Exp/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exp exp = db.Exps.Find(id);
            if (exp == null)
            {
                return HttpNotFound();
            }
            return View(exp);
        }

        // POST: /Exp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Exp exp = db.Exps.Find(id);
            db.Exps.Remove(exp);
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
