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
    public class ProductsController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();

        // GET: /Products/
        public ActionResult Index()
        {
            return View(db.Products.Where(i=>i.Status==CommonUtils.STATUS_ACTIVE).ToList());
        }

        // GET: /Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: /Products/Create
        public ActionResult Create()
        {
            Session["products"] = null;

            ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name");
            ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name"); 
            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_GrpName_AR");

            return View();
        }
        public JsonResult Add(string name, string namear, string BAR_CODE,string Comment, string Tree, string Tree1,string Item_Grp_ID)
        {

            var products = new List<Product>();
            if (Session["products"] != null)
                products = (List<Product>)Session["products"];
            var productObj = new Product();
            productObj.Name = name;
            productObj.Name_AR = namear;
            productObj.BAR_CODE = BAR_CODE;
            productObj.Comment = Comment;
            if (!string.IsNullOrEmpty(Tree))
            {
                int ac = int.Parse(Tree);
                productObj.Income_Acc = ac;// db.Trees.Where(i => i.Acc_No == ac).SingleOrDefault();
            }
            if (!string.IsNullOrEmpty(Tree1))
            {
                int ac1 = int.Parse(Tree1);
                productObj.Expenses_Acc = ac1;// db.Trees.Where(i => i.Acc_No == ac1).SingleOrDefault();
            }
            if (!string.IsNullOrEmpty(Item_Grp_ID))
            {
                int group = int.Parse(Item_Grp_ID);
                productObj.Item_Grp_ID = group; //db.Items_Group.Where(i => i.Item_Grp_ID == group).SingleOrDefault();
            }
            products.Add(productObj);
            Session["products"] = products;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Products/Partial/_ProductTAddPartial.cshtml", products.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }

        // POST: /Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Product product)
        {
            try
            {
                if (Session["products"] != null)
                {
                    List<Product> productlist = (List<Product>)Session["products"];
                    int id = 0;
                    if (db.Products.Any())
                        id = db.Products.Max(i => i.Product_ID);

                    foreach (var item in productlist)
                    {
                        var productObj = new Product();
                        id += 1;
                        productObj.Product_ID = id;
                        productObj.Name = item.Name;
                        productObj.Name_AR = item.Name_AR;
                        productObj.BAR_CODE = item.BAR_CODE;
                        productObj.Income_Acc = item.Income_Acc;
                        productObj.Expenses_Acc = item.Expenses_Acc;
                        productObj.Comment = item.Comment;
                        productObj.Item_Grp_ID = item.Item_Grp_ID;
                        productObj.Status = "Active";
                        productObj.LastUpdate = DateTime.Now;
                        productObj.Entered_By = CurrentUserName;
                        db.Products.Add(productObj);
                        db.SaveChanges();
                    }
                    Session["products"] = null;

                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_GrpName_AR");

                return View();
            }
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_GrpName_AR");

                return View(product);
            }

            ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name");
            ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name");
            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_GrpName_AR");

            return View(product);
        }

        public JsonResult getGroup(int? Item_Grp_ID)
        {
            var obj = new Items_Group();
            obj = db.Items_Group.Where(i => i.Item_Grp_ID == Item_Grp_ID).SingleOrDefault();
            if(obj!=null && obj.Income_Acc!=null&& obj.Income_Acc>0 && obj.Expenses_Acc!=null&& obj.Expenses_Acc>0)
                return Json("Suc", JsonRequestBehavior.AllowGet);


            return Json("error", JsonRequestBehavior.AllowGet);
        }

        // GET: /Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", product.Status);
            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_GrpName_AR", product.Item_Grp_ID);
            ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name", product.Income_Acc);
            ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name", product.Expenses_Acc);

            return View(product);
        }

        // POST: /Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include= "Product_ID,Name,Name_AR,BAR_CODE,Comment,Status,LastUpdate,Entered_By,Item_Grp_ID")] Product product)
        {
            if (ModelState.IsValid)
            {
                try { 
                product.LastUpdate = DateTime.Now;
                product.Entered_By = CurrentUserName;

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
              //
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
                //
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.Tree = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                ViewBag.Tree1 = new SelectList(db.Trees, "Acc_No", "Acc_Name");
                    ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", product.Status);
                    ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_GrpName_AR", product.Item_Grp_ID);

                    return View(product);
            }
       // }
            return View(product);
        }

        // GET: /Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", product.Status);
            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_GrpName_AR", product.Item_Grp_ID);

            return View(product);
        }

        // POST: /Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            product.LastUpdate = DateTime.Now;
            product.Entered_By = CurrentUserName;
            product.Status = "Deleted";
            db.Entry(product).State = EntityState.Modified; db.SaveChanges();
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
