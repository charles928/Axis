//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Axis
{
    using System;
    
    public partial class spGet_Spec_Result
    {
        public int event_parent_category_id { get; set; }
        public string event_parent_category_desc { get; set; }
        public int event_child_category_id { get; set; }
        public string event_child_category_desc { get; set; }
        public int event_grandchild_category_id { get; set; }
        public string event_grandchild_category_desc { get; set; }
        public Nullable<int> Events { get; set; }
        public Nullable<int> UListings { get; set; }
        public Nullable<int> UTickets { get; set; }
        public Nullable<decimal> UCost { get; set; }
        public Nullable<decimal> UPrice { get; set; }
        public Nullable<decimal> UProfit { get; set; }
        public int SListings { get; set; }
        public int STickets { get; set; }
        public decimal SCost { get; set; }
        public decimal SPrice { get; set; }
        public Nullable<decimal> SProfit { get; set; }
        public Nullable<decimal> Priced { get; set; }
        public Nullable<decimal> Shared { get; set; }
        public Nullable<decimal> Sold { get; set; }
        public Nullable<int> ExpiredEvents { get; set; }
    }
}
