using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NMS.Tools
{
    public class BaseController : Controller
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {

            base.Initialize(requestContext);

            CultureInfo culture = CultureInfo.CreateSpecificCulture("ar-SA");

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture.NumberFormat.DigitSubstitution = DigitShapes.Context;
            CultureInfo.DefaultThreadCurrentCulture.DateTimeFormat = CultureInfo.CreateSpecificCulture("en-GB").DateTimeFormat;
            CultureInfo.DefaultThreadCurrentUICulture.DateTimeFormat = CultureInfo.CreateSpecificCulture("en-GB").DateTimeFormat;


        }
    }
}