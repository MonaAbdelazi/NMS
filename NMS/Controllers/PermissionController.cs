using NMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Ajax.Utilities;
using NMS.Models;

namespace NMS.Controllers
{
    [Authorize(Roles = "Admins")]
    public class PermissionController : BaseController
    {
        string ASSEMBLYNAME = "NMS.dll";

        //
        private NaalaxAuthEntities2 db = new NaalaxAuthEntities2();
        // GET: /Permission/
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetActionByController(string controller)
        {
            List<DropDownData> viewNames = new List<DropDownData>();
            System.Reflection.Assembly a = Assembly.LoadFrom(HttpContext.Server.MapPath("~/bin/" + ASSEMBLYNAME));
            IEnumerable<Type> controllers = a.GetTypes().Where(t =>
                typeof(Controller).IsAssignableFrom(t)).OrderBy(o => o.Name);
            foreach (var item in controllers)
            {
                if (item.Name.Replace("Controller", "") == controller)
                {
                    IEnumerable<MemberInfo> memberInfos = item.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                        .Where(o => !o.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()).DistinctBy(o => o.Name)
                        .OrderBy(x => x.Name);
                    foreach (var member in memberInfos)
                    {
                        if (member.ReflectedType.IsPublic && !member.IsDefined(typeof(NonActionAttribute)))
                        {
                            viewNames.Add(new DropDownData
                            {
                                Text = member.Name,
                                Value = member.Name
                            });
                        }
                    }
                }

            }
            return Json(viewNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetControllerByArea(string area)
        {
            string areapath = "RtecERP.Areas." + area + ".Controllers";
            List<DropDownData> controllerNames = new List<DropDownData>();
            Assembly a = Assembly.LoadFrom(HttpContext.Server.MapPath("~/bin/" + ASSEMBLYNAME));

            var controllers = from t in Assembly.GetExecutingAssembly().GetTypes()
                              where t.IsClass && t.Namespace == areapath
                                    && (t.IsSubclassOf(typeof(Controller)) || t.IsSubclassOf(typeof(Base)))
                              select t;

            foreach (var item in controllers)
            {
                controllerNames.Add(new DropDownData
                {
                    Text = item.Name.Replace("Controller", ""),
                    Value = item.Name.Replace("Controller", "")
                });

            }
            return Json(controllerNames, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPermissionByView(string Controller, string View, string UserId)//string Area,
        {
            bool result = db.AspNetUserPermissions.Any(o => o.Controller ==Controller && (o.Controller == Controller || o.Controller == "*") && (o.Vieww == View || o.Vieww == "*")
                && o.UserId == UserId && o.CanAccess == "true");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        class DropDownData
        {
            public string Text { get; set; }
            public string Value { get; set; }

        }
        public ActionResult Permissions()
        {
            //   List<PAGE> controllers = db.PAGES.ToList().DistinctBy(o => o.CONTROLLER).ToList();
          //  List<DropDownData> areaNames = new List<DropDownData>();
            List<DropDownData> controllerNames = new List<DropDownData>();
            List<DropDownData> viewNames = new List<DropDownData>();

            Assembly a = Assembly.LoadFrom(HttpContext.Server.MapPath("~/bin/" + ASSEMBLYNAME));

            //List<Route> areas = RouteTable.Routes.OfType<Route>().Where(o => o.DataTokens != null && o.DataTokens.ContainsKey("area"))
            //     .ToList();
            //foreach (var area in areas)
            //{
            //    areaNames.Add(new DropDownData
            //    {
            //        Text = area.DataTokens["area"].ToString(),
            //        Value = area.DataTokens["area"].ToString()
            //    });
            //}

            IEnumerable<Type> controllers = a.GetTypes().Where(t =>
                typeof(Controller).IsAssignableFrom(t)).OrderBy(o => o.Name);
            foreach (var item in controllers)
            {
                controllerNames.Add(new DropDownData
                {
                    Text = item.Name.Replace("Controller", ""),
                    Value = item.Name.Replace("Controller", "")
                });

            }
            IEnumerable<MemberInfo> memberInfos = controllers.FirstOrDefault().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(o => !o.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()).OrderBy(x => x.Name);
            foreach (var item in memberInfos)
            {
                if (item.ReflectedType.IsPublic && !item.IsDefined(typeof(NonActionAttribute)))
                {
                    viewNames.Add(new DropDownData { Text = item.Name, Value = item.Name });
                }
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers.ToList(), "Id", "UserName");
           // ViewBag.Area = new SelectList(areaNames, "Value", "Text");
            ViewBag.Controller = new SelectList(controllerNames, "Value", "Text");
            ViewBag.Action = new SelectList(viewNames, "Value", "Text");
            return View();
        }
        [HttpPost]
        public ActionResult Permissions(AspNetUserPermission permission, string CanAccess)
        {
            //o.Area == permission.Area &&
            bool hasRow = !db.AspNetUserPermissions.Any(o => 
                                                             o.UserId == permission.UserId &&
                                                             (o.Controller == permission.Controller ||
                                                              o.Controller == "*") &&
                                                             (o.Vieww == permission.Vieww ||
                                                             o.Vieww == "*")
                                                             && o.CanAccess == CanAccess);


            if (ModelState.IsValid && hasRow)
            {

                if (!db.AspNetUserPermissions.Any())
                {
                    permission.PermissionId = 0;
                }
                else
                {
                    permission.PermissionId = db.AspNetUserPermissions.Max(o => o.PermissionId) + 1;
                }
                permission.CanAccess = CanAccess;
                db.AspNetUserPermissions.Add(permission);
                db.SaveChanges();
                return RedirectToAction("Permissions");
            }



           // List<DropDownData> areaNames = new List<DropDownData>();
            List<string> controllerNames = new List<string>();
            List<string> viewNames = new List<string>();

            Assembly a = Assembly.LoadFrom(HttpContext.Server.MapPath("~/bin/" + ASSEMBLYNAME));

            List<Route> areas = RouteTable.Routes.OfType<Route>().Where(o => o.DataTokens != null && o.DataTokens.ContainsKey("area"))
                .ToList();
            //foreach (var area in areas)
            //{
            //    areaNames.Add(new DropDownData
            //    {
            //        Text = area.DataTokens["area"].ToString(),
            //        Value = area.DataTokens["area"].ToString()
            //    });
            //}

            IEnumerable<Type> controllers = a.GetTypes().Where(t =>
                typeof(Controller).IsAssignableFrom(t)).OrderBy(o => o.Name);
            foreach (var item in controllers)
            {
                controllerNames.Add(item.Name.Replace("Controller", ""));

            }
            IEnumerable<MemberInfo> memberInfos = controllers.FirstOrDefault().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(o => !o.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any()).OrderBy(x => x.Name);
            foreach (var item in memberInfos)
            {
                if (item.ReflectedType.IsPublic && !item.IsDefined(typeof(NonActionAttribute)))
                {
                    viewNames.Add(item.Name);
                }
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers.ToList(), "Id", "UserName");
           // ViewBag.Area = new SelectList(areaNames, "Value", "Text");
            ViewBag.Controller = new SelectList(controllerNames, "Value", "Text");
            ViewBag.Action = new SelectList(viewNames, "Value", "Text");
            return View();
        }
	}
}