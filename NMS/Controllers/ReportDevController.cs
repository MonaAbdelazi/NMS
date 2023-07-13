using DevExpress.XtraReports.Web;
using NMS.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NMS.Controllers
{
    public class ReportDevController : Controller
    {
        //
        // GET: /ReportDev/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /ReportDev/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult ContryReport1eport()
        {

            CachedReportSourceWeb cachedReportSource = new CachedReportSourceWeb(new XtraReport1());
            return View(cachedReportSource);
        }

        //
        // GET: /ReportDev/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ReportDev/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ReportDev/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ReportDev/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ReportDev/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ReportDev/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
