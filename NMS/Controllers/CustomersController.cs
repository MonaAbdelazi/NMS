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
    public class CustomersController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Customers/
        public ActionResult Index()
        {
            return View(db.Customers.Where(i=>i.Status=="Active").ToList());
        }

        // GET: /Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
          //  ViewBag.Type = new SelectList(CommonUtils.CusType(), "Value", "Text",customer.Type);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text");

            return View(customer);
        }
        public JsonResult Add(string name, string namear, int? phone, string email, string address, string Type,string project,string Tax)
        {

            var Customer = new List<Customer>();
            if (Session["Customer"] != null)
                Customer = (List<Customer>)Session["Customer"];
            var CustomerObj = new Customer();

            CustomerObj.Cus_Name = name;

            CustomerObj.Cus_Name_AR = namear;


            CustomerObj.Phone = phone;
            CustomerObj.E_Mail = email;
            CustomerObj.Address = address;
            CustomerObj.Type = Type;
            CustomerObj.Project = project;
            CustomerObj.Tax = Tax;
            Customer.Add(CustomerObj);
            Session["Customer"] = Customer;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Customers/Partial/_CustomerAddPartial.cshtml", Customer.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }

        // GET: /Customers/Create
        public ActionResult Create()
        {
            //ViewBag.Type = new SelectList(CommonUtils.CusType(), "Value", "Text");
            
            return View();
        }

        // POST: /Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Cus_ID,Cus_Name,Cus_Name_AR,Address,Phone,E_Mail,Web_Site,Entered_By,LastUpdate,Status")] Customer customer)
        {
            try
            {
                if (Session["Customer"] != null)
                {
                    List<Customer> Customerlist = (List<Customer>)Session["Customer"];
                    int id = 0;
                    if (db.Customers.Any())
                        id = db.Customers.Max(i => i.Cus_ID);

                    foreach (var item in Customerlist)
                    {
                        var custObj = new Customer();
                        id += 1;
                        custObj.Cus_ID = id;
                        custObj.Cus_Name = item.Cus_Name;
                        custObj.Cus_Name_AR = item.Cus_Name_AR;
                        custObj.Status = "Active";
                        custObj.Phone = item.Phone;
                        custObj.Address = item.Address;
                        custObj.E_Mail = item.E_Mail;
                        custObj.Type = item.Type;
                        custObj.Project = item.Project;
                        custObj.Tax = item.Tax;
                        custObj.LastUpdate = DateTime.Now;
                        custObj.Entered_By = CurrentUserName;
                        db.Customers.Add(custObj);
                        db.SaveChanges();
                        Session["Customer"] = null;
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
                return View(customer);
            }
        }

        // GET: /Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", customer.Status);
           // ViewBag.Type = new SelectList(CommonUtils.CusType(), "Value", "Text",customer.Type);

            return View(customer);
        }

        // POST: /Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Cus_ID,Cus_Name,Cus_Name_AR,Address,Phone,E_Mail,Web_Site,Entered_By,LastUpdate,Status")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", customer.Status);

                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: /Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", customer.Status);
         //   ViewBag.Type = new SelectList(CommonUtils.CusType(), "Value", "Text",customer.Type);
            
            return View(customer);
        }

        // POST: /Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            customer.Status = "Deleted";
            db.Entry(customer).State = EntityState.Modified;
            //db.Customers.Remove(customer);
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
