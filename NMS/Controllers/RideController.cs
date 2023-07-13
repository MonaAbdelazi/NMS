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
    public class RideController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Ride/
        public ActionResult Index()
        {
            var rides = db.Ride.Where(i=>i.Status=="Active");
            return View(rides.ToList());
        }

        // GET: /Ride/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ride ride = db.Ride.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name", ride.Emp_ID);
            ViewBag.SirkID = new SelectList(db.InItemsInvoices, "ID", "Entered_By", ride.SirkID);
            ViewBag.Invoice_ID = new SelectList(db.Invoices, "Invoice_ID", "Payment_Method", ride.Invoice_ID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", ride.Warehouse_ID);
            ViewBag.Type = new SelectList(CommonUtils.getType(), "Value", "Text", ride.Type);
            return View(ride);
        }
        //
        public JsonResult Add(int count, double Amount, string Type, int Emp_ID, int? Invoice_ID, Int64? SirkID,DateTime date, string comment)
        {

            var Ride = new List<Ride>();
            if (Session["Ride"] != null)
                Ride = (List<Ride>)Session["Ride"];
            var RideObj = new Ride();

            RideObj.count = count;

            RideObj.Amount = Amount;
            RideObj.Type = Type;
            RideObj.Emp_ID = Emp_ID;
            RideObj.Invoice_ID = Invoice_ID;
            RideObj.SirkID = (int) SirkID;
            RideObj.date = date;

            RideObj.comment = comment;



            Ride.Add(RideObj);
            Session["Ride"] = Ride;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Ride/Partial/_RideAddPartial.cshtml", Ride.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // GET: /Ride/Create
        public ActionResult Create()
        {
            ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name");
            ViewBag.SirkID = new SelectList(db.InItemsInvoices, "ID", "InSirkNo");
            ViewBag.Invoice_ID = new SelectList(db.Invoices, "Invoice_ID", "Invoice_ID");
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
            ViewBag.Type = new SelectList(CommonUtils.getType(), "Value", "Text");

            return View();
        }

        // POST: /Ride/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Ride_Id,count,Amount,Type,Emp_ID,Invoice_ID,SirkID,date,Status,EnterdBy,ApprovedBy,Warehouse_ID")] Ride ride)
        {
            try
            {
                if (Session["Ride"] != null)
                {
                    List<Ride> Ridelist = (List<Ride>)Session["Ride"];
                    int id = 0;
                    if (db.Ride.Any())
                        id = db.Ride.Max(i => i.Ride_Id);

                    foreach (var item in Ridelist)
                    {
                        var rideObj = new Ride();
                        id += 1;
                        rideObj.Ride_Id = id;
                        rideObj.count = item.count;
                        rideObj.Amount = item.Amount;
                        rideObj.Type = item.Type;
                        rideObj.Emp_ID = item.Emp_ID;
                        rideObj.Invoice_ID = item.Invoice_ID;
                        rideObj.SirkID = item.SirkID;

                        rideObj.comment = item.comment;
                        rideObj.Status = "Active";
                        rideObj.LastUpdate = DateTime.Now;
                        rideObj.EnterdBy = CurrentUserName;
                        rideObj.date = item.date;

                        rideObj.Warehouse_ID = Convert.ToInt32(Session["WareHouse_ID"]); 
                        db.Ride.Add(rideObj);
                        db.SaveChanges();
                        Session["Ride"] = null;
                    }
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name", ride.Emp_ID);
                ViewBag.SirkID = new SelectList(db.InItemsInvoices, "ID", "InSirkNo", ride.SirkID);
                ViewBag.Invoice_ID = new SelectList(db.Invoices, "Invoice_ID", "Invoice_ID", ride.Invoice_ID);
                ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", ride.Warehouse_ID);
                ViewBag.Type = new SelectList(CommonUtils.getType(), "Value", "Text", ride.Type);

                return RedirectToAction("Index");

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
         //   if (ModelState.IsValid)
         //   {
         //       db.Rides.Add(ride);
         // ride.Status="Active";
         // ride.LastUpdate=DateTime.Now;
         //ride.EnterdBy = CurrentUserName;
         //       db.SaveChanges();
         //       return RedirectToAction("Index");
         //   }

            ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name", ride.Emp_ID);
            ViewBag.SirkID = new SelectList(db.InItemsInvoices, "ID", "Entered_By", ride.SirkID);
            ViewBag.Invoice_ID = new SelectList(db.Invoices, "Invoice_ID", "Payment_Method", ride.Invoice_ID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", ride.Warehouse_ID);
            return View(ride);
        }

        // GET: /Ride/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ride ride = db.Ride.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name", ride.Emp_ID);
            ViewBag.SirkID = new SelectList(db.InItemsInvoices, "ID", "Entered_By", ride.SirkID);
            ViewBag.Invoice_ID = new SelectList(db.Invoices, "Invoice_ID", "Payment_Method", ride.Invoice_ID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", ride.Warehouse_ID);
            ViewBag.Type = new SelectList(CommonUtils.getType(), "Value", "Text",ride.Type);
            return View(ride);
        }

        // POST: /Ride/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Ride_Id,count,Amount,Type,Emp_ID,Invoice_ID,SirkID,date,Status,EnterdBy,ApprovedBy,Warehouse_ID")] Ride ride)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ride).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name", ride.Emp_ID);
            ViewBag.SirkID = new SelectList(db.InItemsInvoices, "ID", "Entered_By", ride.SirkID);
            ViewBag.Invoice_ID = new SelectList(db.Invoices, "Invoice_ID", "Payment_Method", ride.Invoice_ID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", ride.Warehouse_ID);
            return View(ride);
        }

        // GET: /Ride/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ride ride = db.Ride.Find(id);
            if (ride == null)
            {
                return HttpNotFound();
            }
            ViewBag.Emp_ID = new SelectList(db.Employesses, "Emp_ID", "Name", ride.Emp_ID);
            ViewBag.SirkID = new SelectList(db.InItemsInvoices, "ID", "Entered_By", ride.SirkID);
            ViewBag.Invoice_ID = new SelectList(db.Invoices, "Invoice_ID", "Payment_Method", ride.Invoice_ID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", ride.Warehouse_ID);
            ViewBag.Type = new SelectList(CommonUtils.getType(), "Value", "Text", ride.Type);
            return View(ride);
        }

        // POST: /Ride/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ride ride = db.Ride.Find(id);
            ride.Status = "Deleted";
            db.Entry(ride).State = EntityState.Modified;
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
