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
    public class WareHoustTransController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        private NaalaxAuthEntities2 dbuser = new NaalaxAuthEntities2();

        // GET: /WareHoustTrans/
        public ActionResult Index()
        {
            var warehousttrans = db.WareHoustTrans.Include(w => w.WareHouse).Include(w => w.WareHouse1);
            return View(warehousttrans.ToList());
        }

        // GET: /WareHoustTrans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WareHoustTran wareHoustTran = db.WareHoustTrans.Find(id);
            if (wareHoustTran == null)
            {
                return HttpNotFound();
            }
            return View(wareHoustTran);
        }

        // GET: /WareHoustTrans/Create
        public ActionResult Create()
        {
            ViewBag.From_B_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
            ViewBag.To_B_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
            var itemList = from b in db.Items
                           where b.Status == "Active" 
                           select new { desc = b.Product.Name_AR, code = b.Item_ID };
            ViewBag.Item_ID = new SelectList(itemList, "code", "desc");

            Session["transfers"] = null;

            return View();
        }
        public JsonResult Add(string To_B_ID, string Quantity, string Item_ID, string From_B_ID)
        {

            var transfers = new List<WareHoustTran>();
            if (Session["transfers"] != null)
                transfers = (List<WareHoustTran>)Session["transfers"];
            var tranObj = new WareHoustTran();
            if(!string.IsNullOrEmpty(To_B_ID))
            tranObj.To_B_ID = int.Parse(To_B_ID);
            if (!string.IsNullOrEmpty(Quantity))
                tranObj.Quantity = int.Parse(Quantity);
            if (!string.IsNullOrEmpty(Item_ID))
                tranObj.Item_ID = int.Parse(Item_ID);
            if (!string.IsNullOrEmpty(From_B_ID))
                tranObj.From_B_ID = int.Parse(From_B_ID);

            transfers.Add(tranObj);
            Session["transfers"] = transfers;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/WarehoustTrans/Partial/_TranTAddPartial.cshtml", transfers.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }


        // POST: /WareHoustTrans/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,From_B_ID,Item_ID,Quantity,To_B_ID,Entered_By,Status,LastUpdated")] WareHoustTran wareHoustTran)
        {
            try
            {
                if (Session["transfers"] != null)
                {
                    List<WareHoustTran> transfers = (List<WareHoustTran>)Session["transfers"];
                    int id = 0;
                    if (db.WareHoustTrans.Any())
                        id = db.WareHoustTrans.Max(i => i.ID);
                    int idm = 0;
                    if (db.Items.Any())
                        idm = db.Items.Max(i => i.Item_ID);
                    var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();

                    foreach (var item in transfers)
                    {
                        id += 1;
                        item.ID = id;
                        item.Status = "Active";
                        item.LastUpdate = DateTime.Now;
                        item.Entered_By = CurrentUserName;
                        db.WareHoustTrans.Add(item);
                        //update from warehouse
                        Item itemObj = db.Items.Where(i => i.Item_ID == item.Item_ID).SingleOrDefault();
                        itemObj.Qunt -= item.Quantity;
                        itemObj.AvailableQ -= item.Quantity;
                         db.Entry(itemObj).State = EntityState.Modified;
                        db.WareHoustTrans.Add(item);
                        //Add new Item to To warehouse
                        Item newItem = new Item();
                        idm += 1;
                        newItem.Item_ID = idm;
                        newItem.Comp_ID = itemObj.Comp_ID;
                        newItem.Unit_ID = itemObj.Unit_ID;
                        newItem.Item_Grp_ID = itemObj.Item_Grp_ID;
                        newItem.Product_ID = itemObj.Product_ID;
                        newItem.Qunt = item.Quantity;
                        newItem.AvailableQ = item.Quantity;
                        newItem.SoldQ = 0;
                        newItem.PriceForOnce = itemObj.PriceForOnce;
                        newItem.Price_dozen = itemObj.Price_dozen;
                        newItem.OpeningBalance = double.Parse((itemObj.Cost * newItem.Qunt).ToString());
                        newItem.Exp_Date = itemObj.Exp_Date;
                        newItem.Cost = itemObj.Cost;
                        newItem.Status = "Active";
                        newItem.LastUpdate = DateTime.Now;
                        newItem.Entered_By = CurrentUserName;
                        Branch branch = db.Branches.Where(i => i.Branch_ID == user.Branch_ID).SingleOrDefault();
                        newItem.Country_ID = branch.Country_ID;
                      //  if (user.WareHouse_ID != null && user.WareHouse_ID > 0)
                            newItem.Warehouse_ID = item.To_B_ID;
                       // newItem.br
                        db.Items.Add(newItem);

                    }
                    db.SaveChanges();
                    Session["transfers"] = null;
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View();
            }
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.From_B_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
                ViewBag.To_B_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
                ViewBag.Item_ID = new SelectList(db.Items, "Item_ID", "Item_Name", wareHoustTran.Item_ID);

                Session["transfers"] = null;
                return View(wareHoustTran);
            }
            ViewBag.From_B_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
            ViewBag.To_B_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
            ViewBag.Item_ID = new SelectList(db.Items, "Item_ID", "Item_Name");

            Session["transfers"] = null;

            return View(wareHoustTran);
        }

        // GET: /WareHoustTrans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WareHoustTran wareHoustTran = db.WareHoustTrans.Find(id);
            if (wareHoustTran == null)
            {
                return HttpNotFound();
            }
            ViewBag.From_B_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", wareHoustTran.From_B_ID);
            ViewBag.To_B_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", wareHoustTran.To_B_ID);
            var itemList = from b in db.Items
                           where b.Status == "Active"
                           select new { desc = b.Product.Name_AR, code = b.Item_ID };
            ViewBag.Item_ID = new SelectList(itemList, "code", "desc");
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", wareHoustTran.Status);

            return View(wareHoustTran);
        }

        // POST: /WareHoustTrans/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,From_B_ID,Item_ID,Quantity,To_B_ID,Entered_By,Status,LastUpdated")] WareHoustTran wareHoustTran)
        {
            if (ModelState.IsValid)
            {
                wareHoustTran.LastUpdate = DateTime.Now;
                wareHoustTran.Entered_By = CurrentUserName;
                db.Entry(wareHoustTran).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.From_B_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", wareHoustTran.From_B_ID);
            ViewBag.To_B_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", wareHoustTran.To_B_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", wareHoustTran.Status);
            var itemList = from b in db.Items
                           where b.Status == "Active"
                           select new { desc = b.Product.Name_AR, code = b.Item_ID };
            ViewBag.Item_ID = new SelectList(itemList, "code", "desc",wareHoustTran.Item_ID);

            return View(wareHoustTran);
        }
        public JsonResult fromSelected(string From_B_ID)
        {

            try

            {
                int From = int.Parse(From_B_ID);
                var lms = db.Items.Where(o => o.Warehouse_ID==From).ToList();

                var itemList = from b in db.Items
                                             where b.Status == "Active" && b.Warehouse_ID==From
                                             select new { desc = b.Product.Name_AR, code = b.Item_ID };
                var result3 = new SelectList(itemList, "code", "desc");

                return Json(result3, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult checkQuantity(int? Item_ID, int? Qunt)
        {
            var items = new List<WareHoustTran>();
            if (Session["transfers"] != null)
                items = (List<WareHoustTran>)Session["transfers"];
            int Qun = 0;
            var obj = new Item();
            obj = db.Items.Where(i => i.Item_ID == Item_ID).SingleOrDefault();
            var listItem = items.Where(i => i.Item_ID == Item_ID).ToList();
            foreach (var listobj in listItem)
            {
                Qun += int.Parse(listobj.Quantity.ToString());
            }
            if (Qun > 0)
            {
                if (obj.AvailableQ > (Qunt + Qun))
                    return Json("OK", JsonRequestBehavior.AllowGet);
                else
                {
                    return Json("ERR", JsonRequestBehavior.AllowGet);

                }
            }

            if (obj != null && obj.AvailableQ > Qunt)
                return Json("OK", JsonRequestBehavior.AllowGet);
            else
            {
                return Json("ERR", JsonRequestBehavior.AllowGet);

            }
        }

        // GET: /WareHoustTrans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WareHoustTran wareHoustTran = db.WareHoustTrans.Find(id);
            if (wareHoustTran == null)
            {
                return HttpNotFound();
            }
            return View(wareHoustTran);
        }

        // POST: /WareHoustTrans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WareHoustTran wareHoustTran = db.WareHoustTrans.Find(id);
            wareHoustTran.LastUpdate = DateTime.Now;
            wareHoustTran.Entered_By = CurrentUserName;
            wareHoustTran.Status = "Deleted";
            db.Entry(wareHoustTran).State = EntityState.Modified;
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
