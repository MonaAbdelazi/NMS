using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using NMS.Tools;
using NMS.Data;
using NMS.Utility;
using NMS.Core.Resources;

namespace NMS.Controllers
{
    public class InvoicesController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        private NaalaxAuthEntities2 dbuser = new NaalaxAuthEntities2();



        // GET: /Invoices/
        [Authorize(Roles = "Admins,Casher")]
        public ActionResult Index()
        {
            var ware = Convert.ToInt32(Session["WareHouse_ID"]);
            var invoices = db.Invoices.Where(i => i.Status == "Active" && i.WareHouse_ID == ware);//.Include(i => i.Branch).Include(i => i.Company).Include(i => i.Customer).Include(i => i.Supplier).Include(i => i.Unit).Include(i => i.WareHouse);
            return View(invoices.ToList());
        }
        public ActionResult IndexApprove()
        {
            var ware = Convert.ToInt32(Session["WareHouse_ID"]);
            var invoices = db.Invoices.Where(i => i.Status == "Active" && i.WareHouse_ID == ware);//.Include(i => i.Branch).Include(i => i.Company).Include(i => i.Customer).Include(i => i.Supplier).Include(i => i.Unit).Include(i => i.WareHouse);
            return View(invoices.ToList());
            //&&i.Entered_By!=CurrentUserName
        }

        // GET: /Invoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // GET: /Invoices/Create
        public ActionResult Create()
        {

            Session["list"] = null;
            Session["items"] = null;

            var user = CurrentUserID;
            AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
            //ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name");
            //ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name");
            //ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name");
            ViewBag.Cus_ID = new SelectList(db.Customers.Where(i=>i.Status=="Active"), "Cus_ID", "Cus_Name");
            var itemList = from b in db.Items
                           where b.Status == "Active" && b.Warehouse_ID==userlogged.WareHouse_ID
                                select new { desc = b.Product.Name_AR, code = b.Item_ID };
            ViewBag.Item_ID = new SelectList(itemList, "code", "desc");

           ViewBag.Payment_Method = new SelectList(CommonUtils.getPaymentM(), "Value", "Text");
            //ViewBag.Supp_ID = new SelectList(db.Suppliers, "Supp_ID", "Supp_Name");
            //ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name");
            //ViewBag.WareHouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");

            return View();
        }
       
        public ActionResult Approve(int? id)
        {
            Session["items"] = null;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Where(i => i.Invoice_ID == id).SingleOrDefault();
            if (invoice == null)
            {
                return HttpNotFound();
            }
            Session["list"] = invoice.Invoice_Items.ToList();
            var items = new List<Invoice_Items>();
            if (Session["list"] != null)
                items = (List<Invoice_Items>)Session["list"];
            int Qun = 0;
            var obj = new Item();
            foreach (var listobj in items)
            {
              List<Item>  listobjdb = db.Items.Where(i => i.Item_ID == listobj.Item_ID).ToList();
                var sumdbQ = listobjdb.Sum(i => i.AvailableQ);
                List<Invoice_Items> listItem = items.Where(i => i.Item_ID ==listobj.Item_ID).ToList();
                var sumInv = listItem.Sum(i => i.Item.AvailableQ);
                if (sumInv > sumdbQ)
                {
                    //TempData["AlertMessage"] = Feedback.SavedSuccessfully + "Print ";
                    CommonUtils.SetFeedback(Feedback.QuantityNotAvalable, Feedback.Feedback_Error);
                    return RedirectToAction("IndexApprove");
                }
                listobj.Approved_By = CurrentUserName;
            }
            
            return View(invoice);
        }

