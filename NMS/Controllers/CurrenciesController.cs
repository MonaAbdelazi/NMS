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
    public class CurrenciesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Currencies/
        public ActionResult Index()
        {
            return View(db.Currencies.Where(i => i.Status == CommonUtils.STATUS_ACTIVE).ToList());
        }

        // GET: /Currencies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = db.Currencies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // GET: /Currencies/Create
        public ActionResult Create()
        {
            Session["currencies"] = null;

            return View();
        }
        public JsonResult Add(string name, string namear, string CurrentCost,string EX_Rate,string Last_Ex_Rare_Date)
        {

            var currencies = new List<Currency>();
            if (Session["currencies"] != null)
                currencies = (List<Currency>)Session["currencies"];
            var currObj = new Currency();

            currObj.Curr_Name = name;
            if(!string.IsNullOrEmpty(CurrentCost))
            currObj.CurrentCost =double.Parse(CurrentCost);
            if (!string.IsNullOrEmpty(EX_Rate))
                currObj.EX_Rate =double.Parse(EX_Rate);
            if (!string.IsNullOrEmpty(Last_Ex_Rare_Date))
                currObj.Last_Ex_Rare_Date =DateTime.Parse(Last_Ex_Rare_Date);
            currObj.Curr_Name_AR = namear;
            currencies.Add(currObj);
            Session["currencies"] = currencies;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Currencies/Partial/_CurrTAddPartial.cshtml", currencies.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }

        // POST: /Currencies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Currency currency)
        {
            try
            {
                if (Session["currencies"] != null)
                {
                    List<Currency> currencylist = (List<Currency>)Session["currencies"];
                    int id = 0;
                    if (db.Currencies.Any())
                        id = db.Currencies.Max(i => i.Curr_ID);

                    foreach (var item in currencylist)
                    {
                        var curObj = new Currency();
                        id += 1;
                        curObj.Curr_ID = id;
                        curObj.Curr_Name = item.Curr_Name;
                        curObj.Curr_Name_AR = item.Curr_Name_AR;
                        curObj.CurrentCost = item.CurrentCost;
                        curObj.EX_Rate = item.EX_Rate;
                        curObj.Status = "Active";
                        curObj.LastUpdate = DateTime.Now;
                        curObj.Entered_By = CurrentUserName;
                        db.Currencies.Add(curObj);
                        db.SaveChanges();
                    }
                    Session["currencies"] = null;

                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View();
            }
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(currency);
            }


            return View(currency);
        }


        // GET: /Currencies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = db.Currencies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", currency.Status);

            return View(currency);
        }

        // POST: /Currencies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Curr_ID,Curr_Name,Curr_Name_AR,CurrentCost,EX_Rate,Last_Ex_Rare_Date,Status,LastUpdate,Entered_By")] Currency currency)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    currency.LastUpdate = DateTime.Now;
                    currency.Entered_By = CurrentUserName;
                    db.Entry(currency).State = EntityState.Modified;
                    db.SaveChanges();
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);

                    return RedirectToAction("Index");
                }
                return View(currency);
            }
            catch (DbEntityValidationException ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(currency);
            }
        }

        // GET: /Currencies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Currency currency = db.Currencies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // POST: /Currencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Currency currency = db.Currencies.Find(id);
            currency.LastUpdate = DateTime.Now;
            currency.Entered_By = CurrentUserName;
            currency.Status = "Deleted";
            db.Entry(currency).State = EntityState.Modified;

            CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);

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
