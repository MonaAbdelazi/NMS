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
    public class Items_GroupController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Items_Group/
        public ActionResult Index()
        {
            return View(db.Items_Group.Where(i=>i.Status==CommonUtils.STATUS_ACTIVE).ToList());
        }

        // GET: /Items_Group/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items_Group items_Group = db.Items_Group.Find(id);
            if (items_Group == null)
            {
                return HttpNotFound();
            }
            return View(items_Group);
        }

        // GET: /Items_Group/Create
        public ActionResult Create()
        {
            Session["groups"] = null;
            ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name");
            ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name");
            return View();
        }

        // POST: /Items_Group/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Items_Group items_Group)
        {
            try
            {
                if (Session["groups"] != null)
                {
                    List<Items_Group> grouplist = (List<Items_Group>)Session["groups"];
                    int id = 0;
                    if (db.Items_Group.Any())
                        id = db.Items_Group.Max(i => i.Item_Grp_ID);

                    foreach (var item in grouplist)
                    {
                        var groupObj = new Items_Group();
                        id += 1;
                        groupObj.Item_Grp_ID = id;
                        groupObj.Item_Grp_Name = item.Item_Grp_Name;
                        groupObj.Item_GrpName_AR = item.Item_GrpName_AR;
                        groupObj.Comment = item.Comment;
                        groupObj.Status = "Active";
                        groupObj.LastUpdate = DateTime.Now;
                        groupObj.Entered_By = CurrentUserName;
                        db.Items_Group.Add(groupObj);
                        db.SaveChanges();
                    }
                    Session["groups"] = null;

                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                return View();
            }
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                return View(items_Group);
            }

            ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name");
            ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name");
            return View(items_Group);
        }
        public JsonResult Add(string name, string namear, string Comment)
        {

            var groups = new List<Items_Group>();
            if (Session["groups"] != null)
                groups = (List<Items_Group>)Session["groups"];
            var groupObj = new Items_Group();
            groupObj.Item_Grp_Name = name;
            groupObj.Item_GrpName_AR = namear;
            groupObj.Comment = Comment;
           
            groups.Add(groupObj);
            Session["groups"] = groups;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Items_Group/Partial/_GroupTAddPartial.cshtml", groups.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        // GET: /Items_Group/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items_Group items_Group = db.Items_Group.Find(id);
            
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", items_Group.Status);

            if (items_Group == null)
            {
                return HttpNotFound();
            }
            return View(items_Group);
        }

        // POST: /Items_Group/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Tree, string Tree1,[Bind(Include="Item_Grp_ID,Item_Grp_Name,Item_GrpName_AR,Comment,Status,LastUpdate,Entered_By,Tree,Tree1")] Items_Group items_Group)
        {
            //  if (ModelState.IsValid)
            try
            {
                if (!string.IsNullOrEmpty(Tree))
                {
                    int ac = int.Parse(Tree);
                    items_Group.Income_Acc = ac;// db.Trees.Where(i => i.Acc_No == ac).SingleOrDefault();
                }
                if (!string.IsNullOrEmpty(Tree1))
                {
                    int ac1 = int.Parse(Tree1);
                    items_Group.Expenses_Acc = ac1;// db.Trees.Where(i => i.Acc_No == ac1).SingleOrDefault();
                }
                items_Group.LastUpdate = DateTime.Now;
                items_Group.Entered_By = CurrentUserName;
                db.Entry(items_Group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
             
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", items_Group.Status);

                return View(items_Group);
    }
 
            return View(items_Group);
        }

        // GET: /Items_Group/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Items_Group items_Group = db.Items_Group.Find(id);
            if (items_Group == null)
            {
                return HttpNotFound();
            }
            return View(items_Group);
        }

        // POST: /Items_Group/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Items_Group items_Group = db.Items_Group.Find(id);
            items_Group.LastUpdate = DateTime.Now;
            items_Group.Entered_By = CurrentUserName;
            items_Group.Status = "Deleted";
            db.Entry(items_Group).State = EntityState.Modified;
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
