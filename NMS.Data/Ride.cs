//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NMS.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ride
    {
        public int Ride_Id { get; set; }
        public Nullable<int> count { get; set; }
        public Nullable<double> Amount { get; set; }
        public string Type { get; set; }
        public Nullable<int> Emp_ID { get; set; }
        public Nullable<int> Invoice_ID { get; set; }
        public Nullable<int> SirkID { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public string Status { get; set; }
        public string EnterdBy { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<int> Warehouse_ID { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public string comment { get; set; }
    
        public virtual Employess Employess { get; set; }
        public virtual InItemsInvoice InItemsInvoice { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual WareHouse WareHouse { get; set; }
    }
}
