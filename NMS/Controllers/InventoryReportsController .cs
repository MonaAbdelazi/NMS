using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NMS.Core.Resources;
using NMS.Tools;
using NMS.Data;
using NMS.Utility;
using System.Configuration;
using System.Data.SqlClient;

namespace NMS.Controllers
{

   // [Authorize(Roles = "Admins")]
    public class InventoryReportsController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        private NaalaxAuthEntities2 dbuser = new NaalaxAuthEntities2();

        // GET: /Company_Type/DialyTransactions
        public ActionResult InventoryStatus()
        {
            var user = CurrentUserID;
            AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();

            ViewBag.Product_ID = new SelectList(db.Products.Where(i=>i.Status!=""), "Product_ID", "Name_AR");
            ViewBag.Warehouse_ID = new SelectList(db.WareHouses, "Warehouse_ID", "Name");
            ViewBag.Status = new SelectList(CommonUtils.getStatus(), "Value", "Text");
            var itemList = from b in db.Items
                           where b.Status == "Active" && b.Warehouse_ID == userlogged.WareHouse_ID
                           select new { desc = b.Product.Name_AR, code = b.Item_ID };
            ViewBag.Item_ID = new SelectList(itemList, "code", "desc");
            return View();
        }

        public ActionResult Installments()
        {
            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name");
            ViewBag.Inst_ID = new SelectList(db.Installments, "Inst_ID", "Inst_ID");
            return View();
        }
        public ActionResult IncomeExpenses()
        {
            ViewBag.warehouse = new SelectList(db.WareHouses, "Warehouse_ID", "Name_AR");
            return View();
        }
        public ActionResult Invoices()
        {
            var invoices = db.Invoices.Include(i => i.Branch).Include(i => i.Company).Include(i => i.Customer).Include(i => i.Suppliers).Include(i => i.Unit).Include(i => i.WareHouse);
            return View(invoices.ToList());

        }

        public ActionResult rptnstallments(int Cus_ID)
        {
            var dt = new List<instViewMode>();

            Installment inv = db.Installments.Where(i => i.Cus_ID == Cus_ID).Include(i => i.Customer).Include(i => i.Invoice).Include(i => i.Payments).SingleOrDefault();
            if (inv != null)
            {
                instViewMode obj = new instViewMode();
                obj.Amount = inv.Amount;
                if (inv.currntamount != null)
                    obj.currntamount = decimal.Parse(inv.currntamount.ToString());
                else
                    obj.currntamount = 0;
                if (inv.currntinst != null)

                    obj.currntinst = int.Parse(inv.currntinst.ToString());
                else
                    obj.currntinst = 0;
                if (inv.Cus_ID != null)
                    obj.Cus_ID = int.Parse(inv.Cus_ID.ToString());
                obj.EndDate = inv.EndDate;
                obj.Inst_ID = inv.Inst_ID;
                obj.Invoice = inv.Invoice;
                obj.Customer = inv.Customer;
                obj.Invoice_ID = inv.Invoice_ID;
                if (inv.numPaidinst != null)

                    obj.No_Of_Inst = int.Parse(inv.numPaidinst.ToString());
                if (inv.Paid != null)

                    obj.Paid = decimal.Parse(inv.Paid.ToString());
                if (inv.paiddate != null)

                    obj.paiddate = DateTime.Parse(inv.paiddate.ToString());
                obj.Payments = inv.Payments;
                if (inv.ResidualAmt != null)

                    obj.ResidualAmt = int.Parse(inv.ResidualAmt.ToString());
                if (inv.ResidualIns != null)

                    obj.ResidualIns = int.Parse(inv.ResidualIns.ToString());
                obj.StartDate = inv.StartDate;
                obj.WareHouse = inv.WareHouse;
                obj.Warehouse_ID = inv.Warehouse_ID;

                dt.Add(obj);
                var paremeters = new List<KeyValuePair<string, string>>();
                if (Cus_ID != null && Cus_ID > 0)
                {
                    var Customer = db.Customers.Where(i => i.Cus_ID == Cus_ID).SingleOrDefault();

                    paremeters.Add(new KeyValuePair<string, string>("custName", Customer.Cus_Name_AR.ToString()));
                }
                else
                {
                    paremeters.Add(new KeyValuePair<string, string>("custName", "NONE"));

                }

                //ViewReportLocal("rptStandardReport", dt, paremeters);

                Session["ReportParameter"] = paremeters;
                Session["ReportData"] = dt;
                Session["ReportName"] = "rptInstallments";
               
                return Json(true, JsonRequestBehavior.AllowGet);


            }
            else
            {//
             // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                return View();

            }


        }


