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
    
    public partial class spSeason_History_Result
    {
        public int purchase_order_id { get; set; }
        public int ticket_group_id { get; set; }
        public int event_id { get; set; }
        public string event_name { get; set; }
        public string section { get; set; }
        public string row { get; set; }
        public string Seats { get; set; }
        public Nullable<int> qty { get; set; }
        public string Agent { get; set; }
        public Nullable<int> Events { get; set; }
        public Nullable<int> Tickets { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public System.DateTime Split { get; set; }
    }
}