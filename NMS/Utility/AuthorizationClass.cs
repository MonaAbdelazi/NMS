using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

using System.Web.Mvc;
using System.Web.UI;
 
using NMS.Utility;
using System.Web.Http.Controllers;
using Microsoft.AspNet.Identity;


namespace NMS.Utility
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IAuthorization" />
    public class AuthorizationClass : IAuthorization
    {
        public string ProjectName { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string UsrId { get; set; }

        public string RoleId { get; set; }

        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public AuthorizationContext FilterContext { get; set; }

        public HttpActionContext HttpFilterContext { get; set; }

        string IAuthorization.ProjectName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        string IAuthorization.Url
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        string IAuthorization.UserName
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        string IAuthorization.Area
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        string IAuthorization.Controller
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        string IAuthorization.Action
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        AuthorizationContext IAuthorization.FilterContext
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        HttpActionContext IAuthorization.HttpFilterContext
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Constructor Method
        /// </summary>
        /// <param name="filterContext"></param>
        public AuthorizationClass(AuthorizationContext filterContext)
        {
            Url = filterContext.HttpContext.Request.Path;
            UsrId = HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.GetUserId();
            //  UsrId = HttpContext.Current.Request.RequestContext.HttpContext.;
            if (HttpContext.Current.Session["Role"] != null)
                RoleId = HttpContext.Current.Session["Role"].ToString();
            UserName = filterContext.HttpContext.User.Identity.Name;
            Area = filterContext.HttpContext.Request.RequestContext.RouteData.DataTokens.ContainsKey("area") ? filterContext.HttpContext.Request.RequestContext.RouteData.DataTokens["area"].ToString() : "";
            Controller = filterContext.HttpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            Action = filterContext.HttpContext.Request.RequestContext.RouteData.Values["action"].ToString();
            ProjectName = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly.GetName().Name;
            FilterContext = filterContext;
        }
        //HttpContext.Current.Request.RequestContext
        public AuthorizationClass(HttpActionContext httpContext)
        {
            Url = HttpContext.Current.Request.Url.AbsolutePath;
            UsrId = HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.GetUserId();
            if (HttpContext.Current.Session["Role"] != null)
                RoleId = HttpContext.Current.Session["Role"].ToString();
            UserName = HttpContext.Current.Request.RequestContext.HttpContext.User.Identity.Name;
            Area = HttpContext.Current.Request.RequestContext.RouteData.DataTokens.ContainsKey("area") ? HttpContext.Current.Request.RequestContext.RouteData.DataTokens.ContainsKey("area").ToString() : "";
            Controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();
            Action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            ProjectName = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly.GetName().Name;
            HttpFilterContext = httpContext;
        }

        bool IAuthorization.CheckPermission()
        {
            throw new NotImplementedException();
        }
    }
}
        /// <summary>
        /// Check if user have permission to request page and return boolean
        /// </summary>
        /// <returns>bool if have permission returen true else fasle </returns>
       // [OutputCache(Duration = 86400)]
        //public bool CheckPermission()
        //{
        //    USRole db = new USRole();
        //    if (String.IsNullOrEmpty(Action))
        //        Action = "Index";
        //    if (String.IsNullOrEmpty(Controller))
        //        Controller = "Default";
        //    List<AspNetUserPermission> permissions = db.AspNetUserPermissions.Where(o =>
        //        (o.UserId == UsrId) &&
        //        (o.Area.ToLower() == Area.ToLower() || o.Area == "*") &&
        //     (o.Controller.ToLower() == Controller.ToLower() || o.Controller == "*") &&
        //     (o.View.ToLower() == Action.ToLower() || o.View == "*") && o.CanAccess == "true").ToList();
        //    if (HttpContext.Current.Session["Role"] != null)
        //    {
        //        List<AspNetRolePermission> rolepermissions = db.AspNetRolePermissions.Where(o =>
        //            (o.RoleId == RoleId) &&
        //            (o.Area.ToLower() == Area.ToLower() || o.Area == "*") &&
        //            (o.Controller.ToLower() == Controller.ToLower() || o.Controller == "*") &&
        //            (o.View.ToLower() == Action.ToLower() || o.View == "*") && o.CanAccess == "true").ToList();
        //        return permissions.Any() || rolepermissions.Any();
        //    }
        //    return permissions.Any();
        //}



   // }
//}
