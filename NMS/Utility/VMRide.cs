using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMS.Utility
{
    public class VMRide
    {

       
            public int Ride_Id { get; set; }
            public int count { get; set; }
            public double Amount { get; set; }
            public string Type { get; set; }
            public string Emp_ID { get; set; }
            public int Invoice_ID { get; set; }
            public int SirkID { get; set; }
            public System.DateTime date { get; set; }
            public string Status { get; set; }
            public string EnterdBy { get; set; }
            public string ApprovedBy { get; set; }
            public int Warehouse_ID { get; set; }
            public System.DateTime LastUpdate { get; set; }

            public string comment { get; set; }
        }
    }
 