        public JsonResult Add(string Item_ID, string Qunt, string Price_Unit, string totalAmount,string Discount)
        {

            var items = new List<Invoice_Items>();
            if (Session["items"] != null)
                items = (List<Invoice_Items>)Session["items"];
            var currObj = new Invoice_Items();

            if (!string.IsNullOrEmpty(Item_ID))
                currObj.Item_ID = int.Parse(Item_ID);
            if (!string.IsNullOrEmpty(Qunt))
                currObj.Quantity = int.Parse(Qunt);
            if (!string.IsNullOrEmpty(Price_Unit))
                currObj.unit_Price = int.Parse(Price_Unit);
            currObj.total_Price = currObj.Quantity * currObj.unit_Price;
            currObj.Item_NameAr = items.Where(i => i.Item_ID == currObj.Item_ID).Select(i => i.Item_NameAr).SingleOrDefault();
            if (!string.IsNullOrEmpty(Discount))
                currObj.Discount =Convert.ToDecimal( Discount);
            items.Add(currObj);
            Session["items"] = items;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Invoices/Partial/_ItemTAddPartial.cshtml", items.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult checkQuantity(int? Item_ID,int? Qunt)
        {
            var items = new List<Invoice_Items>();
            if (Session["items"] != null)
                items = (List<Invoice_Items>)Session["items"];
            int Qun = 0;
            var obj = new Item();
            var ware = Convert.ToInt16( Session["WareHouse_ID"]);
            obj = db.Items.Where(i => i.Item_ID == Item_ID&&i.Warehouse_ID==ware).SingleOrDefault();
            var listItem = items.Where(i => i.Item_ID == Item_ID).ToList();
            foreach (var listobj in listItem)
            {
                Qun +=int.Parse(listobj.Quantity.ToString());
            }
            if (Qun>0)
            {
                if (obj.AvailableQ > (Qunt+Qun))
                    return Json("OK", JsonRequestBehavior.AllowGet);
                else
                {
                    return Json("ERR", JsonRequestBehavior.AllowGet);

                }
            }

                if (obj!=null&& obj.AvailableQ>Qunt)
            return Json("OK", JsonRequestBehavior.AllowGet);
            else
            {
                return Json("ERR", JsonRequestBehavior.AllowGet);

            }
        }
        public JsonResult getPrice(int? Item_ID)
        {
            var obj = new Item();
            obj = db.Items.Where(i => i.Item_ID == Item_ID).SingleOrDefault();


            return Json(obj.PriceForOnce.ToString(), JsonRequestBehavior.AllowGet);
        }
        // POST: /Invoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admins,Casher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Invoice invoice,string Total_Price,string comment)
        {
            AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();

            try
            {
                int warehouse = Convert.ToInt32(Session["WareHouse_ID"]);
                int branch = Convert.ToInt32(Session["Branch_ID"]);
                var time = DateTime.Now.Date;
                var dailyActivity = db.DailyActivities.Where(i => i.Warehouse_ID == warehouse && i.OpenStatus == "Openned" && i.Branch_ID == branch && i.DayDate == time).ToList();

                if (Session["items"] != null&& dailyActivity.Count>0)
                {
                    double amt = 0;
                    List<Invoice_Items> invoicelist = (List<Invoice_Items>)Session["items"];
                   List<Invoice_Items> ilist = new List<Invoice_Items>();

                    int id = 0;
                    int idItem = 0;
                    int? invNo = 0;
                    if (db.Invoices.Any())
                    {
                        id = db.Invoices.Max(i => i.Invoice_ID);
                        invNo = db.Invoices.Max(i => i.InvoiceNo);
                    }
                    if (db.Invoice_Items.Any())
                    {
                        idItem = db.Invoice_Items.Max(i => i.In_Item_ID);
                    }
                    invNo += 1;
                    id += 1;
                    Invoice inv = new Invoice();
                    Session["Invoice_ID"] = id;

                    inv.Invoice_ID = id;

                    inv.InvoiceNo = invNo;
                    inv.Isuue_Date = DateTime.Now;
                    inv.Status = "Active";
                    inv.LastUpdate = DateTime.Now;
                    inv.Entered_By = CurrentUserName;
                    inv.WareHouse_ID = userlogged.WareHouse_ID;
                   inv.Payment_Method = invoice.Payment_Method;//"Cash";
                    var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
                    inv.Branch_ID= int.Parse(user.Branch_ID.ToString());
                    inv.Cus_ID = invoice.Cus_ID;
                   
                    foreach (var obj in invoicelist)
                    {
                        // var invoiceObj = new Invoice();
                        idItem += 1;
                        obj.In_Item_ID = idItem;
                        obj.Invoice_ID = id;
                        obj.Issue_Date = DateTime.Now;
                        obj.Item = db.Items.Where(i => i.Item_ID == obj.Item_ID).SingleOrDefault();
                        inv.Company = obj.Item.Company;
                        obj.Status = "Active";
                        obj.last_Updated = DateTime.Now;
                        obj.Entered_By = CurrentUserName;
                        obj.Item_NameAr = obj.Item.Product.Name_AR;
                        obj.comment = comment;//obj.Item.Product.Comment;
                        obj.WareHouse_ID = warehouse;
                     //   amt +=int.Parse(obj.total_Price.ToString());
                        if (obj.Discount > 0)
                        {
                            amt = Convert.ToDouble(obj.total_Price) -Convert.ToDouble( obj.Discount);
                            obj.total_Price =Convert.ToDecimal( amt);

                        }
                        else
                        {
                            obj.total_Price = obj.total_Price;// Convert.ToDecimal(amt);

                        }

                        //inv.Total_Price =   Convert.ToDecimal(Total_Price); 
                       db.Invoice_Items.Add(obj);
                        ilist.Add(obj);
                        invoice.DisCount =Convert.ToDouble( obj.Discount);
                        //Item item = obj.Item;
                        //item.AvailableQ = item.AvailableQ - obj.Qunt;
                        //item.SoldQ += obj.Qunt;
                        //db.Entry(item).State = EntityState.Modified;
                    }

                    inv.Total_Price = decimal.Parse(Total_Price.ToString());
                    
                    if (invoice.DisCount > 0)
                        inv.Total_Price = Decimal.Subtract(decimal.Parse(inv.Total_Price.ToString()), decimal.Parse(invoice.DisCount.ToString()));
                    // inv.Tax_NoFor_Comp =int.Parse(( 0.17 * amt).ToString());
                    db.Invoices.Add(inv);

                    db.SaveChanges();
                    // print(ilist);
                    TempData["AlertMessage"] = Feedback.SavedSuccessfully + "Print " ;
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    Session["items"] = null;
                    Session["list"] = null;
                    Session["items"] = null;


                    var itemList = from b in db.Items
                                   where b.Status == "Active" && b.Warehouse_ID == userlogged.WareHouse_ID
                                   select new { desc = b.Product.Name_AR, code = b.Item_ID };
                  ViewBag.Item_ID = new SelectList(itemList, "code", "desc");
                    //ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name");
                    //ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name");
                    //ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name");
                    ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name");
                    //ViewBag.Supp_ID = new SelectList(db.Suppliers, "Supp_ID", "Supp_Name");
                    //ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name");
                    //ViewBag.WareHouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
                    ViewBag.Payment_Method = new SelectList(CommonUtils.getPaymentM(), "Value", "Text", invoice.Payment_Method);

                    return View(inv);
                }
            }
            catch (Exception ex)
            {
                Session["items"] = null;
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
            //    AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();

                var itemList = from b in db.Items
                               where b.Status == "Active" && b.Warehouse_ID == userlogged.WareHouse_ID
                               select new { desc = b.Product.Name_AR, code = b.Item_ID };
                //ViewBag.Item_ID = new SelectList(itemList, "code", "desc");
                //ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name");
                //ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name");
                //ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name");
                //ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name");
                //ViewBag.Supp_ID = new SelectList(db.Suppliers, "Supp_ID", "Supp_Name");
                //ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name");
                //ViewBag.WareHouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
                ViewBag.Payment_Method = new SelectList(CommonUtils.getPaymentM(), "Value", "Text", invoice.Payment_Method);

                return View(new Invoice());
            }


           
             ViewBag.Payment_Method = new SelectList(CommonUtils.getPaymentM(), "Value", "Text",invoice.Payment_Method);
            return View("Oops");
        }
        // GET: /Branches/
        public JsonResult GetReport(string Status)
        {
            if (Session["Invoice_ID"] != null)
            {
                int Invoice_ID = int.Parse(Session["Invoice_ID"].ToString());
                Invoice inv = db.Invoices.Where(i => i.Invoice_ID == Invoice_ID).SingleOrDefault();

                List<Invoice_Items> listitems = db.Invoice_Items.Where(i => i.Invoice_ID == Invoice_ID).ToList();
                var dt = new List<VMInvoiceItems>();
                foreach (var item in listitems)
                {
                    VMInvoiceItems vm = new VMInvoiceItems();
                    vm.Item_ID = item.Item_ID;
                    vm.Item_NameAr = item.Item_NameAr;
                    vm.last_Updated =DateTime.Parse(item.last_Updated.ToString());
                    vm.Quantity = item.Quantity;
                    vm.Status = item.Status;
                    vm.total_Price = item.total_Price;
                    vm.unit_Price = item.unit_Price;
                    vm.WareHouse_ID =int.Parse(item.WareHouse_ID.ToString());
                    vm.Discount =Convert.ToInt16( item.Discount);
                    vm.Invoice_ID = item.Invoice_ID;
                    vm.Entered_By = item.Entered_By;
                    vm.comment = item.comment;
                    vm.Approved_By = "";// Convert.ToString( item.Approved_By);
                    vm.Issue_Date =DateTime.Parse(item.Issue_Date.ToString());



                    dt.Add(vm);
                }
                if (dt != null && dt.Count > 0)
                { 

                    var paremeters = new List<KeyValuePair<string, string>>();

                   // paremeters.Add(new KeyValuePair<string, string>("Name", "Active"));
                    paremeters.Add(new KeyValuePair<string, string>("IssueDate", inv.Isuue_Date.ToString()));
                    paremeters.Add(new KeyValuePair<string, string>("TotalAmount", inv.Total_Price.ToString()));
                    if (inv.Cus_ID != null && inv.Tax_NoFor_Comp > 0)
                    {
                        paremeters.Add(new KeyValuePair<string, string>("Tax_NoFor_Comp", inv.Tax_NoFor_Comp.ToString()));
                    }
                    else
                    {
                        paremeters.Add(new KeyValuePair<string, string>("Tax_NoFor_Comp", "0"));

                    }
                    if (inv.Cus_ID != null && inv.Cus_ID > 0)
                    {
                        var Customer = db.Customers.Where(i => i.Cus_ID == inv.Cus_ID).SingleOrDefault();

                        paremeters.Add(new KeyValuePair<string, string>("custName", Customer.Cus_Name_AR.ToString()));
                    }
                    else
                    {
                        paremeters.Add(new KeyValuePair<string, string>("custName", "NONE"));

                    }
                    if (inv.Status.Trim() == "Active")

                        paremeters.Add(new KeyValuePair<string, string>("Status", "قيد الدفع"));
                    if (inv.Status.Trim() == "Approved")

                        paremeters.Add(new KeyValuePair<string, string>("Status", "مدفوع"));

                    //ViewReportLocal("rptStandardReport", dt, paremeters);

                    Session["ReportParameter"] = paremeters;
                    Session["ReportData"] = dt;
                    Session["ReportName"] = "rptInvoices";
                 //   Session["ReportOption"] = "Invoiceslist";

                    return Json(true, JsonRequestBehavior.AllowGet);


                }
                else
                {
                    // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                    return Json(false, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                return Json(false, JsonRequestBehavior.AllowGet);

            }

        }
        [Authorize(Roles = "Admins,Casher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(List<Invoice_Items> invoicelist,decimal? Tax_NoFor_Comp)
        {
            try
            {
                if (Session["list"] != null)
                {
                    List<Invoice_Items> list = (List<Invoice_Items>)Session["list"];
                    Invoice_Items item = list[0];
                    Invoice inv = db.Invoices.Where(i => i.Invoice_ID == item.Invoice_ID).SingleOrDefault();
                    Session["Invoice_ID"] = item.Invoice_ID;
                    inv.Status = "Approved";
                    inv.LastUpdate = DateTime.Now;
                    inv.Approved_By = CurrentUserName;
                    if (Tax_NoFor_Comp!=null)
                    {
                        inv.Tax_NoFor_Comp = decimal.Parse((0.17 * int.Parse(inv.Total_Price.ToString())).ToString());

                    }

                    foreach (var obj in list)
                    {
                        List<Invoice_Items> newinvitem = new List <Invoice_Items>();
                        obj.Status = "Approved";
                        obj.last_Updated = DateTime.Now;
                        obj.Approved_By = CurrentUserName;
                        obj.total_Price =Convert.ToDecimal( inv.Total_Price);
                        obj.Discount = Convert.ToDecimal(obj.Discount);
                       // obj.WareHouse = db.WareHouses.Where(i=>i.Warehouse_ID==obj.WareHouse_ID).SingleOrDefault();
                        newinvitem.Add(obj);
                      //  db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();
                        Item invocrItem = db.Items.Where(i=>i.Item_ID==obj.Item_ID).SingleOrDefault();
                        invocrItem.AvailableQ =int.Parse( (invocrItem.AvailableQ - obj.Quantity).ToString());
                        invocrItem.SoldQ += int.Parse(obj.Quantity.ToString());

                      
                        db.Entry(invocrItem).State = EntityState.Modified;
                        db.SaveChanges();
                        db.Entry(newinvitem).State = EntityState.Modified;
                        var oldEntity = db.Invoice_Items.Where(i => i.In_Item_ID == obj.In_Item_ID).SingleOrDefault(); // You can use find instead of FGetById
                        db.Entry(oldEntity).CurrentValues.SetValues(newinvitem);

                        UpdateModel(newinvitem);

                    }

                    db.Entry(inv).State = EntityState.Modified;
                    var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
                    var time = DateTime.Now.Date;
                    DailyActivity d = db.DailyActivities.Where(i => i.DayDate == time  && i.OpenStatus == "Openned" && i.Warehouse_ID ==user.WareHouse_ID).SingleOrDefault();
                    if (d.SoldAmount == null)
                    {
                        d.SoldAmount = 0;
                        d.SoldAmount = d.SoldAmount + inv.Total_Price;

                    }
                    else
                    {
                        d.SoldAmount = d.SoldAmount + inv.Total_Price;

                    }
                    db.Entry(d).State = EntityState.Modified;


                }
                db.SaveChanges();
                Session["list"] = null;
                TempData["AlertMessage"] = Feedback.SavedSuccessfully + "Print ";
                CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);

                //var user = CurrentUserID;
                 AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
                //ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name");
                //ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name");
                //ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name");
                //ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name");
                 ViewBag.Item_ID = new SelectList(db.Items.Where(i => i.Warehouse_ID == userlogged.WareHouse_ID).ToList(), "Item_ID", "Item_Name");
                //ViewBag.Supp_ID = new SelectList(db.Suppliers, "Supp_ID", "Supp_Name");
                //ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name");
                //ViewBag.WareHouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
                return View(new Invoice());
            }
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                var user = CurrentUserID;
                AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
                ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name");
                ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name");
                ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name");
                ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name");
                ViewBag.Item_ID = new SelectList(db.Items.Where(i => i.Warehouse_ID == userlogged.WareHouse_ID).ToList(), "Item_ID", "Item_Name");
                ViewBag.Supp_ID = new SelectList(db.Suppliers, "Supp_ID", "Supp_Name");
                ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name");
                ViewBag.WareHouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
                return View(new Invoice());
            }



        }

        // GET: /Invoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            Session["items"] = invoice.Invoice_Items.ToList();
            ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name", invoice.Branch_ID);
            //ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", invoice.Comp_ID);
            //ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name", invoice.Curr_ID);
            //ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name", invoice.Cus_ID);
            //var itemList = from b in db.Items
            //               where b.Status == "Active" && b.Warehouse_ID == userlogged.WareHouse_ID
            //               select new { desc = b.Product.Name_AR, code = b.Item_ID };
            //ViewBag.Item_ID = new SelectList(db.Items, "Item_ID", "Item_Name");
            //ViewBag.Supp_ID = new SelectList(db.Suppliers, "Supp_ID", "Supp_Name", invoice.Supp_ID);
            //ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", invoice.Unit_ID);
            //ViewBag.WareHouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", invoice.WareHouse_ID);
            return View(invoice);
        }

        // POST: /Invoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admins,Casher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Invoice invoice,List<Invoice_Items> items)
        {
            try
            {
                if (Session["items"] != null)
                {
                    double amt = 0;
                    List<Invoice_Items> invoicelist = (List<Invoice_Items>)Session["items"];
                    List<Invoice_Items> ilist = new List<Invoice_Items>();

                    int id = 0;
                    int idItem = 0;
                    int? invNo = 0;
                    if (db.Invoices.Any())
                    {
                        id = db.Invoices.Max(i => i.Invoice_ID);
                        invNo = db.Invoices.Max(i => i.InvoiceNo);
                    }
                    if (db.Invoice_Items.Any())
                    {
                        idItem = db.Invoice_Items.Max(i => i.In_Item_ID);
                    }
                    invNo += 1;
                    id += 1;
                    Invoice inv = new Invoice();
                    inv.Invoice_ID = id;
                    Session["Invoice_ID"] = id;

                    inv.InvoiceNo = invNo;
                    inv.Isuue_Date = DateTime.Now;
                    inv.Status = "Active";
                    inv.LastUpdate = DateTime.Now;
                    inv.Entered_By = CurrentUserName;
                    inv.Payment_Method = "Cash";
                    var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
                    inv.Branch_ID = int.Parse(user.Branch_ID.ToString());
                    inv.Curr_ID = invoice.Curr_ID;
                    inv.Cus_ID = invoice.Cus_ID;
                    foreach (var obj in invoicelist)
                    {
                        // var invoiceObj = new Invoice();
                        idItem += 1;
                        obj.In_Item_ID = idItem;
                        obj.Invoice_ID = id;
                        obj.Issue_Date = DateTime.Now;
                        obj.Item = db.Items.Where(i => i.Item_ID == obj.Item_ID).SingleOrDefault();
                        inv.Company = obj.Item.Company;
                        obj.Status = "Active";
                        obj.last_Updated = DateTime.Now;
                        obj.Entered_By = CurrentUserName;
                        obj.Item_NameAr = obj.Item.Product.Name_AR;
                        inv.Unit_ID =int.Parse(obj.Item.Unit_ID.ToString());
                        amt += int.Parse(obj.total_Price.ToString());
                        db.Invoice_Items.Add(obj);
                        ilist.Add(obj);
                        //Item item = obj.Item;
                        //item.AvailableQ = item.AvailableQ - obj.Qunt;
                        //item.SoldQ += obj.Qunt;
                        //db.Entry(item).State = EntityState.Modified;
                    }

                    inv.Total_Price = decimal.Parse(amt.ToString());
                    // inv.Tax_NoFor_Comp =int.Parse(( 0.17 * amt).ToString());
                    db.Invoices.Add(inv);

                    db.SaveChanges();
                    // print(ilist);
                    TempData["AlertMessage"] = Feedback.SavedSuccessfully + "Print ";
                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    Session["items"] = null;
                    Session["list"] = null;
                    Session["items"] = null;

                    AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
                    //ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name");
                    //ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name");
                    //ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name");
                    //ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name");
                    //ViewBag.Item_ID = new SelectList(db.Items.Where(i => i.Warehouse_ID == userlogged.WareHouse_ID).ToList(), "Item_ID", "Item_Name");
                    //ViewBag.Supp_ID = new SelectList(db.Suppliers, "Supp_ID", "Supp_Name");
                    //ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name");
                    //ViewBag.WareHouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
                    return View(inv);
                }
            }
            catch (Exception ex)
            {
                Session["items"] = null;
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(invoice);
            }


            //ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name", invoice.Branch_ID);
            //ViewBag.Comp_ID = new SelectList(db.Companies, "Comp_ID", "Comp_Name", invoice.Comp_ID);
            //ViewBag.Curr_ID = new SelectList(db.Currencies, "Curr_ID", "Curr_Name", invoice.Curr_ID);
            //ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name", invoice.Cus_ID);
            ViewBag.Item_ID = new SelectList(db.Items, "Item_ID", "Item_Name");
            //ViewBag.Supp_ID = new SelectList(db.Suppliers, "Supp_ID", "Supp_Name", invoice.Supp_ID);
            //ViewBag.Unit_ID = new SelectList(db.Units, "Unit_ID", "Unit_Name", invoice.Unit_ID);
            //ViewBag.WareHouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name", invoice.WareHouse_ID);
            return View(invoice);
        }

        // GET: /Invoices/Delete/5
        [Authorize(Roles = "Admins,Casher")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: /Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice invoice = db.Invoices.Find(id);
            var list = invoice.Invoice_Items.ToList();
            foreach (var item in list)
            {
                item.Entered_By = CurrentUserName;
                item.Status = "Deleted";
                db.Entry(item).State = EntityState.Modified;
                
            }
            invoice.LastUpdate = DateTime.Now;
            invoice.Entered_By = CurrentUserName;
            invoice.Status = "Deleted";
            db.Entry(invoice).State = EntityState.Modified;


            db.SaveChanges();
            CommonUtils.SetFeedback(Feedback.Feedback_Success, Feedback.Feedback_Success);
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

    public partial class VMInvoiceItems
    {
        public int In_Item_ID { get; set; }
        public int Invoice_ID { get; set; }
        public int Item_ID { get; set; }
        public decimal Quantity { get; set; }
        public decimal unit_Price { get; set; }
        public decimal total_Price { get; set; }
        public System.DateTime last_Updated { get; set; }
        public string Status { get; set; }
        public string Entered_By { get; set; }
        public string Approved_By { get; set; }
        public System.DateTime Issue_Date { get; set; }
        public string Item_NameAr { get; set; }
        public string comment { get; set; }
        public int WareHouse_ID { get; set; }
        public decimal Discount { get; set; }
    }
    }
