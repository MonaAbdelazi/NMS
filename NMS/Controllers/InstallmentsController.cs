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
using System.Data.Entity.Core.Objects;


namespace NMS.Controllers
{


    public class InstallmentsController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        private NaalaxAuthEntities2 dbuser = new NaalaxAuthEntities2();
        //
        //public class VMinstallment
        //{
        //    public decimal Paid { get; set; }
        //    public int Inst_ID { get; set; }
        //    public string CusName { get; set; }
        //    public System.DateTime StartDate { get; set; }
        //    public System.DateTime EndDate { get; set; }
        //    public int No_Of_Inst { get; set; }
        //    public double Amount { get; set; }
        //    public string Comment { get; set; }
        //    public string Status { get; set; }

        //    public int Invoice_ID { get; set; }
        //    public decimal ResidualAmt { get; set; }
        //    public int ResidualIns { get; set; }
        //    public int Warehouse_ID { get; set; }

        //    public int numPaidinst { get; set; }

        //    public DateTime paiddate { get; set; }



        //}
        // GET: /Installments/
        public ActionResult Index()
        {
            var installments = db.Installments.Include(i => i.Customer);
            return View(installments.ToList());
        }

        // GET: /Installments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Installment installment = db.Installments.Find(id);
            if (installment == null)
            {
                return HttpNotFound();
            }
            return View(installment);
        }
        //
        public JsonResult GetInstReport(int? CusName, int? Inst_ID)
        {
            if (Inst_ID != null)
            {

                Installment ins = db.Installments.Where(i => i.Inst_ID == Inst_ID).SingleOrDefault();

                List<Installment> listitems = db.Installments.Where(i => i.Inst_ID == Inst_ID).ToList();

                var dt = new VMinstallment();

                var Cust = db.Customers.Where(i => i.Cus_ID == ins.Cus_ID).SingleOrDefault();
                foreach (var item in listitems)
                {
                    dt.Inst_ID = item.Inst_ID;
                    dt.Invoice_ID = item.Invoice_ID;
                    dt.No_Of_Inst = item.No_Of_Inst;
                    dt.numPaidinst = Convert.ToInt32(item.numPaidinst);
                    dt.ResidualIns = Convert.ToInt32(item.ResidualIns);

                    dt.Paid = Convert.ToDouble(item.Paid);
                    dt.ResidualAmt = Convert.ToDecimal(item.ResidualAmt);
                    dt.Amount = Convert.ToDecimal(item.Amount);
                    dt.paiddate = Convert.ToDateTime(item.paiddate);
                    dt.StartDate = item.StartDate;
                    dt.EndDate = item.EndDate;
                    dt.CusName = Cust.Cus_Name_AR;

                    dt.Warehouse_ID = item.Warehouse_ID;
                    //  listitems.Add(item);
                }
                if (dt != null)
                {



                    Session["ReportData"] = dt;
                    Session["ReportName"] = "Inst";
                    Session["ReportOption"] = "installmentlist";

                    return Json(true, JsonRequestBehavior.AllowGet);


                }
                else
                {

                    return Json(false, JsonRequestBehavior.AllowGet);

                }
            }

            else
            {


                //Installment ins = db.Installments.Where(i => i.Cus_ID == CusName).SingleOrDefault();

                List<Installment> listitems = db.Installments.Where(i => i.Cus_ID == CusName).ToList();

                var dt = new VMinstallment();
                var ddt = new List<VMinstallment>();
                var Cust = db.Customers.Where(i => i.Cus_ID == CusName).SingleOrDefault();
                foreach (var item in listitems)
                {
                    dt.Inst_ID = item.Inst_ID;
                    dt.Invoice_ID = item.Invoice_ID;
                    dt.No_Of_Inst = item.No_Of_Inst;
                    dt.numPaidinst = Convert.ToInt32(item.numPaidinst);
                    dt.ResidualIns = Convert.ToInt32(item.ResidualIns);

                    dt.Paid = Convert.ToDouble(item.Paid);
                    dt.ResidualAmt = Convert.ToDecimal(item.ResidualAmt);
                    dt.Amount = Convert.ToDecimal(item.Amount);
                    dt.paiddate = Convert.ToDateTime(item.paiddate);
                    dt.StartDate = item.StartDate;
                    dt.EndDate = item.EndDate;
                    dt.CusName = Cust.Cus_Name_AR;

                    dt.Warehouse_ID = item.Warehouse_ID;
                    ddt.Add(dt);
                }
                if (dt != null)
                {



                    Session["ReportData"] = ddt;
                    Session["ReportName"] = "Inst";


                    return Json(true, JsonRequestBehavior.AllowGet);


                }
                
           
             
            else
            {

                    return Json(false, JsonRequestBehavior.AllowGet);

                }

            } } 
        //public ActionResult AddTestDetiles(Invoice TestDetiles)
        //{
        //    return View("AddTestDetiles", new List<Invoice>() { new Invoice() });
        //}
        public JsonResult getPrice(int? Item_ID)
        {
            var obj = new Item();
            obj = db.Items.Where(i => i.Item_ID == Item_ID).SingleOrDefault();


            return Json(obj.PriceForOnce.ToString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult checkQuantity(int? Item_ID, int? Qunt)
        {
            var items = new List<Invoice_Items>();
            if (Session["items"] != null)
                items = (List<Invoice_Items>)Session["items"];
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

        //
        public ActionResult IndexReport()
        {
            ViewBag.CusName = new SelectList(db.Customers, "Cus_ID", "Cus_Name");

            return View();
        }
        // GET: /Installments/Create
        public ActionResult Create()
        {
            var user = CurrentUserID;
            AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name");
            var itemList = from b in db.Items
                           where b.Status == "Active" && b.Warehouse_ID == userlogged.WareHouse_ID
                           select new { desc = b.Product.Name_AR, code = b.Item_ID };
            ViewBag.Item_ID = new SelectList(itemList, "code", "desc");
        //    Installment master = new Installment();
          var  master =new List<Invoice_Items> {
                new Invoice_Items()
            };
            
            return View();
           
        }

        // POST: /Installments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Inst_ID,Cus_ID,StartDate,EndDate,No_Of_Inst,Amount,Comment")] Installment installment, List<Invoice_Items> TestDetiles)
        {
            var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
            Invoice inv = new Invoice();

            if (ModelState.IsValid)
            {
                double amt = 0;
                int Invoiceid = 0;
                int idItem = 0;
                int? invNo = 0;
                var id = 0;
                if (db.Invoices.Any())
                {
                    Invoiceid = db.Invoices.Max(i => i.Invoice_ID);
                    invNo = db.Invoices.Max(i => i.InvoiceNo);
                }
                invNo += 1;
                id += 1;
                Invoice invx = new Invoice();

                inv.Invoice_ID = Invoiceid;
                inv.InvoiceNo = invNo;
                inv.Isuue_Date = DateTime.Now;
                inv.Status = "Active";
                inv.LastUpdate = DateTime.Now;
                
                inv.Entered_By = CurrentUserName;
                inv.Payment_Method = "PostPayed";


                inv.Branch_ID = int.Parse(user.Branch_ID.ToString());
                inv.Cus_ID = installment.Cus_ID;
                inv.Total_Price = Convert.ToDecimal(installment.Amount);
                inv.WareHouse_ID = Convert.ToInt32(Session["WareHouse_ID"]);
                db.Invoices.Add(inv);




                //
                var items = new List<Invoice_Items>();
             //   if (Session["items"] != null)
                 //   items = (List<Invoice_Items>)Session["items"];
                var currObj = new Invoice_Items();
                ++Invoiceid;
                if (db.Invoice_Items.Any())
                {
                    id = db.Invoice_Items.Max(i => i.In_Item_ID);
                }
                foreach (var item in TestDetiles)
                {
                  
                    currObj.Item_ID =item.Item_ID;

                    currObj.Quantity = item.Quantity;

                    currObj.unit_Price =Convert.ToDecimal(item.unit_Price);
                    currObj.total_Price = Convert.ToDecimal( installment.Amount);
                    currObj.last_Updated = DateTime.Now;
                    currObj.Status = "Active";
                    currObj.Entered_By = CurrentUserName;
                    currObj.Issue_Date = DateTime.Now;
                   currObj.Invoice_ID= Invoiceid;
                    currObj.Item = db.Items.Where(i => i.Item_ID == currObj.Item_ID).SingleOrDefault();
                    currObj.Item_NameAr = currObj.Item.Product.Name_AR;// db.Items.Where(o => o.Item_ID==item.Item_ID).Select(o=>o.Item_name).SingleOrDefault();
                    inv.Qunt = Convert.ToInt32( item.Quantity);
                    inv.Price_Unit =Convert.ToDouble( item.unit_Price);

                    currObj.In_Item_ID = ++id;


                    currObj.total_Price = currObj.Quantity * currObj.unit_Price;

                    db.Invoice_Items.Add(currObj);
                    items.Add(currObj);

                    db.SaveChanges();
                        ++id;
                    currObj = new Invoice_Items();


                }


                //
                if (db.Installments.Any())
                {
                    id = db.Installments.Max(i => i.Inst_ID);
                 

                }
                installment.Inst_ID = id;

                installment.Status="Active";
	 	      installment.Last_Update=DateTime.Now;
                installment.paiddate = DateTime.Now;
                installment.Invoice_ID = Invoiceid;
		         installment.Enterd_By = CurrentUserName;
                installment.ResidualAmt =Convert.ToDecimal( installment.Amount);
                installment.No_Of_Inst = installment.No_Of_Inst;
                installment.currntamount = 0;
                installment.Paid = 0;
                installment.currntinst = 0;
                installment.numPaidinst = 0;
                installment.Warehouse_ID =Convert.ToInt32( user.WareHouse_ID);
                
                db.Installments.Add(installment);

                db.SaveChanges();

                TempData["AlertMessage"] = Feedback.SavedSuccessfully + "Print ";
                CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                return RedirectToAction("Index");
            }

            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name", installment.Cus_ID);
            return View(installment);
        }
        public JsonResult GetReport(string Status)
        {
            if (Session["Invoice_ID"] != null)
            {
                int Invoice_ID = int.Parse(Session["Invoice_ID"].ToString());
                Invoice inv = db.Invoices.Where(i => i.Invoice_ID == Invoice_ID).SingleOrDefault();

                List<Invoice_Items> listitems = db.Invoice_Items.Where(i => i.Invoice_ID == Invoice_ID).ToList();
                var dt = new List<Invoice_Items>();
                foreach (var item in listitems)
                {
                    item.Item = db.Items.Where(i => i.Item_ID == item.Item_ID).SingleOrDefault();
                    item.Invoice = db.Invoices.Where(i => i.Invoice_ID == item.Invoice_ID).SingleOrDefault();
                    dt.Add(item);
                }
                if (dt != null && dt.Count > 0)
                {

                    //    List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>();





                    var paremeters = new List<KeyValuePair<string, string>>();

                    paremeters.Add(new KeyValuePair<string, string>("Name", "Active"));
                    paremeters.Add(new KeyValuePair<string, string>("IssueDate", inv.Isuue_Date.ToString()));
                    paremeters.Add(new KeyValuePair<string, string>("TotalAmount", inv.Total_Price.ToString()));
                    paremeters.Add(new KeyValuePair<string, string>("Tax_NoFor_Comp", inv.Tax_NoFor_Comp.ToString()));
                    if (inv.Cus_ID != null && inv.Cus_ID > 0)
                    {
                        var Customer = db.Customers.Where(i => i.Cus_ID == inv.Cus_ID).SingleOrDefault();

                        paremeters.Add(new KeyValuePair<string, string>("custName", Customer.Cus_Name_AR.ToString()));
                    }
                    else
                    {
                        paremeters.Add(new KeyValuePair<string, string>("custName", "NONE"));

                    }
                    //ViewReportLocal("rptStandardReport", dt, paremeters);

                    Session["ReportParameter"] = paremeters;
                    Session["ReportData"] = dt;
                    Session["ReportName"] = "rptInvoices";

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
        // GET: /Installments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Installment installment = db.Installments.Find(id);
            
            if (installment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ResidualAmt = Convert.ToDecimal( installment.Amount)-Convert.ToDecimal( installment.currntamount);
            ViewBag.ResidualIns = installment.No_Of_Inst-installment.currntinst;
            ViewBag.Paid = db.Installments.Where(i => i.Invoice_ID == installment.Invoice_ID).Sum(i=>i.currntamount);
            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name", installment.Cus_ID);
            return View(installment);
        }

        // POST: /Installments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Inst_ID,Cus_ID,StartDate,EndDate,No_Of_Inst,Amount,Comment")] Installment installment,decimal ResidualAmt,int ResidualIns,int Invoice_ID)
        {
            if (ModelState.IsValid)
            {
                var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
                
                installment.Invoice_ID = Invoice_ID;
                installment.Status = "Active";
                installment.Last_Update = DateTime.Now;
                installment.ResidualAmt = ResidualAmt;//Convert.ToDecimal( installment.Amount)-Convert.ToDecimal( installment.currntamount);
                installment.ResidualIns = ResidualIns;//installment.No_Of_Inst-installment.currntinst;
                installment.numPaidinst = installment.No_Of_Inst - installment.ResidualIns;
                installment.Warehouse_ID = Convert.ToInt32( user.WareHouse_ID);
                db.Entry(installment).State = EntityState.Modified;

                //
                
                //

                var time = DateTime.Now.Date;
                var daialy = db.DailyActivities.Where(i => i.Warehouse_ID == user.WareHouse_ID && i.DayDate ==time).SingleOrDefault();
                if (daialy != null)
                {
                    daialy.cashierID = CurrentUserName;
                    daialy.DayDate = DateTime.Now;
                    daialy.ExpensesDesc = installment.Comment;
                    daialy.Entered_By = CurrentUserName;
                    if (daialy.Status != "Closed" && daialy.DayDate.Value.Date == DateTime.Now.Date)
                    {
                        daialy.paymentAmount = ResidualAmt;
                        daialy.OpenningBal = daialy.OpenningBal + ResidualAmt;
                    }


                    daialy.ExpensesDesc = installment.Comment;
                    daialy.Warehouse_ID = Convert.ToInt32(user.WareHouse_ID);
                    daialy.Branch_ID = Convert.ToInt32(user.Branch_ID);
                    db.Entry(daialy).State = EntityState.Modified;

                }
          

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name", installment.Cus_ID);
            return View(installment);
        }

        // GET: /Installments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Installment installment = db.Installments.Find(id);
            if (installment == null)
            {
                return HttpNotFound();
            }
            return View(installment);
        }

        // POST: /Installments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Installment installment = db.Installments.Find(id);
            db.Installments.Remove(installment);
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
