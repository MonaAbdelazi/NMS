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
   [Authorize(Roles = "Casher,Admins")]
    public class BranchesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        private NaalaxAuthEntities2 dbuser = new NaalaxAuthEntities2();


        public JsonResult CountrySelected(string Country_ID)
        {

            try

            {
                int Country = int.Parse(Country_ID);
                var lms = db.Cities.Where(o => o.Country_ID == Country).ToList();
                              
                var result3 = new SelectList(lms, "City_ID", "Name_AR");

                return Json(result3, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        // GET: /Branches/
        //Ajax-Json
        public JsonResult Add(string Branch_Name, string Branch_Name_AR, int Country_ID,int City_ID,DateTime Opening_Date, string Location, string Comment)
        {

             var Branch = new List<Branch>();
            if (Session["Branch"] != null)
                Branch = (List<Branch>)Session["Branch"];
            var BranchObj = new Branch();

            BranchObj.Branch_Name = Branch_Name;

            BranchObj.Branch_Name_AR = Branch_Name_AR;
            BranchObj.Country_ID = Country_ID;
            BranchObj.City_ID = City_ID;

            BranchObj.Opening_Date = Opening_Date;
            BranchObj.Location = Location;
            BranchObj.Comment = Comment;



            Branch.Add(BranchObj);
            Session["Branch"] = Branch;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Branches/Partial/_BranchAddPartial.cshtml", Branch.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            var branches = db.Branches.Where(i=>i.Status=="Active").Include(b => b.City).Include(b => b.Country);
            return View(branches.ToList());
        }

        // GET: /Branches/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // GET: /Branches/Create
        public ActionResult Create()
        {
            ViewBag.City_ID = new SelectList(db.Cities.Where(i => i.Status == "Active"), "City_ID", "Name");
            ViewBag.Country_ID = new SelectList(db.Countries.Where(i => i.Status == "Active"), "Country_ID", "Name");
            return View();
        }

        // POST: /Branches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Branch_ID,City_ID,Country_ID,Branch_Name,Branch_Name_AR,Opening_Date,Location,Comment,Status,LastUpdate,Entered_By")] Branch branch)
        {
            try
            {
                if (Session["Branch"] != null)
                {
                    List<Branch> Branchist = (List<Branch>)Session["Branch"];
                    int id = 0;
                    if (db.Branches.Any())
                        id = db.Branches.Max(i => i.Branch_ID);

                    foreach (var item in Branchist)
                    {
                        var BranchObj = new Branch();
                        id += 1;
                        BranchObj.Country_ID = id;
                        BranchObj.Branch_Name = item.Branch_Name;
                        BranchObj.Branch_Name_AR = item.Branch_Name_AR;
                        BranchObj.Country_ID = item.Country_ID;
                        BranchObj.City_ID = item.City_ID;
                        BranchObj.Opening_Date = item.Opening_Date;
                        BranchObj.Location = item.Location;
                        BranchObj.Comment = item.Comment;
                        BranchObj.Status = "Active";
                        BranchObj.LastUpdate = DateTime.Now;
                        BranchObj.Entered_By = CurrentUserName;
                        db.Branches.Add(BranchObj);
                        db.SaveChanges();
                        Session["Branch"] = null;
                    }
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Country_ID = new SelectList(db.Countries.Where(i => i.Status == "Active"), "Country_ID", "Name", branch.Country_ID);
                ViewBag.City_ID = new SelectList(db.Cities.Where(i => i.Status == "Active"), "City_ID", "Name", branch.City_ID);
                return View(branch);

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

        // GET: /Branches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            ViewBag.City_ID = new SelectList(db.Cities.Where(i => i.Status == "Active"), "City_ID", "Name", branch.City_ID);
            ViewBag.Country_ID = new SelectList(db.Countries.Where(i => i.Status == "Active"), "Country_ID", "Name", branch.Country_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", branch.Status);
            return View(branch);
        }

        // POST: /Branches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Branch_ID,City_ID,Country_ID,Branch_Name,Branch_Name_AR,Opening_Date,Location,Comment,Status,LastUpdate,Entered_By")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                branch.LastUpdate = DateTime.Now;
                branch.Entered_By = CurrentUserName;
                db.Entry(branch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.City_ID = new SelectList(db.Cities.Where(i => i.Status == "Active"), "City_ID", "Name", branch.City_ID);
            ViewBag.Country_ID = new SelectList(db.Countries.Where(i => i.Status == "Active"), "Country_ID", "Name", branch.Country_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", branch.Status);
            return View(branch);
        }

        // GET: /Branches/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = db.Branches.Find(id);
            AspNetUser usr = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();

            if (branch == null || branch.Branch_ID == usr.Branch_ID)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", branch.Status);
            ViewBag.City_ID = new SelectList(db.Cities.Where(i => i.Status == "Active"), "City_ID", "Name", branch.City_ID);
            ViewBag.Country_ID = new SelectList(db.Countries.Where(i => i.Status == "Active"), "Country_ID", "Name", branch.Country_ID);

            return View(branch);
        }

        // POST: /Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Branch branch = db.Branches.Find(id);
            branch.LastUpdate = DateTime.Now;
            branch.Entered_By = CurrentUserName;
            branch.Status = "Deleted";
            db.Entry(branch).State = EntityState.Modified;
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
