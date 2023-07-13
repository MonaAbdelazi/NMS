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

namespace NMS.Controllers
{
    [Authorize(Roles = "Admins")]
    public class CompanyController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Company/
        public ActionResult Index()
        {
            var companies = db.Companies.ToList();//Include(c => c.Activity).Include(c => c.City).Include(c => c.Company_Type).Include(c => c.Country).Include(c => c.Currency);
            return View(companies.ToList());
        }

        // GET: /Company/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }
        //
        public JsonResult Add(int
Activity_ID,int City_ID,string Commercial_No,string Comp_Name,string Comp_Name_AR,int Comp_Type_ID,int Country_ID,int Curr_ID,DateTime IssueDate,DateTime EndDate,HttpPostedFileBase Logo)
        {

            var company = new List<Company>();
            if (Session["Company"] != null)
                company = (List<Company>)Session["Company"];
            var comptObj = new Company();

            comptObj.Activity_ID = Activity_ID;   

            comptObj.City_ID = City_ID;
            comptObj.Commercial_No = Commercial_No;

            comptObj.Comp_Name = Comp_Name;
            comptObj.Comp_Name_AR = Comp_Name_AR;

            comptObj.Comp_Type_ID = Comp_Type_ID;
            comptObj.Country_ID = Country_ID;

            comptObj.Curr_ID = Curr_ID;
            comptObj.EndDate = EndDate;

         //   comptObj.Entered_By = Entered_By;
            comptObj.IssueDate = IssueDate;
            //  comptObj.LastUpdate = LastUpdate;
            string pic = System.IO.Path.GetFileName(Logo.FileName);
            string path = System.IO.Path.Combine(
                Server.MapPath("~/images/profile"), pic);

            Logo.SaveAs(path);
                   comptObj.Logo = Logo.FileName;


            company.Add(comptObj);
            Session["Company"] = company;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Company/Partial/_CompAddPartial.cshtml", company.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // GET: /Company/Create
        public ActionResult Create()
        {
            ViewBag.Activity_ID = new SelectList(db.Activities, "Activity_ID", "Activity_Name");
            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name");
            ViewBag.Comp_Type_ID = new SelectList(db.Company_Type, "Comp_Type_ID", "Name");
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name");
            ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name");
            return View();
        }

        // POST: /Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Comp_ID,Comp_Type_ID,City_ID,Country_ID,Comp_Name,Comp_Name_AR,Logo,Activity_ID,Commercial_No,IssueDate,EndDate,Curr_ID,Status,LastUpdate,Entered_By")] Company company,HttpPostedFileBase Logo)
        {
            if (ModelState.IsValid)
            {
          company.Status="Active";
		  company.LastUpdate=DateTime.Now;
		 company.Entered_By = CurrentUserName;
                //
        
             string pic = System.IO.Path.GetFileName(Logo.FileName);
             string path = System.IO.Path.Combine(
                                    Server.MapPath("~/images/profile"), pic);

             Logo.SaveAs(path);
             
             company.Logo = Logo.FileName;


                 
         
                //
         db.Companies.Add(company);

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Activity_ID = new SelectList(db.Activities, "Activity_ID", "Activity_Name", company.Activity_ID);
            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", company.City_ID);
            ViewBag.Comp_Type_ID = new SelectList(db.Company_Type, "Comp_Type_ID", "Name", company.Comp_Type_ID);
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", company.Country_ID);
            ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name", company.Curr_ID);
            return View(company);
        }

        // GET: /Company/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.Activity_ID = new SelectList(db.Activities, "Activity_ID", "Activity_Name", company.Activity_ID);
            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", company.City_ID);
            ViewBag.Comp_Type_ID = new SelectList(db.Company_Type, "Comp_Type_ID", "Name", company.Comp_Type_ID);
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", company.Country_ID);
            ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name", company.Curr_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", company.Status);
            return View(company);
        }

        // POST: /Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Comp_ID,Comp_Type_ID,City_ID,Country_ID,Comp_Name,Comp_Name_AR,Logo,Activity_ID,Commercial_No,IssueDate,EndDate,Curr_ID,Status,LastUpdate,Entered_By")] Company company, HttpPostedFileBase Logo)
        {
            if (ModelState.IsValid)
            {
                if (Logo != null)
                {
                    string pic = System.IO.Path.GetFileName(Logo.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/images/profile"), pic);

                    Logo.SaveAs(path);

                    company.Logo = Logo.FileName;

                }
                else
                {
                    var cmpx = db.Companies.Where(i => i.Comp_ID == company.Comp_ID).SingleOrDefault();
                    company.Logo = cmpx.Logo;
                }
                

                company.LastUpdate = DateTime.Now;
                company.Entered_By = CurrentUserName;
                //db.Entry(company).State = EntityState.Modified;
                db.Companies.Attach(company);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Activity_ID = new SelectList(db.Activities, "Activity_ID", "Activity_Name", company.Activity_ID);
            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", company.City_ID);
            ViewBag.Comp_Type_ID = new SelectList(db.Company_Type, "Comp_Type_ID", "Name", company.Comp_Type_ID);
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", company.Country_ID);
            ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name", company.Curr_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", company.Status);
            return View(company);
        }

        // GET: /Company/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: /Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
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
