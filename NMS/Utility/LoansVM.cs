using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMS.Utility
{
    public class LoansVM
    {
        public int loan_id { get; set; }
        public Nullable<double> amount { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public string status { get; set; }
        public string Emp_ID { get; set; }
        public Nullable<int> acc_no { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public string Entered_By { get; set; }
    }
}