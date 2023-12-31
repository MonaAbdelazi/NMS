﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NMS.Tools
{
    /// <summary>

    /// A strongly-type extension method for accessing TempData which

    /// is used to store our Alerts.

    /// </summary>

    public static class AlertExtensions
    {

        private const string Alerts = "_Alerts";



        public static List<Alert> GetAlerts(this TempDataDictionary tempData)
        {

            if (!tempData.ContainsKey(Alerts))
            {

                tempData[Alerts] = new List<Alert>();

            }



            return (List<Alert>)tempData[Alerts];

        }



        // helper methods to simplify the creation of the AlertDecoratorResult types

        public static ActionResult WithSuccess(this ActionResult result, string message)
        {

            return new AlertDecoratorResult(result, "success", message);

        }



        public static ActionResult WithInfo(this ActionResult result, string message)
        {

            return new AlertDecoratorResult(result, "info", message);

        }



        public static ActionResult WithWarning(this ActionResult result, string message)
        {

            return new AlertDecoratorResult(result, "warning", message);

        }



        public static ActionResult WithError(this ActionResult result, string message)
        {

            return new AlertDecoratorResult(result, "error", message);

        }

    }
}