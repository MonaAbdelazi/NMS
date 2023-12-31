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
    
    public partial class Transactions
    {
        public long SystemId { get; set; }
        public string Status { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public string Entered_By { get; set; }
        public Nullable<long> Dr_Acc { get; set; }
        public Nullable<long> Cr_Acc { get; set; }
        public decimal Dr_Amt { get; set; }
        public decimal Cr_Amt { get; set; }
        public Nullable<int> CurrencyId { get; set; }
        public System.DateTime transactionDate { get; set; }
        public string TranReference { get; set; }
        public string Label { get; set; }
    
        public virtual Currency Currencies { get; set; }
        public virtual Tree Tree { get; set; }
        public virtual Tree Tree1 { get; set; }
    }
}
