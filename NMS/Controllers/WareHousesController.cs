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
    public class WareHousesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        private NaalaxAuthEntities2 dbuser = new NaalaxAuthEntities2();

        // GET: /WareHouses/
        public ActionResult Index()
        {
            var warehouses = db.WareHouses.Include(w => w.Branch).Include(w => w.Employess);
            return View(warehouses.Where(i=>i.Status==CommonUtils.STATUS_ACTIVE).ToList());
        }

        // GET: /WareHouses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WareHouse wareHouse = db.WareHouses.Find(id);
            if (wareHouse == null)
            {
                return HttpNotFound();
            }
            return View(wareHouse);
        }

        // GET: /WareHouses/Create
        public ActionResult Create()
        {
            Session["wares"] = null;
            ViewBag.Branch_ID = new SelectList(db.Branches.Where(i=>i.Status=="Active"), "Branch_ID", "Branch_Name");
            ViewBag.Emp_ID = new SelectList(db.Employesses.Where(i => i.Status == "Active"), "Emp_ID", "Name");
            return View();
        }

        // POST: /WareHouses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( WareHouse wareHouse)
        {
            try
            {
                if (Session["wares"] != null)
                {
                    List<WareHouse> warelist = (List<WareHouse>)Session["wares"];
                    int id = 0;
                    if (db.WareHouses.Any())
                        id = db.WareHouses.Max(i => i.Warehouse_ID);

                    foreach (var item in warelist)
                    {
                        var wareObj = new WareHouse();
                        id += 1;
                        wareObj.Warehouse_ID = id;
                        wareObj.Name = item.Name;
                        wareObj.Name_AR = item.Name_AR;
                        wareObj.Keeper_Phone = item.Keeper_Phone;
                        wareObj.Size = item.Size;
                        wareObj.Location = item.Location;
                        wareObj.Emp_ID = item.Emp_ID;
                        wareObj.Employess = item.Employess;
                        wareObj.Branch_ID = item.Branch_ID;
                        wareObj.Status = "Active";
                        wareObj.LastUpdate = DateTime.Now;
                        wareObj.Entered_By = CurrentUserName;
                        db.WareHouses.Add(wareObj);
                        db.SaveChanges();
                        Session["wares"] = null;
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
                return View(wareHouse);
            }


            return View(wareHouse);
        }
        public JsonResult Add(string name, string namear, string Branch_ID,string Location, string Size )
        {

            var wares =  new List<WareHouse>();
            if (Session["wares"] != null)
                wares = (List<WareHouse>)Session["wares"];
            var wareObj = new WareHouse();
            wareObj.Name = name;
            wareObj.Name_AR = namear;
            if (!string.IsNullOrEmpty(Branch_ID)) {
                wareObj.Branch_ID = int.Parse(Branch_ID);
                wareObj.Branch = db.Branches.Where(i => i.Branch_ID == wareObj.Branch_ID).SingleOrDefault();
            }
            wareObj.Location = Location;
            if (!string.IsNullOrEmpty(Size))
               wareObj.Size = int.Parse(Size);
           
            wares.Add(wareObj);
            Session["wares"] = wares;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/WareHouses/Partial/_WareTAddPartial.cshtml", wares.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // GET: /WareHouses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WareHouse wareHouse = db.WareHouses.Find(id);
            if (wareHouse == null)
            {
                return HttpNotFound();
            }
            ViewBag.Branch_ID = new SelectList(db.Branches.Where(i => i.Status == "Active"), "Branch_ID", "Branch_Name", wareHouse.Branch_ID);
            ViewBag.Emp_ID = new SelectList(db.Employesses.Where(i => i.Status == "Active"), "Emp_ID", "Name", wareHouse.Emp_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", wareHouse.Status);

            return View(wareHouse);
        }

        // POST: /WareHouses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Warehouse_ID,Name,Name_AR,Emp_ID,Keeper_Phone,Size,Location,Entered_By,LastUpdate,Status,Branch_ID")] WareHouse wareHouse)
        {
            if (ModelState.IsValid)
            {
                wareHouse.LastUpdate = DateTime.Now;
                wareHouse.Entered_By = CurrentUserName;

                db.Entry(wareHouse).State = EntityState.Modified;
                db.SaveChanges();
                CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);

                return RedirectToAction("Index");
            }
            ViewBag.Branch_ID = new SelectList(db.Branches.Where(i => i.Status == "Active"), "Branch_ID", "Branch_Name", wareHouse.Branch_ID);
            ViewBag.Emp_ID = new SelectList(db.Employesses.Where(i => i.Status == "Active"), "Emp_ID", "Name", wareHouse.Emp_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", wareHouse.Status);

            return View(wareHouse);
        }

        // GET: /WareHouses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WareHouse wareHouse = db.WareHouses.Find(id);
            AspNetUser usr = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
            if (wareHouse == null || wareHouse.Warehouse_ID == usr.WareHouse_ID)
            {
                return HttpNotFound();
            }
            ViewBag.Branch_ID = new SelectList(db.Branches.Where(i => i.Status == "Active"), "Branch_ID", "Branch_Name", wareHouse.Branch_ID);
            ViewBag.Emp_ID = new SelectList(db.Employesses.Where(i => i.Status == "Active"), "Emp_ID", "Name", wareHouse.Emp_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", wareHouse.Status);

            return View(wareHouse);
        }

        // POST: /WareHouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WareHouse wareHouse = db.WareHouses.Find(id);
            wareHouse.LastUpdate = DateTime.Now;
            wareHouse.Entered_By = CurrentUserName;
            wareHouse.Status = "Deleted";
            db.Entry(wareHouse).State = EntityState.Modified;


            db.SaveChanges();
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
