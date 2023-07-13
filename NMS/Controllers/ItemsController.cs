//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using NMS.Tools;
//using NMS.Data;
//using NMS.Utility;
//using NMS.Core.Resources;
//using System.Data.Entity.Validation;

//namespace NMS.Controllers
//{

//    public class ItemsController : BaseController
//    {
//        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
//        private NaalaxAuthEntities2 dbuser = new NaalaxAuthEntities2();

//        // GET: /Items/
//        public ActionResult Index()
//        {
//            var ware = Convert.ToInt32( Session["WareHouse_ID"]);
//            var items = db.Items.Where(i => i.Status != "Deleted");
//            return View(items.Where(i=>i.Warehouse_ID== ware).ToList());
//        }
//        public ActionResult IndexApprove()
//        {
//            var invoices = db.InItemsInvoices.Where(i => i.Status == "Pending");
//            return View(invoices.ToList());
//            //&&i.Entered_By!=CurrentUserName
//        }

//        // GET: /Items/Details/5
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Item item = db.Items.Find(id);
//            if (item == null)
//            {
//                return HttpNotFound();
//            }
//            return View(item);
//        }

//        // GET: /Items/Create
//        public ActionResult Create()
//        {
//            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name");
//            ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name");
//            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name");
//            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name");
//            ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name"); 
//            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
//            ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name");
//            Session["items"] = null;
//            return View();
//        }
//        public ActionResult Approve(int? id)
//        {
//            Session["items"] = null;

//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            InItemsInvoice invoice = db.InItemsInvoices.Where(i => i.ID == id).SingleOrDefault();
//            if (invoice == null)
//            {
//                return HttpNotFound();
//            }
//            Session["list"] = invoice.Items.ToList();
//            var items = new List<Item>();
//            if (Session["list"] != null)
//                items = (List<Item>)Session["list"];

//            return View(invoice);
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Approve(List<Item> invoicelist)
//        {
//            try
//            {
//                if (Session["list"] != null)
//                {
//                    List<Item> list = (List<Item>)Session["list"];
//                    Item item = list[0];
//                    InItemsInvoice inv = db.InItemsInvoices.Where(i => i.ID== item.InItemsInvId).SingleOrDefault();
//                    Session["InItemsInvId"] = item.InItemsInvId;
//                    inv.Status = "Active";
//                    inv.lastUpdated = DateTime.Now;
//                    inv.Approved_By = CurrentUserName;
//                    foreach (var obj in inv.Items.ToList())
//                    {
//                        obj.Status = "Active";
//                        obj.LastUpdate= DateTime.Now;
//                        // db.Entry(obj).State = EntityState.Modified;

//                    }
//                    db.Entry(inv).State = EntityState.Modified;

//                }
//                db.SaveChanges();
//                Session["list"] = null;
//                CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);

//                return RedirectToAction("IndexApprove");
//            }
//            catch (Exception ex)
//            {
//                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
//                return View(invoicelist);
//            }



//        }


//        public JsonResult Add(string InSirkNo,string Cost,  string PriceForOnce, string Qunt,string Item_Grp_ID,string Product_ID )
//        {

//            var items = new List<Item>();
//            if (Session["items"] != null)
//                items = (List<Item>)Session["items"];
//            var ItemObj = new Item();
//            if (!string.IsNullOrEmpty(Cost))
//            {
//                int amy = int.Parse(Cost);
//                ItemObj.Cost = amy;
//            }
//           ItemObj.InItemsInvoice = new InItemsInvoice();
//            if (!string.IsNullOrEmpty(InSirkNo))
//                ItemObj.InItemsInvoice.InSirkNo = int.Parse(InSirkNo);
//            if (!string.IsNullOrEmpty(PriceForOnce))
//            {
//                int dozen = int.Parse(PriceForOnce);
//                ItemObj.PriceForOnce = dozen;
//            }
//            if (!string.IsNullOrEmpty(Qunt))
//            {
//                int ac1 = int.Parse(Qunt);
//                ItemObj.Qunt = ac1;
//            }
//            if (!string.IsNullOrEmpty(Item_Grp_ID))
//            {
//                int Item_Grp_IDs = int.Parse(Item_Grp_ID);
//                ItemObj.Item_Grp_ID = Item_Grp_IDs;
//            }
//            if (!string.IsNullOrEmpty(Product_ID))
//            {
//                int Product_IDs = int.Parse(Product_ID);
//                ItemObj.Product_ID = Product_IDs;
//            }
//            items.Add(ItemObj);
//            Session["items"] = items;
//            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Items/Partial/_ItemTAddPartial.cshtml", items.ToList());
//            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
//        }
//        public JsonResult groupSelected(string Item_Grp_ID)
//        {

