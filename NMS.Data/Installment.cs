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
    
    public partial class Installment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Installment()
        {
            this.Payments = new HashSet<Payment>();
        }
    
        public int Inst_ID { get; set; }
        public Nullable<int> Cus_ID { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int No_Of_Inst { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> Last_Update { get; set; }
        public string Enterd_By { get; set; }
        public int Invoice_ID { get; set; }
        public Nullable<decimal> ResidualAmt { get; set; }
        public Nullable<int> ResidualIns { get; set; }
        public int Warehouse_ID { get; set; }
        public Nullable<decimal> Paid { get; set; }
        public Nullable<int> numPaidinst { get; set; }
        public Nullable<decimal> currntamount { get; set; }
        public Nullable<System.DateTime> paiddate { get; set; }
        public Nullable<int> currntinst { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment> Payments { get; set; }
    }
}