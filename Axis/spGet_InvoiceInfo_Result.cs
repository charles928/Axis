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
    
    public partial class spGet_InvoiceInfo_Result
    {
        public int invoice_id { get; set; }
        public string Status { get; set; }
        public Nullable<decimal> invoice_total { get; set; }
        public Nullable<decimal> invoice_total_due { get; set; }
        public Nullable<decimal> invoice_total_expense { get; set; }
        public Nullable<decimal> invoice_total_shipping_cost { get; set; }
        public Nullable<decimal> invoice_total_taxes { get; set; }
        public Nullable<decimal> invoice_balance_due { get; set; }
        public string notes { get; set; }
        public string displayed_notes { get; set; }
        public string external_PO { get; set; }
        public Nullable<int> ticket_request_id { get; set; }
        public string shipping_tracking_number { get; set; }
        public string shipping_status { get; set; }
        public Nullable<System.DateTime> shipping_status_date { get; set; }
        public string shipping_notes { get; set; }
        public string Agent { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public string DeliveryMethod { get; set; }
        public Nullable<System.DateTime> invoice_due_date { get; set; }
        public string StatusDate { get; set; }
        public string Stamp { get; set; }
        public string email_info { get; set; }
    }
}