//            try

//            {
//                int group = int.Parse(Item_Grp_ID);
//                var lms = db.Products.Where(o => o.Item_Grp_ID == group).ToList();

//                var result3 = new SelectList(lms, "Product_ID", "Name");

//                return Json(result3, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {
//                return Json("", JsonRequestBehavior.AllowGet);
//            }

//        }
//        // POST: /Items/Create
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(Item item,string OpeningBalance,string Product_ID,string Item_Grp_ID,string Comp_ID)
//        {
//          try
//            {
//                int objid = 0;
//                if (db.InItemsInvoices.Any())
//                    objid = db.InItemsInvoices.Max(i => i.ID);
//                objid += 1;
//                InItemsInvoice inInv = new InItemsInvoice();
//                inInv.EnteredDate = DateTime.Now;
//                inInv.Entered_By = CurrentUserName;
//                inInv.Approved_By = "None";
//                inInv.ID = objid;
//                inInv.lastUpdated = DateTime.Now;
//                inInv.Status = "Pending";
//                if (Session["items"] != null)
//                {
//                    List<Item> items = (List<Item>)Session["items"];
//                    int id = 0;
//                    if (db.Items.Any())
//                        id = db.Items.Max(i => i.Item_ID);
//                    var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
//                    inInv.Items = new List<Item>();
//                    foreach (var obj in items)
//                    {
//                        var Objitem= new Item();
//                        id += 1;
//                        Objitem.Item_ID = id;
//                        inInv.InSirkNo = obj.InItemsInvoice.InSirkNo;
//                        Objitem.InItemsInvId = objid;
//                        Objitem.InItemsInvoice = inInv;
//                        Objitem.Comp_ID = obj.Comp_ID;
//                        Objitem.Unit_ID = obj.Unit_ID;
//                        Objitem.Item_Grp_ID = obj.Item_Grp_ID;
//                        Objitem.Product_ID = obj.Product_ID;
//                        Objitem.Qunt = obj.Qunt;
//                        Objitem.AvailableQ = obj.Qunt;
//                        Objitem.SoldQ = 0;
//                        Objitem.PriceForOnce = obj.PriceForOnce;
//                        Objitem.Price_dozen = obj.Price_dozen;
//                        Objitem.OpeningBalance =double.Parse( (obj.Cost*obj.Qunt).ToString());
//                        Objitem.Exp_Date = obj.Exp_Date;
//                        Objitem.Cost = obj.Cost;
//                        Objitem.Status = "Pending";
//                        Objitem.LastUpdate = DateTime.Now;
//                        Objitem.Entered_By = CurrentUserName;
//                        Branch branch = db.Branches.Where(i => i.Branch_ID == user.Branch_ID).SingleOrDefault();
//                        Objitem.Country_ID = branch.Country_ID;
//                       if(user.WareHouse_ID!=null && user.WareHouse_ID>0)
//                        Objitem.Warehouse_ID =int.Parse( user.WareHouse_ID.ToString());
//                        inInv.Items.Add(Objitem);
//                       // db.Items.Add(Objitem);
//                    }
//                    db.InItemsInvoices.Add(inInv);
//                    db.SaveChanges();

//                    Session["items"] = null;

