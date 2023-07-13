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
    public class Invoice_ItemsController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Invoice_Items/
        public ActionResult Index()
        {
            var ware = Convert.ToInt32(Session["WareHouse_ID"]);
            var invoice_items = db.Invoice_Items.Where(i => i.Invoice.Status == "Approved" && i.Invoice.WareHouse_ID == ware);//.Include(i => i.Invoice).Include(i => i.Item);
            return View(invoice_items.ToList());
        }

        // GET: /Invoice_Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice_Items invoice_Items = db.Invoice_Items.Find(id);
            if (invoice_Items == null)
            {
                return HttpNotFound();
            }
            return View(invoice_Items);
        }

        // GET: /Invoice_Items/Create
        public ActionResult Create()
        {
            ViewBag.Invoice_ID = new SelectList(db.Invoices, "Invoice_ID", "Payment_Method");
            ViewBag.Item_ID = new SelectList(db.Items, "Item_ID", "Status");
            return View();
        }

        // POST: /Invoice_Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="In_Item_ID,Invoice_ID,Item_ID,Quantity,unit_Price,total_Price,last_Updated,Status,Entered_By,Approved_By,Issue_Date,Item_NameAr,comment")] Invoice_Items invoice_Items)
        {
            if (ModelState.IsValid)
            {
                db.Invoice_Items.Add(invoice_Items);
          invoice_Items.Status="Active";
		  invoice_Items.last_Updated=DateTime.Now;
		 invoice_Items.Entered_By = CurrentUserName;
               
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Invoice_ID = new SelectList(db.Invoices, "Invoice_ID", "Payment_Method", invoice_Items.Invoice_ID);
            ViewBag.Item_ID = new SelectList(db.Items, "Item_ID", "Item_Name", invoice_Items.Item_ID);
          
            return View(invoice_Items);
        }

        // GET: /Invoice_Items/Edit/5
        public ActionResult Edit(int? id)
        {
            var ware = Convert.ToInt32(Session["WareHouse_ID"]);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice_Items invoice_Items = db.Invoice_Items.Find(id);
            if (invoice_Items == null)
            {
                return HttpNotFound();
            }
            ViewBag.Payment_Method = new SelectList(CommonUtils.getPaymentM(), "Value", "Text");
            var itemList = from b in db.Items
                           where b.Status == "Active" && b.Warehouse_ID == ware
                           select new { desc = b.Product.Name_AR, code = b.Item_ID };
            ViewBag.Item_IDN = new SelectList(itemList, "code", "desc");
           
            return View(invoice_Items);
        }

        // POST: /Invoice_Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Invoice_Items invoice_IteX, decimal? totalAmount,decimal? Price_Unit,int? Qunt,int Item_ID,int? Item_IDN,int? RemainedAmount)
        {
            try
            {
                if (ModelState.IsValid)
                {
                     Invoice_Items invoice_IteZ = new Invoice_Items();
                     var id = invoice_IteZ.In_Item_ID;

                    // old.comment = invoice_Items.comment;
                    //  invoice_IteX.Issue_Date = old.Issue_Date;
                    // invoice_IteX.Invoice_ID = old.Invoice_ID;
                    invoice_IteX.total_Price = Convert.ToDecimal(totalAmount);
                    invoice_IteX.unit_Price = Convert.ToDecimal(Price_Unit);
                    invoice_IteX.Quantity = Convert.ToDecimal(Qunt);
                    invoice_IteX.Status = "Replace";
                    invoice_IteX.last_Updated = DateTime.Now;
                    invoice_IteX.Entered_By = CurrentUserName;
                    invoice_IteX.Approved_By = CurrentUserName;
                    invoice_IteX.Item_NameAr = Item_IDN.ToString();
                    invoice_IteX.Item_ID = Convert.ToInt32( Item_IDN);
                    invoice_IteX.Invoice_ID = db.Invoice_Items.Where(i => i.In_Item_ID == invoice_IteX.In_Item_ID).Select(i => i.Invoice_ID).SingleOrDefault();
                    invoice_IteX.Issue_Date = db.Invoice_Items.Where(i => i.In_Item_ID == invoice_IteX.In_Item_ID).Select(i => i.Issue_Date).SingleOrDefault();
                    invoice_IteX.WareHouse_ID = db.Invoice_Items.Where(i => i.In_Item_ID == invoice_IteX.In_Item_ID).Select(i => i.WareHouse_ID).SingleOrDefault();
                    db.Entry(invoice_IteX).State = EntityState.Modified;

                    // invoice_IteX.comment = old.comment;
                    // هنا يتم تعديل الكمية للصنف القديم
                    var item = new Item();
                    item = db.Items.Where(i => i.Item_ID ==Item_ID).SingleOrDefault();
                   
                    item.AvailableQ = item.AvailableQ + Qunt;
                    db.Entry(item).State = EntityState.Modified;
                    // هنا يتم تعديل الكمية للصنف الجديد
                    var itemX = new Item();
              
                    itemX = db.Items.Where(i => i.Item_ID == Item_IDN).SingleOrDefault();

                    itemX.AvailableQ = itemX.AvailableQ - Qunt;
                    db.Entry(itemX).State = EntityState.Modified;
                    //Invoice Edit
                    var invoicex = db.Invoices.Where(i => i.Invoice_ID == invoice_IteX.Invoice_ID).SingleOrDefault();
                    invoicex.LastUpdate = DateTime.Now;
                    invoicex.Approved_By = CurrentUserName;
                    invoicex.Total_Price = invoicex.Total_Price + RemainedAmount;
                    db.Entry(invoicex).State = EntityState.Modified;
            var ware = Convert.ToInt32(Session["WareHouse_ID"]);
                    var time = DateTime.Now.Date;
                    var day = db.DailyActivities.Where(i => i.DayDate == time  && i.Warehouse_ID == ware && i.OpenStatus == "Openned").SingleOrDefault();
                    if(day!=null)
                    {
                        if (day.SoldAmount == null)
                        {
                            day.SoldAmount = 0;
                            day.SoldAmount = day.SoldAmount + RemainedAmount;

                        }
                        else
                        {
                            day.SoldAmount = day.SoldAmount + RemainedAmount;

                        }

                        db.Entry(day).State = EntityState.Modified;

                        db.SaveChanges();
                        CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Saved);

                    }

                }
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
                throw ;
            }
            CommonUtils.SetFeedback(Feedback.QuantityNotAvalable, Feedback.Feedback_Error);
            return RedirectToAction("Index");
           
            ViewBag.Payment_Method = new SelectList(CommonUtils.getPaymentM(), "Value", "Text");

            var itemList = from b in db.Items
                           where b.Status == "Active"  
                           select new { desc = b.Product.Name_AR, code = b.Item_ID };
            ViewBag.Item_ID = new SelectList(itemList, "code", "desc");
            return View(invoice_IteX);
        }

        // GET: /Invoice_Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice_Items invoice_Items = db.Invoice_Items.Find(id);
            if (invoice_Items == null)
            {
                return HttpNotFound();
            }
            return View(invoice_Items);
        }

        // POST: /Invoice_Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice_Items invoice_Items = db.Invoice_Items.Find(id);
            db.Invoice_Items.Remove(invoice_Items);
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
