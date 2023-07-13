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
    public class TreesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Trees/

        public JsonResult checkLevel(long ParentId)
        {

            if (ParentId > 0)
            {
                Tree paccount = db.Trees.Where(i => i.SystemId == ParentId).SingleOrDefault();
                if (paccount.Level == 2)
                {
                    return Json("OK", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("False", JsonRequestBehavior.AllowGet);

        }

        public JsonResult Add(string Acc_Name, string Acc_Name_AR, int GL_ID,int Aux_ID, string Type,string Cat)
        {

            var Tree = new List<Tree>();
            if (Session["Tree"] != null)
                Tree = (List<Tree>)Session["Tree"];
            var TreeObj = new Tree();

            TreeObj.Acc_Name = Acc_Name;

            TreeObj.Acc_Name_AR = Acc_Name_AR;
           TreeObj.Account_Type = Type;
            TreeObj.Category = Cat;



            Tree.Add(TreeObj);
            Session["Tree"] = Tree;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Trees/Partial/_TreeAddPartial.cshtml", Tree.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index()
        {
            var trees = db.Trees;
            return View(trees.ToList());
        }

        // GET: /Trees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tree tree = db.Trees.Find(id);
            if (tree == null)
            {
                return HttpNotFound();
            }
            return View(tree);
        }

        // GET: /Trees/Create
        public ActionResult Create()
        {
            ViewBag.Aux_ID = new SelectList(db.Auxiliaries, "Aux_ID", "Aux_Name");
            ViewBag.GL_ID = new SelectList(db.GLs, "GL_ID", "Genreal_Name");
            ViewBag.Type = new SelectList(string.Empty, "Value", "Text");
            ViewBag.Cat = new SelectList(CommonUtils.getacccat(), "Value", "Text");
            ViewBag.ParentId = new SelectList(db.Trees, "SystemId", "Acc_Name"); 
            ViewBag.currencyId = new SelectList(db.Currencies, "Curr_ID", "Curr_Name");
            ViewBag.Acc_Nature = new SelectList(CommonUtils.getaccNat(), "Value", "Text");

            return View();
        }
        
              public JsonResult catPSelected(string Category)
        {
            try
            {
                   return Json(new SelectList(db.Trees.Where(i=>i.Category==Category), "SystemId", "Acc_Name"), JsonRequestBehavior.AllowGet);
                 
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult catSelected(string Category)
        {
            try { 
               switch (Category)
                {
                    case "A":
                    return Json(CommonUtils.getAssetTypes(), JsonRequestBehavior.AllowGet);
                    break;
                    case "E":
                    return Json(CommonUtils.getExpensesTypes(), JsonRequestBehavior.AllowGet);

                    break;
                    case "L":
                    return Json(CommonUtils.getLiabilitiesTypes(), JsonRequestBehavior.AllowGet);

                    break;
                    case "Eq":
                    return Json(CommonUtils.getEqutitiesypes(), JsonRequestBehavior.AllowGet);

                    break;
                    case "I":
                        return Json(CommonUtils.getIncomeypes(), JsonRequestBehavior.AllowGet);

                        break;
                    default:
                    return Json("", JsonRequestBehavior.AllowGet);

                    break;
                }
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }

        // POST: /Trees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tree tree , bool Leave)
        {
            try
            {
                int? level = 0;
                if (tree.ParentId > 0)
                {
                    Tree paccount = db.Trees.Where(i => i.SystemId == tree.ParentId).SingleOrDefault();
                    level = paccount.Level;
                   
                }
                level += 1;
                if (level > 3)
                {
                    CommonUtils.SetFeedback("Level Reached", Feedback.Feedback_Error);
                    return View();
                }
                long id = 0;
                if (db.Trees.Any())
                   id = db.Trees.Max(i => i.SystemId);
                id += 1;
                tree.SystemId = id;
                tree.Level = level;
                if (Leave == false)
                    tree.Leave = "F";
                if (Leave == true)
                    tree.Leave = "T";

                tree.Status = "Active";
                tree.LastUpdate = DateTime.Now;
                tree.Entered_By = CurrentUserName;
                db.Trees.Add(tree);
                db.SaveChanges();
                CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
               return RedirectToAction("Index");
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

        // GET: /Trees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tree tree = db.Trees.Find(id);
            if (tree == null)
            {
                return HttpNotFound();
            }

            ViewBag.Types = new SelectList(string.Empty, "Value", "Text", tree.Account_Type.Trim());
            string cat = tree.Category.Trim();
            ViewBag.Cats = new SelectList(CommonUtils.getacccat(), "Value", "Text", cat);
            ViewBag.ParentIds = new SelectList(db.Trees, "SystemId", "Acc_Name", tree.ParentId);
            ViewBag.currencyIds = new SelectList(db.Currencies, "Curr_ID", "Curr_Name", tree.currencyId);
            ViewBag.Acc_Natures = new SelectList(CommonUtils.getaccNat(), "Value", "Text", tree.Acc_Nature.Trim());
            ViewBag.Statuss = new SelectList(CommonUtils.getStatus(), "Value", "Text", tree.Status.Trim());
            if (tree.Leave == "T")
                ViewBag.Leave = true;
            if (tree.Leave == "F")
                ViewBag.Leave = false;
            return View(tree);
        }

        // POST: /Trees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Acc_No,GL_ID,Aux_ID,Acc_Name,Acc_Name_AR,Type,Status,LastUpdate,Entered_By,Cat")] Tree tree)
        {
            if (ModelState.IsValid)
            {
                tree.LastUpdate = DateTime.Now;
                tree.Entered_By = CurrentUserName;
               
                db.Entry(tree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
       
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", tree.Status);
            return View(tree);
        }

        // GET: /Trees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tree tree = db.Trees.Find(id);
            if (tree == null)
            {
                return HttpNotFound();
            }
            ViewBag.Types = new SelectList(string.Empty, "Value", "Text", tree.Account_Type.Trim());
            string cat = tree.Category.Trim();
            ViewBag.Cats = new SelectList(CommonUtils.getacccat(), "Value", "Text", cat);
            ViewBag.ParentIds = new SelectList(db.Trees, "SystemId", "Acc_Name", tree.ParentId);
            ViewBag.currencyIds = new SelectList(db.Currencies, "Curr_ID", "Curr_Name", tree.currencyId);
            ViewBag.Acc_Natures = new SelectList(CommonUtils.getaccNat(), "Value", "Text", tree.Acc_Nature.Trim());
            ViewBag.Statuss = new SelectList(CommonUtils.getStatus(), "Value", "Text", tree.Status.Trim());
            if (tree.Leave == "T")
                ViewBag.Leave = true;
            if (tree.Leave == "F")
                ViewBag.Leave = false;
            return View(tree);
        }

        // POST: /Trees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tree tree = db.Trees.Find(id);

            tree.LastUpdate = DateTime.Now;
            tree.Entered_By = CurrentUserName;
            tree.Status = "Deleted";
            db.Entry(tree).State = EntityState.Modified;
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
