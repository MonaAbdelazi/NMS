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
    public class CountriesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Countries/
        //Json-Ajax
        public JsonResult Add(string name, string namear,string CountryKey)
        {

            var Contry = new List<Country>();
            if (Session["Country"] != null)
                Contry = (List<Country>)Session["Country"];
            var cntryObj = new Country();

            cntryObj.Name = name;

            cntryObj.Name_AR = namear;
            cntryObj.CountryKey = CountryKey;



            Contry.Add(cntryObj);
            Session["Country"] = Contry;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Countries/Partial/_CountriesAddPartial.cshtml", Contry.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View(db.Countries.Where(i=>i.Status=="Active").ToList());
        }

        // GET: /Countries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = db.Countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // GET: /Countries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Country_ID,Name,Name_AR,CountryKey,Status,LastUpdate,Entered_By")] Country country)
        {
            try
            {
                if (Session["Country"] != null)
                {
                    List<Country> coutrylist = (List<Country>)Session["Country"];
                    int id = 0;
                    if (db.Countries.Any())
                        id = db.Countries.Max(i => i.Country_ID);

                    foreach (var item in coutrylist)
                    {
                        var counObj = new Country();
                        id += 1;
                        counObj.Country_ID = id;
                        counObj.Name = item.Name;
                        counObj.Name_AR = item.Name_AR;
                        counObj.CountryKey = item.CountryKey;
                        counObj.Status = "Active";
                        counObj.LastUpdate = DateTime.Now;
                        counObj.Entered_By = CurrentUserName;
                        db.Countries.Add(counObj);
                        db.SaveChanges();
                        Session["Country"] = null;
                    }
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
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
        }

        // GET: /Countries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = db.Countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", country.Status);
            return View(country);
        }

        // POST: /Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Country_ID,Name,Name_AR,CountryKey,Status,LastUpdate,Entered_By")] Country country)
        {
            if (ModelState.IsValid)
            {
                country.LastUpdate = DateTime.Now;
                country.Entered_By = CurrentUserName;
                db.Entry(country).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(country);
        }

        // GET: /Countries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = db.Countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", country.Status);
            return View(country);
        }

        // POST: /Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Country country = db.Countries.Find(id);
            country.Status = "Deleted";
            db.Entry(country).State = EntityState.Modified;
            //  db.Countries.Remove(country);
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
