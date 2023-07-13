using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NMS.Utility
{
     public   class VMinstallment
    {
        public string CusName { get; set; }
         
        public double Paid { get; set; }
       
        public int Invoice_ID { get; set; }
        public decimal ResidualAmt { get; set; }
        public decimal Amount { get; set; }
        public int ResidualIns { get; set; }
        public int Warehouse_ID { get; set; }
        public int No_Of_Inst { get; set; }
        
        public int numPaidinst { get; set; }
       
        public DateTime paiddate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }



        
            public string Status { get; set; }
        public string Comment { get; set; }
        public int Inst_ID { get; set; }




    }
}