using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.Threading;
 
using System.Configuration;
using NMS.Core.Resources;
 


namespace NMS
{

    [Authorize]
    public class Base : Controller
    {

        public string CurrentUserName
        {
            get { return User.Identity.Name ;  }
        }

       
        

        //

        public string CurrentUserID
        {
            get { return User.Identity.GetUserId(); }
        }
        public int CurrentEmpNo
        {
            get { return Session["Name"] == null ? 0 : int.Parse(Session["Name"].ToString()); }
        }
        public string lang
        {
            get { return CommonRes.Lang; }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
              //  filterContext.Result = CheckPermission(filterContext);
            }
            base.OnAuthorization(filterContext);
        }

        [OutputCache(Duration = 86400)]
        //private ActionResult CheckPermission(AuthorizationContext filterContext)
        //{

        //    IAuthorization authorization = new AuthorizationClass(filterContext);
        //    return authorization.CheckPermission() ? null : Redirect("/Users/Account/AccessDenied");

        //}

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            Utility.CommonUtils.SetFeedback("Exception", Feedback.Feedback_Warning);
            Response.Redirect("~/Account/Exception");
        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {

            base.Initialize(requestContext);

            if (Session["Culture"] == null)
                Session["Culture"] = "ar";

            if (Session["Culture"].ToString() == "en")
            {
                Session["Layout"] = "~/Areas/Users/Views/Shared/_Layout-En.cshtml";
            }
            else
            {
                Session["Layout"] = "~/Areas/Users/Views/Shared/_Layout_Ar.cshtml";
            }


            CultureInfo culture = CultureInfo.CreateSpecificCulture(Session["Culture"].ToString() == "en" ? "en-US" : "ar-SA");

            setCultureLang(culture);


        }

        private static void setCultureLang(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture.NumberFormat.DigitSubstitution = DigitShapes.Context;
            CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat = CultureInfo.CreateSpecificCulture("en-GB").DateTimeFormat;
            CultureInfo.DefaultThreadCurrentUICulture.DateTimeFormat = CultureInfo.CreateSpecificCulture("en-GB").DateTimeFormat;
            CommonRes.Culture = culture;
            //Feedback.Culture = culture;
            //Resources.HrResources.Culture = culture;
            MenuRes.Culture = culture;
           
        }

        

    }

    public class BaseController : Base
    {

    }
}