//                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
//                    return RedirectToAction("Index");
//                }
//                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
//                ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", item.City_ID);
//                ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", item.Comp_ID);
//                ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", item.Country_ID);
//                ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name", item.Item_Grp_ID);
//                ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", item.Unit_ID);
//                ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", item.Warehouse_ID);
//                ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name");
//                return View();
//            }
//            catch (DbEntityValidationException ex)
//            {
//                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
//                ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", item.City_ID);
//                ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", item.Comp_ID);
//                ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", item.Country_ID);
//                ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name", item.Item_Grp_ID);
//                ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", item.Unit_ID);
//                ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", item.Warehouse_ID);
//                ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name");
//                return View(item);
//            }


//            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", item.City_ID);
//            ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", item.Comp_ID);
//            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", item.Country_ID);
//            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name", item.Item_Grp_ID);
//            ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", item.Unit_ID);
//            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", item.Warehouse_ID);
//            ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name");

//            return View(item);
//        }

//        // GET: /Items/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Item item = db.Items.Find(id);
//            if (item == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", item.City_ID);
//            ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", item.Comp_ID);
//            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", item.Country_ID);
//            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name", item.Item_Grp_ID);
//            ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", item.Unit_ID);
//            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", item.Warehouse_ID);
//            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", item.Status);

//            ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name",item.Product_ID);

//            return View(item);
//        }

//        // POST: /Items/Edit/5
//        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(Item item)
//        {
//            if (ModelState.IsValid)
//            {
//                Item tem = db.Items.Where(i => i.Item_ID == item.Item_ID).SingleOrDefault();
//                tem.Item_Grp_ID = item.Item_Grp_ID;
//                tem.Product_ID = item.Product_ID;
//                tem.Qunt = item.Qunt;
//                tem.OpeningBalance = item.OpeningBalance;
//                tem.PriceForOnce = item.PriceForOnce;
//                tem.Cost = item.Cost;
//                tem.LastUpdate = DateTime.Now;
//                tem.Entered_By = CurrentUserName;
//                db.Entry(tem).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", item.City_ID);
//            ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", item.Comp_ID);
//            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", item.Country_ID);
//            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name", item.Item_Grp_ID);
//            ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", item.Unit_ID);
//            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", item.Warehouse_ID);
//            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", item.Status);
//            ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name", item.Product_ID);

//            return View(item);
//        }

//        // GET: /Items/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Item item = db.Items.Find(id);
//            if (item == null)
//            {
//                return HttpNotFound();
//            }
//            return View(item);
//        }

//        // POST: /Items/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            Item item = db.Items.Find(id);
//            item.LastUpdate = DateTime.Now;
//            item.Entered_By = CurrentUserName;
//            item.Status = "Deleted";
//            db.Entry(item).State = EntityState.Modified;
//            db.SaveChanges();

