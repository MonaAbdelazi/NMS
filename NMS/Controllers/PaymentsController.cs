 
﻿using System;
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
    [Authorize]
    public class PaymentsController : BaseController
    {
        private Inventory_ManagementXEntities db = new Inventory_ManagementXEntities();
        private NaalaxAuthEntities2 dbuser = new NaalaxAuthEntities2();
        // GET: /Payments/
        public ActionResult Index()
        {
            var payments = db.Payments.Include(p => p.Customer).Include(p => p.Installment);
            return View(payments.ToList());
        }
        public ActionResult IndexJournals()
        {
            var journals = db.Transactions;
            return View(journals.ToList());
        }
        public JsonResult getinst(int Cus_ID)
        {

            try

            {

                var data = new SelectList(db.Installments.Where(o => o.Cus_ID == Cus_ID), "Inst_ID", "Inst_ID");



                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult getPaiddata(int Cus_ID, int Inst_ID)
        {

            try

            {

                var data = db.Installments.Where(o => o.Inst_ID == Inst_ID && o.Cus_ID == Cus_ID).Select(i => i.Paid).SingleOrDefault();



                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        //
        public JsonResult getAmountermin(int Cus_ID, int Inst_ID)
        {


            try

            {

                var data = db.Installments.Where(o => o.Inst_ID == Inst_ID && o.Cus_ID == Cus_ID).Select(i => i.ResidualAmt).SingleOrDefault();



                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult getAmounttotal(int Cus_ID, int Inst_ID)
        {


            try

            {

                var data = db.Installments.Where(o => o.Inst_ID == Inst_ID && o.Cus_ID == Cus_ID).Select(i => i.Amount).SingleOrDefault();



                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult getPaidins(int Cus_ID, int Inst_ID)
        {

            try

            {

                var data = db.Installments.Where(o => o.Inst_ID == Inst_ID && o.Cus_ID == Cus_ID).Select(i => i.numPaidinst).SingleOrDefault();



                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult getreminins(int Cus_ID, int Inst_ID)
        {




            try

            {

                var data = db.Installments.Where(o => o.Inst_ID == Inst_ID && o.Cus_ID == Cus_ID).Select(i => i.ResidualIns).SingleOrDefault();



                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult gettotalins(int Cus_ID, int Inst_ID)
        {

            try

            {

                var data = db.Installments.Where(o => o.Inst_ID == Inst_ID && o.Cus_ID == Cus_ID).Select(i => i.No_Of_Inst).SingleOrDefault();



                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        // GET: /Payments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: /Payments/Create
        public ActionResult Create()
        {
            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name");
            return View();
        }
        
             public ActionResult Journals()
        {
         ViewBag.PaymentType = new SelectList(CommonUtils.getPaymentType(), "Value", "Text");
            ViewBag.Accounts = new SelectList(db.AccountManagments, "SystemId", "Acc_Name");
            // ViewBag.BankAccounts = new SelectList(db.AccountManagments.Where(i => i.Type == "Internal" && i.CustomerId == 0 && i.Tree.Category == "I"), "SystemId", "Acc_Name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Journals(Transactions transaction)
        {
            try
            {
                if (Session["trans"] != null)
                {
                    List<Transactions> tranlist = (List<Transactions>)Session["trans"];
                    long id = 0;
                    if (db.Transactions.Any())
                        id = db.Transactions.Max(i => i.SystemId);

                    foreach (var item in tranlist)
                    {
                        var curObj = new Transactions();
                        id += 1;
                        curObj.SystemId = id;
                        curObj.Cr_Acc = item.Cr_Acc;
                        curObj.Cr_Amt = item.Cr_Amt;
                        //  curObj.Currency = item.Currency;
                        curObj.Dr_Acc = item.Dr_Acc;
                        curObj.Dr_Amt = item.Dr_Amt;
                        curObj.Label = item.Label;
                        curObj.TranReference = item.SystemId.ToString();
                        curObj.transactionDate = transaction.transactionDate;
                        curObj.Status = "Active";
                        curObj.LastUpdate = DateTime.Now;
                        curObj.Entered_By = CurrentUserName;
                        db.Transactions.Add(curObj);
                        db.SaveChanges();
                    }
                    Session["trans"] = null;

                    CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);
                    return RedirectToAction("Index");
                }
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View();
            }
            catch (Exception ex)
            {
                CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Error);
                return View(transaction);
            }


            return View(transaction);
        }

        public JsonResult checknumbersBefore()
        {
            
                List<Transactions> translist = new List<Transactions>();
                if (Session["trans"] != null)
                    translist = (List<Transactions>)Session["trans"];
                decimal sumDr = 0;
                decimal sumCr = 0;
                if (translist.Count > 0)
                {
                    foreach (var item in translist)
                    {
                        sumCr += item.Cr_Amt;
                        sumDr += item.Dr_Amt;
                    }
                }
                if (sumCr != sumDr)
                {
                    TempData["AlertMessage"] = "Errors In Credits and Debits ";
                    CommonUtils.SetFeedback("Errors In Credits and Debits", Feedback.Feedback_Error);



                    return Json("E", JsonRequestBehavior.AllowGet);
                }
                else
                    return Json("OK", JsonRequestBehavior.AllowGet);
            
           

        }


        public JsonResult AddJournals(string Dr_Acc, string Note, string Dr_Amt, string Cr_Amt)
        {

            var trans= new List<Transactions>();
            if (Session["trans"] != null)
                trans = (List<Transactions>)Session["trans"];
            var TranObj = new Transactions();
            decimal dr = 0;
            if (!string.IsNullOrEmpty(Dr_Amt))
                dr = decimal.Parse(Dr_Amt);
            decimal cr = 0;
            if (!string.IsNullOrEmpty(Cr_Amt))
                cr = decimal.Parse(Cr_Amt);
            if (dr > 0 && !string.IsNullOrEmpty(Dr_Acc))
            TranObj.Dr_Acc =long.Parse(Dr_Acc);
            if (cr > 0 && !string.IsNullOrEmpty(Dr_Acc))
                TranObj.Cr_Acc = long.Parse(Dr_Acc);
            TranObj.Label = Note;
            TranObj.Dr_Amt = dr;
            TranObj.Cr_Amt = cr;
            trans.Add(TranObj);
            Session["trans"] = trans;
            string resultsHtml = CommonUtils.RenderPartialViewToString(this, "~/Views/Payments/Partial/_TransAddPartial.cshtml", trans.ToList());
            return Json(resultsHtml, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreatePayment()
        {
            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name"); 
            ViewBag.Type = new SelectList(CommonUtils.getCustomerBanks(), "Value", "Text");
            ViewBag.VendorId = new SelectList(db.Suppliers, "Supp_ID", "Supp_Name");
            ViewBag.PaymentType = new SelectList(CommonUtils.getPaymentType(), "Value", "Text"); 
            ViewBag.CashAccounts = new SelectList(db.AccountManagments.Where(i=>i.Type=="Internal" && i.CustomerId==0 && i.Tree.Category== "A"), "SystemId", "Acc_Name");
           // ViewBag.BankAccounts = new SelectList(db.AccountManagments.Where(i => i.Type == "Internal" && i.CustomerId == 0 && i.Tree.Category == "I"), "SystemId", "Acc_Name");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePayment(Payment payment ,string CashAccounts, string BankAccounts)
        {
            if (ModelState.IsValid)
            {
                var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
                var time = DateTime.Now.Date;
                var daialy = db.DailyActivities.Where(i => i.Warehouse_ID == user.WareHouse_ID && i.DayDate.Value.Day == time.Day && i.DayDate.Value.Month == time.Month && i.DayDate.Value.Year == time.Year).SingleOrDefault();
                if (daialy != null)
                {
                    daialy.cashierID = CurrentUserName;
                    daialy.DayDate = DateTime.Now;
                    daialy.ExpensesDesc = payment.comment;
                    daialy.Entered_By = CurrentUserName;
                    if (daialy.Status != "Closed" && daialy.DayDate.Value.Day == time.Day && daialy.DayDate.Value.Month == time.Month && daialy.DayDate.Value.Year == time.Year)
                    {
                        daialy.paymentAmount = daialy.paymentAmount + payment.currntamount;
                        daialy.OpenningBal = daialy.OpenningBal + payment.currntamount;
                    }



                    daialy.Warehouse_ID = Convert.ToInt32(user.WareHouse_ID);
                    daialy.Branch_ID = Convert.ToInt32(user.Branch_ID);
                    db.Entry(daialy).State = EntityState.Modified;

                }
                else
                {
                    CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Warning);
                    return RedirectToAction("CreatePayment");

                }
                //
                var id = 0;
                if (db.Payments.Any())
                {
                    id = db.Payments.Max(i => i.PayID);
                }
                payment.PayID = ++id;
                payment.paiddate = DateTime.Now;
                payment.Status = "Active";
                payment.Last_Update = DateTime.Now;
                payment.Enterd_By = CurrentUserName;
                payment.Warehouse_ID = Convert.ToInt32(user.WareHouse_ID);
                db.Payments.Add(payment);
                //trans
                Transactions tran = new Transactions();
                long tranid =0 ;
                if (db.Transactions.Any())
                {
                    tranid = db.Transactions.Max(i => i.SystemId);
                }
                tran.SystemId = ++tranid;
                tran.transactionDate = DateTime.Now;
                tran.Status = "Active";
                tran.LastUpdate = DateTime.Now;
                tran.Entered_By = CurrentUserName;
                switch (payment.PaymentType)
                {
                    case "cash":
                        tran.Dr_Acc = long.Parse(CashAccounts);
                        break;
                    case "bank":
                        if(payment.Cus_ID>0)//to be changed to ACC No
                        tran.Dr_Acc =payment.Customer.Cus_ID;
                        if (payment.VendorId > 0)
                            tran.Dr_Acc = payment.VendorId;

                        break;
                    default:
                        break;
                }
               int crAcc = db.AccountManagments.Where(i => i.Type == "Internal" && i.CustomerId == 0 && i.Tree.Category == "I").SingleOrDefault().Acc_No;
                tran.Cr_Acc =long.Parse(crAcc.ToString());
                tran.Cr_Amt =decimal.Parse(payment.currntamount.ToString());
                tran.Dr_Amt = decimal.Parse(payment.currntamount.ToString());
                tran.TranReference = id.ToString();
                db.Transactions.Add(tran);
                //End of trans
                db.SaveChanges();
                CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);

                return RedirectToAction("CreatePayment");
            }

            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name", payment.Cus_ID);
            ViewBag.Inst_ID = new SelectList(db.Installments, "Inst_ID", "Comment", payment.Inst_ID);
            return View(payment);
        }


        // POST: /Payments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PayID,currntamount,paiddate,currntinst,Cus_ID,Inst_ID,comment,Status,Last_Update,Enterd_By,Warehouse_ID")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                //

                var user = dbuser.AspNetUsers.Where(i => i.Id == CurrentUserID).SingleOrDefault();
                var time = DateTime.Now.Date;
                var daialy = db.DailyActivities.Where(i => i.Warehouse_ID == user.WareHouse_ID && i.DayDate.Value.Day == time.Day && i.DayDate.Value.Month == time.Month && i.DayDate.Value.Year == time.Year).SingleOrDefault();
                if (daialy != null)
                {
                    daialy.cashierID = CurrentUserName;
                    daialy.DayDate = DateTime.Now;
                    daialy.ExpensesDesc = payment.comment;
                    daialy.Entered_By = CurrentUserName;
                    if (daialy.Status != "Closed" && daialy.DayDate.Value.Day == time.Day && daialy.DayDate.Value.Month == time.Month && daialy.DayDate.Value.Year == time.Year)
                    {
                        daialy.paymentAmount = daialy.paymentAmount + payment.currntamount;
                        daialy.OpenningBal = daialy.OpenningBal + payment.currntamount;
                    }



                    daialy.Warehouse_ID = Convert.ToInt32(user.WareHouse_ID);
                    daialy.Branch_ID = Convert.ToInt32(user.Branch_ID);
                    db.Entry(daialy).State = EntityState.Modified;

                }
                else
                {
                    CommonUtils.SetFeedback(Feedback.NotSavedSuccessfully, Feedback.Feedback_Warning);
                    return RedirectToAction("Index");

                }

                //
                var installment = db.Installments.Where(i => i.Inst_ID == payment.Inst_ID && i.Cus_ID == payment.Cus_ID).SingleOrDefault();

                installment.Last_Update = DateTime.Now;
                installment.Paid = installment.Paid + payment.currntamount;

                installment.ResidualAmt = Convert.ToDecimal(installment.Amount) - Convert.ToDecimal(installment.Paid);
                installment.numPaidinst = installment.numPaidinst + payment.currntinst;
                installment.ResidualIns = installment.No_Of_Inst - installment.numPaidinst;

                installment.Warehouse_ID = Convert.ToInt32(user.WareHouse_ID);
                db.Entry(installment).State = EntityState.Modified;
                //
                var id = 0;
                if (db.Payments.Any())
                {
                    id = db.Payments.Max(i => i.PayID);
                }
                payment.PayID = ++id;
                payment.paiddate = DateTime.Now;

                payment.Status = "Active";
                payment.Last_Update = DateTime.Now;
                payment.Enterd_By = CurrentUserName;
                payment.Warehouse_ID = Convert.ToInt32(user.WareHouse_ID);
                db.Payments.Add(payment);

                db.SaveChanges();
                CommonUtils.SetFeedback(Feedback.SavedSuccessfully, Feedback.Feedback_Success);

                return RedirectToAction("Index");
            }

            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name", payment.Cus_ID);
            ViewBag.Inst_ID = new SelectList(db.Installments, "Inst_ID", "Comment", payment.Inst_ID);
            return View(payment);
        }

        // GET: /Payments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name", payment.Cus_ID);
            ViewBag.Inst_ID = new SelectList(db.Installments, "Inst_ID", "Comment", payment.Inst_ID);
            return View(payment);
        }

        // POST: /Payments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PayID,currntamount,paiddate,currntinst,Cus_ID,Inst_ID,comment,Status,Last_Update,Enterd_By,Warehouse_ID")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Cus_ID = new SelectList(db.Customers, "Cus_ID", "Cus_Name", payment.Cus_ID);
            ViewBag.Inst_ID = new SelectList(db.Installments, "Inst_ID", "Comment", payment.Inst_ID);
            return View(payment);
        }

        // GET: /Payments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = db.Payments.Find(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: /Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payment payment = db.Payments.Find(id);
            db.Payments.Remove(payment);
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
 