        public ActionResult PrintInvoice(int id)
        {

            int Invoice_ID = id;
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
                vm.Discount = Convert.ToInt16(item.Discount);
                vm.Invoice_ID = item.Invoice_ID;
                vm.Entered_By = item.Entered_By;
                vm.comment = item.comment;
                vm.Approved_By = "admin";// Convert.ToString( item.Approved_By);
                vm.Issue_Date = DateTime.Parse(item.Issue_Date.ToString());



                dt.Add(vm);
            }
            //List<Invoice_Items> listitems = db.Invoice_Items.Where(i => i.Invoice_ID == Invoice_ID).ToList();
            //var dt = new List<Invoice_Items>();
            //foreach (var item in listitems)
            //{
            //    item.Item = db.Items.Where(i => i.Item_ID == item.Item_ID).SingleOrDefault();
            //    item.Invoice = db.Invoices.Where(i => i.Invoice_ID == item.Invoice_ID).SingleOrDefault();
            //    item.Discount = item.Discount;
            //    item.WareHouse_ID = item.WareHouse_ID;

            //    dt.Add(item);
            //}
            if (dt != null && dt.Count > 0)
            {

                //    List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>();





                var paremeters = new List<KeyValuePair<string, string>>();

              //  paremeters.Add(new KeyValuePair<string, string>("Name", "Active"));
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
                if (inv.Status.Trim() == "Active")

                    paremeters.Add(new KeyValuePair<string, string>("Status", "قيد الدفع"));
                if (inv.Status.Trim() == "Approved")

                    paremeters.Add(new KeyValuePair<string, string>("Status", "مدفوع"));

                //ViewReportLocal("rptStandardReport", dt, paremeters);

                Session["ReportParameter"] = paremeters;
                Session["ReportData"] = dt;
                Session["ReportName"] = "rptInvoices";
                if (dt.ToList().Count >= 1)
                {
                    Session["ReportOption"] = null;

                }
                else
                {
                    Session["ReportOption"] = "list";
                }

                return Json(true, JsonRequestBehavior.AllowGet);


            }
            else
            {//
             // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                return View();

            }


        }

        public ActionResult Print(int id)
        {
            Invoice inv = db.Invoices.Where(i => i.Invoice_ID == id).SingleOrDefault();


            return View(inv);


        }
        public ActionResult DialyTransactions()
        {
            var user = CurrentUserID;
            AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();

            ViewBag.Branch_ID = new SelectList(db.Branches, "Branch_ID", "Branch_Name");
            ViewBag.WareHouses_ID = new SelectList(db.WareHouses.Where(i=>i.Status=="Active"), "Warehouse_ID", "Name_AR");

            ViewBag.User_ID = new SelectList(dbuser.AspNetUsers, "Id", "UserName");

            return View();

        }

        public JsonResult rptInventoryStatus(string Item_ID, string WareHouse_ID, string Product_ID)
        {

            List<InventoryViewModel> dt = new List<InventoryViewModel>();
            //from record in wowReportData
            //group record by new { record.ProductID, record.Name } into grouping
            //orderby grouping.Key.ProductID
            //select new topProduct
            //{
            //    ProductID = grouping.Key.ProductID,
            //    Quantity = grouping.Sum(t => t.Quantity),
            //    Name = grouping.Key.Name
            //}
            var listitems = from b in db.Items.Where(i=>i.Status=="Active").ToList()
                            group b by new { b.Product_ID, b.Warehouse_ID } into groups
                            select new
                            {
                                Name_AR = groups.Select(t => t.Product.Name_AR),
                                warename = groups.Select(t => t.WareHouse.Name_AR)
                            ,
                                Qunt = groups.Sum(t => t.Qunt),
                                AvailableQ = groups.Sum(t => t.AvailableQ),
                                SoldQ = groups.Sum(t => t.SoldQ),
                                Item_ID = groups.Sum(t => t.Item_ID),
                                Warehouse_ID = groups.Key.Warehouse_ID,
                                Product_ID = groups.Key.Product_ID,
                               

                            }

                        ;
            if (!string.IsNullOrEmpty(Item_ID))
                listitems = listitems.Where(i => i.Item_ID == int.Parse(Item_ID) );
            if (!string.IsNullOrEmpty(WareHouse_ID))
                listitems = listitems.Where(i => i.Warehouse_ID == int.Parse(WareHouse_ID));
            if (!string.IsNullOrEmpty(Product_ID))
                listitems = listitems.Where(i => i.Product_ID == int.Parse(Product_ID));

            if (Session["Role"].ToString() != "Admins")
            {
                var user = CurrentUserID;
                AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();

                listitems = listitems.Where(i => i.Warehouse_ID == userlogged.WareHouse_ID);
            }
            foreach (var item in listitems.ToList())
            {
                InventoryViewModel vm = new InventoryViewModel();
                vm.AvailableQ = int.Parse(item.AvailableQ.ToString());
                vm.Item_name = (item.Name_AR.ToList()[0].ToString());
                vm.Ware_Name = db.WareHouses.Where(i => i.Warehouse_ID == item.Warehouse_ID).Select(i => i.Name_AR).SingleOrDefault();
                vm.Qunt = int.Parse(item.Qunt.ToString());
                vm.SoldQ = int.Parse(item.SoldQ.ToString());
                dt.Add(vm);
            }
            if (dt != null && dt.ToList().Count > 0)
            {
                Session["ReportData"] = dt;
                Session["ReportName"] = "rptDialyTransaction";

                return Json(true, JsonRequestBehavior.AllowGet);


            }
            else
            {
                // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                return Json(false, JsonRequestBehavior.AllowGet);

            }

        }

        //        string sql = string.Format(
        //                               "select (select Name_AR from Product where Product_ID = a.Product_ID) as Name_AR ,"+
        //"(select Name_AR from WareHouse where Warehouse_ID = a.Warehouse_ID) as WName_AR,"+
        //"sum(Qunt) as Qunt, sum(AvailableQ) as AvQ, sum(SoldQ) as SolQ, Warehouse_ID "

        //+"from Items a  Where a.Status='Active'");
        //if (Status != null)
        //       {
        //            sql = string.Format("{0} and Status IN ({1})", sql, String.Join(",", Status));

        //        }
        //if (!string.IsNullOrEmpty(Item_ID))
        //    sql = string.Format("{0} and Item_ID='{1}' ", sql, Item_ID);
        //if (!string.IsNullOrEmpty(WareHouse_ID) )
        //    sql = string.Format("{0} and WareHouse_ID='{1}'", sql, WareHouse_ID);
        //if (!string.IsNullOrEmpty(Product_ID))
        //    sql = string.Format("{0} and Product_ID='{1}'", sql, Product_ID);
        //
        //if (Session["Role"].ToString() != "Admin")
        //{
        //    var user = CurrentUserID;
        //    AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();

        //    sql = string.Format("{0} and WareHouse_ID='{1}'", sql, userlogged.WareHouse_ID);
        //}

        //sql = string.Format("{0} group by Warehouse_ID, Product_ID", sql);

        //    foreach (var item in itemListgroup by Warehouse_ID, Product_ID
        //    {
        //        //item.Item_Grp_ID = item.Item_Grp_ID ?? 0;
        //        //item.Qunt = item.Qunt ?? 0;
        //        //item.OpeningBalance = item.OpeningBalance ?? 0;
        //        //item.Price_dozen = item.Price_dozen ?? 0;
        //        //item.PriceForOnce = item.PriceForOnce ?? 0;
        //        //item.Max_Limit = item.Max_Limit ?? 0;
        //        //item.Min_Limit = item.Min_Limit ?? 0;
        //        //item.Exp_Date = item.Exp_Date ?? DateTime.Now;
        //        //item.Country_ID = item.Country_ID ?? 0;
        //        //item.Comp_ID = item.Comp_ID ?? 0;
        //        //item.City_ID = item.City_ID ?? 0;
        //        //item.Warehouse_ID = item.Warehouse_ID ?? 0;
        //        //item.Product_ID = item.Product_ID ?? 0;
        //        //item.Cost = item.Cost ?? 0;
        //        //item.SoldQ = item.SoldQ ?? 0;
        //        //item.AvailableQ = item.AvailableQ ?? 0;
        //        item.Item_name = item.Product.Name_AR;
        //        item.Ware_Name = item.WareHouse.Name_AR;
        //        item.Unit = item.Unit ?? new Unit();
        //        item.WareHouse = item.WareHouse ?? new WareHouse();
        //        item.Product = item.Product ?? new Product();
        //        item.Country = item.Country ?? new Country();
        //        item.Company = item.Company ?? new Company();
        ////        item.City = item.City ?? new City();
        ////    }
        //using (SqlConnection  objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["Inven"].ConnectionString))
        //{
        //    objConn.Open();

        //    SqlTransaction transaction = objConn.BeginTransaction();

        //    SqlCommand objCmd = new SqlCommand();
        //    objCmd.Connection = objConn;
        //    objCmd.Transaction = transaction;

        //    objCmd.CommandType = CommandType.Text;
        //    objCmd.CommandText = sql;
        //    objCmd.Parameters.Clear();
        //    DataTable dt = new DataTable();
        //    using (SqlDataReader rdr = objCmd.ExecuteReader())
        //   {
        //       dt.Load(rdr);

        //   }
        ////   var dt = itemList;



        //}
        // }

        public JsonResult getIncomeExpenses(int? warehouse, string date)
        {
            List<InventoryViewModel> dt = new List<InventoryViewModel>();

            List<DailyActivity> listitems = db.DailyActivities.ToList();
            AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
            WareHouse house = db.WareHouses.Where(i => i.Warehouse_ID == warehouse).SingleOrDefault();

            if (warehouse > 0)
            {
                int warehouseint = int.Parse(warehouse.ToString());

                {
                    listitems = listitems.Where(i => i.Warehouse_ID == warehouseint).ToList();
                }
                if (!string.IsNullOrEmpty(date))
                {
                    listitems = listitems.Where(i => i.DayDate == DateTime.Parse(date)).ToList();
                }

                var totIncome = listitems.Sum(i => i.SoldAmount + i.paymentAmount);
                decimal totExpenses2 = 0;
                List<Exp> gg = db.Exps.Where(i => i.Warehouse_ID == warehouseint).ToList();
                if (gg.Count > 0 && date != "")
                {

                    var da = DateTime.Parse(date);
                    var x = gg.Where(i => i.date == da.Date).ToList();
                    totExpenses2 = Convert.ToDecimal(x.Sum(i => i.Amount));
                }

                else
                {
                    var lis = db.Exps.Where(i => i.Warehouse_ID == warehouseint).ToList();
                    if (lis.Count>0 )
                    {
                        var lis2 = db.Exps.Where(i => i.Warehouse_ID == warehouseint);

                        totExpenses2 = lis2.Sum(i => i.Amount);

                    }
                    else
                    {
                        totExpenses2 = 0;
                    }

                }
                var list = new List<ExpVM>();
                var ware = house.Name_AR;
                foreach (var item in listitems.ToList())
                {
                    var list2 = new ExpVM();

                    list2.WareHouse_ID = ware;
                    list2.totIncome = Convert.ToDecimal( totIncome);
                    list2.totExpenses = Convert.ToDecimal(totExpenses2);
                    list2.daydate = Convert.ToDateTime(item.DayDate);
                    list.Add(list2);
                }
                  //  list.Add(new ExpVM());
                Session["ReportName"] = "rptIncomeExpenses";

                if (list != null && list.ToList().Count > 0)
                {
                    //var paremeters = new List<KeyValuePair<string, string>>();

                    //paremeters.Add(new KeyValuePair<string, string>("totIncome", totIncome.ToString()));

                    //paremeters.Add(new KeyValuePair<string, string>("totExpenses", totExpenses2.ToString()));
                    //paremeters.Add(new KeyValuePair<string, string>("WareHouse_ID", house.Name_AR.ToString()));


                 

                    //Session["ReportParameter"] = paremeters;
                    Session["ReportData"] = list;

                    return Json(true, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                return Json(false, JsonRequestBehavior.AllowGet);

            }


        }


        public JsonResult rptDialyTransaction(string User_ID, string date,int? WareHouses_ID)
        {
            List<DailyTraVM> dt = new List<DailyTraVM>();
            List<Invoice_Items> listitems = new List< Invoice_Items>();
            var invo = db.Invoices.Where(i => i.Status == "Approved").Select(i => i.Invoice_ID);

            if (!string.IsNullOrEmpty(User_ID))
            {
                AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == User_ID).SingleOrDefault();
                listitems = db.Invoice_Items.Where(i=>i.Entered_By== userlogged.UserName && i.Status != "Deleted" && invo.Contains(i.Invoice_ID)).ToList();
            }

            if (WareHouses_ID != null)
            {
                var x = db.Invoice_Items.Where(o => o.WareHouse_ID == WareHouses_ID && o.Status != "Deleted" && invo.Contains(o.Invoice_ID)).ToList();

                listitems = x;

            }
            if (!string.IsNullOrEmpty(date))
            {
                var dat = DateTime.Parse(date);
                if (listitems.Count == 0)
                {
                    listitems= db.Invoice_Items.Where(i => i.Status != "Deleted").ToList();
                }
                var tst2 = listitems;
                var tst = tst2.Where(i => i.Issue_Date == dat.Date&&invo.Contains(i.Invoice_ID)).ToList();
                listitems = tst;
            }
           
            //if (Session["Role"].ToString() != "Admin")
            //{
            //    var user = CurrentUserID;
            //    AspNetUser userlogged = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();

            //    listitems = listitems.Where(i => i.Warehouse_ID == userlogged.WareHouse_ID);
            //}
            foreach (var item in listitems.ToList())
            {
                DailyTraVM vm = new DailyTraVM();
                var ware = db.WareHouses.Where(i => i.Warehouse_ID == item.WareHouse_ID).Select(i => i.Name_AR).SingleOrDefault();
                vm.Item_NameAr = item.Item_NameAr;//.ToList()[0].ToString()
                                                    //   vm.comment= (item.comment.ToList()[0].ToString());
                vm.unit_Price = int.Parse(item.unit_Price.ToString());
                vm.total_Price = int.Parse(item.total_Price.ToString());
                vm.Invoice_ID= int.Parse(item.Invoice_ID.ToString());
                vm.Item_ID = item.Item_ID;
                vm.Quantity = int.Parse(item.Quantity.ToString());
                vm.Entered_By = item.Entered_By;
                vm.last_Updated =DateTime.Parse(item.last_Updated.ToString());
                vm.WareHouse_ID =ware;
                vm.Payment_Method =item.Invoice.Payment_Method;
                dt.Add(vm);
            }
            if (dt != null && dt.ToList().Count > 0)
            {
                Session["ReportData"] = dt;
                Session["ReportName"] = "rptDialyTransaction";
                if (dt.ToList().Count>= 1)
                {
                    Session["ReportOption"] = null;

                }
                else
                {
                    Session["ReportOption"] = "list";
                }

                return Json(true, JsonRequestBehavior.AllowGet);


            }
            else
            {
                // SetFeedback(Feedback.NoDataToShow, Core.Classes.Common.CommonUtils.FEEDBACK_ERROR);
                return Json(false, JsonRequestBehavior.AllowGet);

            }


        }

        // GET: /Company_Type/Create
        public ActionResult Create()
        {
            return View();
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
    public class instViewMode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public instViewMode()
        {
            this.Payments = new HashSet<Payment>();
        }

        public int Inst_ID { get; set; }
        public int Cus_ID { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int No_Of_Inst { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public System.DateTime Last_Update { get; set; }
        public string Enterd_By { get; set; }
        public int Invoice_ID { get; set; }
        public decimal ResidualAmt { get; set; }
        public int ResidualIns { get; set; }
        public int Warehouse_ID { get; set; }
        public decimal Paid { get; set; }
        public int numPaidinst { get; set; }
        public decimal currntamount { get; set; }
        public System.DateTime paiddate { get; set; }
        public int currntinst { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payments { get; set; }
    }


    public class ExpVM
    {
        
       public string WareHouse_ID { get; set; }
        public decimal totExpenses { get; set; }
        public decimal totIncome { get; set; }
        public DateTime daydate { get; set; }

    }
    public class DailyTraVM
    {
        
            public int In_Item_ID { get; set; }
        public string WareHouse_ID { get; set; }
        
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
           public string Payment_Method { get; set; }

        public virtual Invoice Invoice { get; set; }
            public virtual Item Item { get; set; }
        
    }
    public class InventoryViewModel
    {
        public int Item_ID { get; set; }
        public int Item_Grp_ID { get; set; }
        public int Unit_ID { get; set; }
        public int Qunt { get; set; }
        public double OpeningBalance { get; set; }
        public double PriceForOnce { get; set; }
        public double Price_dozen { get; set; }
        public int Min_Limit { get; set; }
        public int Max_Limit { get; set; }
        public System.DateTime Exp_Date { get; set; }
        public int Comp_ID { get; set; }
        public int Country_ID { get; set; }
        public int City_ID { get; set; }
        public string Status { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public string Entered_By { get; set; }
        public int Warehouse_ID { get; set; }
        public int Product_ID { get; set; }
        public decimal Cost { get; set; }
        public int SoldQ { get; set; }
        public int AvailableQ { get; set; }
        public string Item_name { get; set; }
        public string Ware_Name { get; set; }

        public virtual City City { get; set; }
        public virtual Company Company { get; set; }
        public virtual Country Country { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice_Items> Invoice_Items { get; set; }
        public virtual Items_Group Items_Group { get; set; }
        public virtual Product Product { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual WareHouse WareHouse { get; set; }
    }
}
