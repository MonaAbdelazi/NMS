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
    public class SuppController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Supp/
        public ActionResult Index()
        {
            var suppliers = db.Suppliers.Where(i=>i.Status=="Active");
            return View(suppliers.ToList());
        }

        // GET: /Supp/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Suppliers supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name");
            ViewBag.acc_no = new SelectList(db.Trees.Where(i => i.Account_Type
                 == "inv"), "Acc_No", "Acc_Name_AR");
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Text", "Value", supplier.Status);
            return View(supplier);
        }
        //
        public JsonResult Add(string Supp_Name, string Supp_Name_AR, string phone, string email, string address, string Representative_Name, string Representative_phone,string acc_no,string Tax)
        {

            var Supplier = new List<Suppliers>();
            if (Session["Supplier"] != null)
                Supplier = (List<Suppliers>)Session["Supplier"];
            var SupplierObj = new Suppliers();

            SupplierObj.Supp_Name = Supp_Name;

            SupplierObj.Supp_Name_AR = Supp_Name_AR;
            SupplierObj.Representative_Phone = Representative_phone;

            SupplierObj.Representative_Name = Representative_Name;

            SupplierObj.Phone = phone;
            SupplierObj.E_Mail = email;
            SupplierObj.Address = address;
            SupplierObj.acc_no = acc_no;

            SupplierObj.Tax = Tax;
            Supplier.Add(SupplierObj);
            Session["Supplier"] = Supplier;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Supp/Partial/_SupplierAddPartial.cshtml", Supplier.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // GET: /Supp/Create
        public ActionResult Create()
        {
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name");
            ViewBag.acc_no = new SelectList(db.AccountManagments.Where(i => i.Type == "External" && i.CustomerId != 0 && i.Tree.Category == "A"), "SystemId", "Acc_Name");
            // ViewBag.BankAccounts = new SelectList(db.AccountManagments.Where(i => i.Type == "Internal" && i.CustomerId == 0 && i.Tree.Category == "I"), "SystemId", "Acc_Name");

            return View();
        }

        // POST: /Supp/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Supp_ID,Supp_Name,Supp_Name_AR,Phone,Address,E_Mail,Web_Site,Representative_Name,Representative_Phone,Status,LastUpdate,Entered_By,Country_ID")] Suppliers supplier)
        {
            try
            {
                if (Session["Supplier"] != null)
                {
                    List<Suppliers> Supplierlist = (List<Suppliers>)Session["Supplier"];
                    int id = 0;
                    if (db.Suppliers.Any())
                        id = db.Suppliers.Max(i => i.Supp_ID);

                    foreach (var item in Supplierlist)
                    {
                        var supObj = new Suppliers();
                        id += 1;
                        supObj.Supp_ID = id;
                        supObj.Supp_Name = item.Supp_Name;
                        supObj.Supp_Name_AR = item.Supp_Name_AR;
                        supObj.Status = "Active";
                        supObj.Phone = item.Phone;
                        supObj.Address = item.Address;
                        supObj.E_Mail = item.E_Mail;
                        supObj.LastUpdate = DateTime.Now;
                        supObj.acc_no = item.acc_no;
                        supObj.Representative_Name = item.Representative_Name;
                        supObj.Representative_Phone = item.Representative_Phone;
                        supObj.Entered_By = CurrentUserName;
                        supObj.Tax = item.Tax;
                        db.Suppliers.Add(supObj);
                        db.SaveChanges();
                        Session["Supplier"] = null;
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
                return View(supplier);
            }
         //   if (ModelState.IsValid)
         //   {
         //       db.Suppliers.Add(supplier);
         // supplier.Status="Active";
         // supplier.LastUpdate=DateTime.Now;
         //supplier.Entered_By = CurrentUserName;
         //       db.SaveChanges();
         //       return RedirectToAction("Index");
         //   }

            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", supplier.Country_ID);
            ViewBag.acc_no = new SelectList(db.Trees.Where(i => i.Account_Type
                  == "inv"), "Acc_No", "Acc_Name_AR");
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Text", "Value", supplier.Status);
            return View(supplier);
        }

        // GET: /Supp/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Suppliers supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", supplier.Country_ID);
            ViewBag.acc_no = new SelectList(db.Trees.Where(i => i.Account_Type
                 == "inv"), "Acc_No", "Acc_Name_AR", supplier.acc_no);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Text", "Value", supplier.Status);

            return View(supplier);
        }

        // POST: /Supp/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Supp_ID,Supp_Name,Supp_Name_AR,Phone,Address,E_Mail,Web_Site,Representative_Name,Representative_Phone,Status,LastUpdate,Entered_By,Country_ID")] Suppliers supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", supplier.Country_ID);
            return View(supplier);
        }

        // GET: /Supp/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Suppliers supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name");
            ViewBag.acc_no = new SelectList(db.Trees.Where(i => i.Account_Type
                 == "inv"), "Acc_No", "Acc_Name_AR");
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Text", "Value", supplier.Status);
            return View(supplier);
        }

        // POST: /Supp/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Suppliers supplier = db.Suppliers.Find(id);
            supplier.Status = "Deleted";
            db.Entry(supplier).State = EntityState.Modified;
           // db.Suppliers.Remove(supplier);
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
