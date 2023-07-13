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
using NMS.Core.Resources;
using NMS.Utility;
using System.Data.Entity.Validation;

namespace NMS.Controllers
{
    [Authorize(Roles = "Casher,Admins")]

    public class DailyActivitiesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        private NaalaxAuthEntities2 dbuser = new NaalaxAuthEntities2();
        //
        public partial class VMDailyActivity
        {

            public Nullable<decimal> OpenningBal { get; set; }

            public Nullable<decimal> Expenses { get; set; }


            public string ExpensesDesc { get; set; }

        }
        //Ajax-Json
        public JsonResult Add(string ExpensesDesc, decimal Expenses)
        {

            var DailyActivity = new List<VMDailyActivity>();
            if (Session["DailyActivity"] != null)
                DailyActivity = (List<VMDailyActivity>)Session["DailyActivity"];
            var DailyObj = new VMDailyActivity();

            DailyObj.Expenses = Expenses;

            DailyObj.ExpensesDesc = ExpensesDesc;
            //  DailyObj.OpenningBal = OpeningBalance;



            DailyActivity.Add(DailyObj);
            Session["DailyActivity"] = DailyActivity;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/DailyActivities/Partial/_DailyAddPartial.cshtml", DailyActivity.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }

        public JsonResult check(int? Expenses)
        {
            int warehouse = Convert.ToInt32(Session["WareHouse_ID"]);
            int branch = Convert.ToInt32(Session["Branch_ID"]);
            var time = DateTime.Now.Date;
            string data="true";
          var  open = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).Sum(i=>i.OpenningBal);
          if(open==null)
          {
              open = 0;
          }
          if (Expenses > open)
          {
              data="false";
              return Json(data, JsonRequestBehavior.AllowGet);
          }

          return Json(data, JsonRequestBehavior.AllowGet);

          
        }
        public ActionResult Exp()
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
            return View();

        }
        [HttpPost]
        public ActionResult Exp(DailyActivity dailyactivitiesX, decimal? OpeningBalance)
        {

            try
            {
                int warehouse = Convert.ToInt32(Session["WareHouse_ID"]);
                int branch = Convert.ToInt32(Session["Branch_ID"]);
                var time = DateTime.Now.Date;
                var dailyactivities = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).SingleOrDefault();

                List<VMDailyActivity> DailyActivitylist = (List<VMDailyActivity>)Session["DailyActivity"];
                if (Session["DailyActivity"] != null)
                {
                    int id = 0;
                    if (db.DailyActivities.Any())
                        id = db.DailyActivities.Max(i => i.ID);
                    if (dailyactivities.DayDate == DateTime.Now.Date)
                    {



                        foreach (var item in DailyActivitylist)
                        {
                            var DailyActivitiesObj = new DailyActivity();
                            id += 1;
                            DailyActivitiesObj.ID = id;
                            DailyActivitiesObj.Expenses = item.Expenses;
                            DailyActivitiesObj.ExpensesDesc = item.ExpensesDesc;
                            DailyActivitiesObj.DayDate = DateTime.Now;
                            DailyActivitiesObj.SoldAmount = db.Invoices.Where(i => i.Approved_By == CurrentUserName && i.LastUpdate == DateTime.Now).ToList().Sum(i => i.Total_Price);
                            DailyActivitiesObj.paymentAmount = 0;
                            DailyActivitiesObj.Warehouse_ID = Convert.ToInt32(Session["WareHouse_ID"]);
                            DailyActivitiesObj.Branch_ID = Convert.ToInt32(Session["Branch_ID"]);
                            DailyActivitiesObj.OpenningBal = OpeningBalance;
                            DailyActivitiesObj.OpenStatus = "Opening";
                            DailyActivitiesObj.Status = "Active";
                            DailyActivitiesObj.LastUpdated = DateTime.Now;
                            DailyActivitiesObj.Entered_By = CurrentUserName;
                            db.DailyActivities.Add(DailyActivitiesObj);
                            db.SaveChanges();
                            Session["DailyActivity"] = null;
                        }
                        CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                        return RedirectToAction("Index");
                    }
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);

                return View(dailyactivities);

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
        }
        public ActionResult IndexClose()
        {
            return View(db.DailyActivities.Where(i => i.OpenStatus == "Openned" && i.Entered_By == CurrentUserName).ToList());
        }

        // GET: /DailyActivities/
        public ActionResult Index()
        {
            var ware = Convert.ToInt32(Session["WareHouse_ID"]);
            var day = db.DailyActivities.Where(i => i.OpenStatus=="Openned"&&i.Warehouse_ID==ware).ToList();
            return View(day);
        }

        // GET: /DailyActivities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyActivity dailyActivity = db.DailyActivities.Find(id);
            if (dailyActivity == null)
            {
                return HttpNotFound();
            }
            return View(dailyActivity);
        }

        // GET: /DailyActivities/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Close(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyActivity dailyActivity = db.DailyActivities.Find(id);
            if (dailyActivity == null)
            {
                return HttpNotFound();
            }
            int warehouse = Convert.ToInt32(Session["WareHouse_ID"]);
            int branch = Convert.ToInt32(Session["Branch_ID"]);
            var time = DateTime.Now.Date;

            var dailyActivityx = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).ToList();
            if (dailyActivity == null)
            {
                return HttpNotFound();
            }
            ViewBag.OpeningBalance = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).Select(i => i.OpenningBal).SingleOrDefault();
             
            return View(dailyActivity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Close([Bind(Include = "ID,OpenningBal,paymentAmount,Expenses,ExpensesDesc,SoldAmount,RemainedAmt,Entered_By,Status,LastUpdated,DayDate,OpenStatus,cashierID")] DailyActivity dailyActivity)
        {
            if (ModelState.IsValid)
            {

                DailyActivity d = db.DailyActivities.Where(i => i.ID == dailyActivity.ID).SingleOrDefault();
                //

               

                d.RemainedAmt = d.OpenningBal + d.SoldAmount - d.Expenses;
                d.Status = "Active";
                d.LastUpdated = DateTime.Now;
                d.Entered_By = CurrentUserName;
                // dailyActivity.cashierID =int.Parse(dbuser.AspNetUsers.Where(i=>i.UserName==CurrentUserName).SingleOrDefault().Id);
                d.OpenStatus = "Closed";
                //dailyActivity.DayDate = DateTime.Now;
                //  db.DailyActivities.Add(dailyActivity);
                db.Entry(d).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");


            }

            return View(dailyActivity);
        }

        // POST: /DailyActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,OpenningBal,paymentAmount,Expenses,SoldAmount,RemainedAmt,Entered_By,Status,LastUpdated,DayDate,OpenStatus,cashierID")] DailyActivity dailyActivity)
        {
            if (ModelState.IsValid)
            {
                int id = 0;
                var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();

                if (db.DailyActivities.Any())
                {
                    id = db.DailyActivities.Max(i => i.ID);
                }
                id += 1;
                var dat = DateTime.Now.Date;
                //check if openned today
                DailyActivity d = db.DailyActivities.Where(i => i.DayDate ==dat && i.OpenStatus == "Openned" && i.Entered_By == CurrentUserName).SingleOrDefault();
                //
                if (d == null)
                {
                    dailyActivity.ID = id;
                    dailyActivity.Status = "Active";
                    dailyActivity.LastUpdated = DateTime.Now;
                    dailyActivity.Entered_By = CurrentUserName;
                    dailyActivity.cashierID = CurrentUserName;//int.Parse(dbuser.AspNetUsers.Where(i=>i.UserName==CurrentUserName).SingleOrDefault().Id);
                    dailyActivity.OpenStatus = "Openned";
                    dailyActivity.paymentAmount = 0;
                    dailyActivity.Expenses = 0;
                    dailyActivity.Warehouse_ID = Convert.ToInt32(user.WareHouse_ID);
                    dailyActivity.Branch_ID = Convert.ToInt32(user.Branch_ID);
                    dailyActivity.DayDate = DateTime.Today;
                    db.DailyActivities.Add(dailyActivity);

                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                else
                {
                    TempData["AlertMessage"] = Feedback.Feedback_Error;
                 //   CommonUtils.SetFeedback(Feedback.AlreadyOpenned, Feedback.Feedback_Error);
                    return View(dailyActivity);

                }
            }

            return View(dailyActivity);
        }

        // GET: /DailyActivities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyActivity dailyActivity = db.DailyActivities.Find(id);
            if (dailyActivity == null)
            {
                return HttpNotFound();
            }
            return View(dailyActivity);
        }

        // POST: /DailyActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,OpenningBal,paymentAmount,Expenses,SoldAmount,RemainedAmt,Entered_By,Status,LastUpdated,DayDate,OpenStatus,cashierID")] DailyActivity dailyActivity)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dailyActivity).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dailyActivity);
        }

        // GET: /DailyActivities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DailyActivity dailyActivity = db.DailyActivities.Find(id);
            if (dailyActivity == null)
            {
                return HttpNotFound();
            }
            return View(dailyActivity);
        }

        // POST: /DailyActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DailyActivity dailyActivity = db.DailyActivities.Find(id);
            db.DailyActivities.Remove(dailyActivity);
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
