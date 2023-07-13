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
    public class CitiesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Cities/
        public JsonResult Add(string name, string namear, int Country_ID)
        {

            var City = new List<City>();
            if (Session["City"] != null)
                City = (List<City>)Session["City"];
            var CityObj = new City();

            CityObj.Name = name;

            CityObj.Name_AR = namear;
            CityObj.Country_ID = Country_ID;



            City.Add(CityObj);
            Session["City"] = City;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Cities/Partial/_CitiesAddPartial.cshtml", City.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            var cities = db.Cities.Include(c => c.Country);
            return View(cities.ToList());
        }

        // GET: /Cities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = db.Cities.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // GET: /Cities/Create
        public ActionResult Create()
        {
            ViewBag.Country_ID = new SelectList(db.Countries.Where(i=>i.Status=="Active"), "Country_ID", "Name");
            return View();
        }

        // POST: /Cities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="City_ID,Name,Name_AR,Country_ID,Status,LastUpdate,Entered_By")] City city)
        {
            try
            {
                if (Session["City"] != null)
                {
                    List<City> cityist = (List<City>)Session["City"];
                    int id = 0;
                    if (db.Cities.Any())
                        id = db.Cities.Max(i => i.City_ID);

                    foreach (var item in cityist)
                    {
                        var CityObj = new City();
                        id += 1;
                        CityObj.Country_ID = id;
                        CityObj.Name = item.Name;
                        CityObj.Name_AR = item.Name_AR;
                        CityObj.Country_ID = item.Country_ID;
                        CityObj.Status = "Active";
                        CityObj.LastUpdate = DateTime.Now;
                        CityObj.Entered_By = CurrentUserName;
                        db.Cities.Add(CityObj);
                        db.SaveChanges();
                        Session["City"] = null;
                    }
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Country_ID = new SelectList(db.Countries.Where(i => i.Status == "Active"), "Country_ID", "Name", city.Country_ID);
                return View(city);
 
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

        // GET: /Cities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = db.Cities.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            ViewBag.Country_ID = new SelectList(db.Countries.Where(i => i.Status == "Active"), "Country_ID", "Name", city.Country_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", city.Status);
            return View(city);
        }

        // POST: /Cities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="City_ID,Name,Name_AR,Country_ID,Status,LastUpdate,Entered_By")] City city)
        {
            if (ModelState.IsValid)
            {
                city.Entered_By = CurrentUserName;
                city.LastUpdate = DateTime.Now;
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Country_ID = new SelectList(db.Countries.Where(i => i.Status == "Active"), "Country_ID", "Name", city.Country_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", city.Status);
            return View(city);
        }

        // GET: /Cities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = db.Cities.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            ViewBag.Country_ID = new SelectList(db.Countries.Where(i => i.Status == "Active"), "Country_ID", "Name", city.Country_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", city.Status);
            return View(city);
        }

        // POST: /Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            City city = db.Cities.Find(id);
            city.Entered_By = CurrentUserName;
            city.LastUpdate = DateTime.Now;
            city.Status = "Deleted";
            db.Entry(city).State = EntityState.Modified;
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