//            CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
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

    public class ItemsController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        private NaalaxAuthEntities2 dbuser = new NaalaxAuthEntities2();

        // GET: /Items/
        public ActionResult Index()
        {
            var ware = Convert.ToInt32(Session["WareHouse_ID"]);
            var items = db.Items.Where(i => i.Status != "Deleted");
            return View(items.Where(i => i.Warehouse_ID == ware).ToList());
        }
        public ActionResult IndexApprove()
        {
            var wareh = Convert.ToInt32(Session["WareHouse_ID"]);
            var itee = db.Items.Where(o => o.Warehouse_ID == wareh).ToList();

            var invoices = db.InItemsInvoices.Where(i => i.Status == "Pending" &&itee.Count>0).ToList();
            if(invoices.Count>0|| invoices != null)
            {
                return View(invoices.ToList());

            }
            return View("Index");

           
        }

        // GET: /Items/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: /Items/Create
        public ActionResult Create()
        {
            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name");
            ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name");
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name");
            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name");
            ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name");
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
            ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name");
            Session["items"] = null;
            return View();
        }
        public ActionResult Approve(int? id)
        {
            Session["items"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InItemsInvoice invoice = db.InItemsInvoices.Where(i => i.ID == id).SingleOrDefault();
            if (invoice == null)
            {
                return HttpNotFound();
            }
            Session["list"] = invoice.Items.ToList();
            var items = new List<Item>();
            if (Session["list"] != null)
                items = (List<Item>)Session["list"];

            return View(invoice);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(List<Item> invoicelist)
        {
            try
            {
                if (Session["list"] != null)
                {
                    List<Item> list = (List<Item>)Session["list"];
                    Item item = list[0];
                    InItemsInvoice inv = db.InItemsInvoices.Where(i => i.ID == item.InItemsInvId).SingleOrDefault();
                    Session["InItemsInvId"] = item.InItemsInvId;
                    inv.Status = "Active";
                    inv.lastUpdated = DateTime.Now;
                    inv.Approved_By = CurrentUserName;
                    foreach (var obj in inv.Items.ToList())
                    {
                        obj.Status = "Active";
                        obj.LastUpdate = DateTime.Now;
                        // db.Entry(obj).State = EntityState.Modified;

                    }
                    db.Entry(inv).State = EntityState.Modified;

                }
                db.SaveChanges();
                Session["list"] = null;
                CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);

                return RedirectToAction("IndexApprove");
            }
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(invoicelist);
            }



        }


        public JsonResult Add(string InSirkNo, string Cost, string PriceForOnce, string Qunt, string Item_Grp_ID, string Product_ID, string Warehouse_ID)
        {


            var items = new List<Item>();
            if (Session["items"] != null)
                items = (List<Item>)Session["items"];
            var ItemObj = new Item();
            if (!string.IsNullOrEmpty(Cost))
            {
                int amy = int.Parse(Cost);
                ItemObj.Cost = amy;
            }
            ItemObj.InItemsInvoice = new InItemsInvoice();
            if (!string.IsNullOrEmpty(InSirkNo))
                ItemObj.InItemsInvoice.InSirkNo = int.Parse(InSirkNo);
            if (!string.IsNullOrEmpty(PriceForOnce))
            {
                int dozen = int.Parse(PriceForOnce);
                ItemObj.PriceForOnce = dozen;
            }
            if (!string.IsNullOrEmpty(Qunt))
            {
                int ac1 = int.Parse(Qunt);
                ItemObj.Qunt = ac1;
            }
            if (!string.IsNullOrEmpty(Item_Grp_ID))
            {
                int Item_Grp_IDs = int.Parse(Item_Grp_ID);
                ItemObj.Item_Grp_ID = Item_Grp_IDs;
            }
            if (!string.IsNullOrEmpty(Product_ID))
            {
                int Product_IDs = int.Parse(Product_ID);
                ItemObj.Product_ID = Product_IDs;
            }
            if (!string.IsNullOrEmpty(Warehouse_ID))
            {
                int Warehouse_IDs = int.Parse(Warehouse_ID);
                ItemObj.Warehouse_ID = Warehouse_IDs;
            }
            items.Add(ItemObj);
            Session["items"] = items;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Items/Partial/_ItemTAddPartial.cshtml", items.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        public JsonResult groupSelected(string Item_Grp_ID)
        {

            try

            {
                int group = int.Parse(Item_Grp_ID);
                var lms = db.Products.Where(o => o.Item_Grp_ID == group).ToList();

                var result3 = new SelectList(lms, "Product_ID", "Name");

                return Json(result3, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        // POST: /Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Item item, string OpeningBalance, string Product_ID, string Item_Grp_ID, string Comp_ID)
        {
            try
            {
                int objid = 0;
                if (db.InItemsInvoices.Any())
                    objid = db.InItemsInvoices.Max(i => i.ID);
                objid += 1;
                InItemsInvoice inInv = new InItemsInvoice();
                inInv.EnteredDate = DateTime.Now;
                inInv.Entered_By = CurrentUserName;
                inInv.Approved_By = "None";
                inInv.ID = objid;
                inInv.lastUpdated = DateTime.Now;
                inInv.Status = "Pending";
                if (Session["items"] != null)
                {
                    List<Item> items = (List<Item>)Session["items"];
                    int id = 0;
                    if (db.Items.Any())
                        id = db.Items.Max(i => i.Item_ID);
                    var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
                    inInv.Items = new List<Item>();
                    foreach (var obj in items)
                    {
                        var Objitem = new Item();
                        id += 1;
                        Objitem.Item_ID = id;
                        inInv.InSirkNo = obj.InItemsInvoice.InSirkNo;
                        Objitem.InItemsInvId = objid;
                        Objitem.InItemsInvoice = inInv;
                        Objitem.Comp_ID = obj.Comp_ID;
                        Objitem.Unit_ID = obj.Unit_ID;
                        Objitem.Item_Grp_ID = obj.Item_Grp_ID;
                        Objitem.Product_ID = obj.Product_ID;
                        Objitem.Qunt = obj.Qunt;
                        Objitem.AvailableQ = obj.Qunt;
                        Objitem.SoldQ = 0;
                        Objitem.PriceForOnce = obj.PriceForOnce;
                        Objitem.Price_dozen = obj.Price_dozen;
                        Objitem.OpeningBalance = double.Parse((obj.Cost * obj.Qunt).ToString());
                        Objitem.Exp_Date = obj.Exp_Date;
                        Objitem.Cost = obj.Cost;
                        Objitem.Status = "Pending";
                        Objitem.LastUpdate = DateTime.Now;
                        Objitem.Entered_By = CurrentUserName;
                        Branch branch = db.Branches.Where(i => i.Branch_ID == user.Branch_ID).SingleOrDefault();
                        Objitem.Country_ID = branch.Country_ID;
                        // if (user.WareHouse_ID != null && user.WareHouse_ID > 0)
                        Objitem.Warehouse_ID = obj.Warehouse_ID; ;//int.Parse(user.WareHouse_ID.ToString());
                        inInv.Items.Add(Objitem);
                        // db.Items.Add(Objitem);
                    }
                    db.InItemsInvoices.Add(inInv);
                    db.SaveChanges();

                    Session["items"] = null;

                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", item.City_ID);
                ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", item.Comp_ID);
                ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", item.Country_ID);
                ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name", item.Item_Grp_ID);
                ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", item.Unit_ID);
                ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", item.Warehouse_ID);
                ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name");
                return View();
            }
            catch (DbEntityValidationException ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", item.City_ID);
                ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", item.Comp_ID);
                ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", item.Country_ID);
                ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name", item.Item_Grp_ID);
                ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", item.Unit_ID);
                ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", item.Warehouse_ID);
                ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name");
                return View(item);
            }


            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", item.City_ID);
            ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", item.Comp_ID);
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", item.Country_ID);
            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name", item.Item_Grp_ID);
            ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", item.Unit_ID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", item.Warehouse_ID);
            ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name");

            return View(item);
        }

        // GET: /Items/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", item.City_ID);
            ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", item.Comp_ID);
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", item.Country_ID);
            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name", item.Item_Grp_ID);
            ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", item.Unit_ID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", item.Warehouse_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", item.Status);

            ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name", item.Product_ID);

            return View(item);
        }

        // POST: /Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                Item tem = db.Items.Where(i => i.Item_ID == item.Item_ID).SingleOrDefault();
                tem.Item_Grp_ID = item.Item_Grp_ID;
                tem.Product_ID = item.Product_ID;
                tem.Qunt = item.Qunt;
                tem.OpeningBalance = item.OpeningBalance;
                tem.PriceForOnce = item.PriceForOnce;
                tem.Cost = item.Cost;
                tem.LastUpdate = DateTime.Now;
                tem.Entered_By = CurrentUserName;
                db.Entry(tem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.City_ID = new SelectList(db.Cities, "City_ID", "Name", item.City_ID);
            ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", item.Comp_ID);
            ViewBag.Country_ID = new SelectList(db.Countries, "Country_ID", "Name", item.Country_ID);
            ViewBag.Item_Grp_ID = new SelectList(db.Items_Group, "Item_Grp_ID", "Item_Grp_Name", item.Item_Grp_ID);
            ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", item.Unit_ID);
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", item.Warehouse_ID);
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text", item.Status);
            ViewBag.Product_ID = new SelectList(db.Products, "Product_ID", "Name", item.Product_ID);

            return View(item);
        }

        // GET: /Items/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = db.Items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: /Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            item.LastUpdate = DateTime.Now;
            item.Entered_By = CurrentUserName;
            item.Status = "Deleted";
            db.Entry(item).State = EntityState.Modified;
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