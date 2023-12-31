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
    
    public partial class InItemsInvoice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InItemsInvoice()
        {
            this.Items = new HashSet<Item>();
            this.Ride = new HashSet<Ride>();
        }
    
        public int ID { get; set; }
        public System.DateTime EnteredDate { get; set; }
        public string Entered_By { get; set; }
        public string Approved_By { get; set; }
        public Nullable<System.DateTime> lastUpdated { get; set; }
        public string Status { get; set; }
        public int InSirkNo { get; set; }
        public Nullable<int> totalQun { get; set; }
        public Nullable<decimal> TotalAmt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Item> Items { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ride> Ride { get; set; }
    }
}
