using GenCode128;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Web.UI.HtmlControls;
using System.IO;
using Microsoft.Exchange.WebServices.Data;
using System.Net;

namespace Axis.Controllers
{
    public class InvoiceController : Controller
    {
        private AxisEntities db = new AxisEntities();
        // GET: Invoice
        public ActionResult Index()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            if (Login_ID == 0)
            {
                return Redirect("../Home/Index");
            }
            else
            {
                List<spUser_GetLogin_Result> A = db.spUser_GetLogin(Login_ID).ToList();
                ViewData["Name"] = A[0].FullName;
                ViewData["Photo"] = @"/Images/" + A[0].Photo;
                DateTime Start = DateTime.Now.Date;
                DateTime End = Start;
                Session["Status"] = 0;
                Session["StartDate2"] = Start.ToString("MM/dd/yyyy");
                Session["EndDate2"] = End.ToString("MM/dd/yyyy");
                Session["Type"] = "All";

                List<spGET_ShippingMethod_Result> SM = db.spGET_ShippingMethod().ToList();
                var ShippingMethod = new SelectList(SM, "shipping_account_delivery_method_id", "shipping_account_delivery_method_desc").ToList();
                ViewData["ShippingMethod"] = ShippingMethod;

                return View();
            }
        }
        public ActionResult UpdateTracking()
        {
            String Type = Request.Form["Type"];
            Int32 Invoice_ID = Convert.ToInt32(Request.Form["Invoice"]);
            String Tracking = Request.Form["Tracking"];
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            List<spUpdate_Tracking_Result> A = db.spUpdate_Tracking(Type, Invoice_ID, Tracking, Login_ID).ToList();
            return Content(A[0].Return.ToString());
        }
        public ActionResult UpdateMethod()
        {
            Int32 Invoice_ID = Convert.ToInt32(Request.Form["Invoice"]);
            Int32 Method_ID = Convert.ToInt32(Request.Form["Method_ID"]);
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            db.spUpdate_Method(Invoice_ID, Method_ID, Login_ID);
            return Content("success");
        }
        public ActionResult UpdateValues()
        {
            Byte Status = Convert.ToByte(Request.Form["Status"]);
            DateTime Start = DateTime.Parse(Request.Form["Start"].ToString());
            DateTime End = DateTime.Parse(Request.Form["End"].ToString());
            String Type = Request.Form["Type"].ToString();
            Session["Status"] = Status;
            Session["StartDate2"] = Start;
            Session["EndDate2"] = End;
            Session["Type"] = Type;
            return Content("Success");
        }
        public ActionResult Search()
        {
            String Search = Request.Form["Search"].ToString();
            Byte Status = Convert.ToByte(Session["Status"]);
            String Type = Session["Type"].ToString();
            DateTime StartDate = DateTime.Parse(Session["StartDate2"].ToString());
            DateTime EndDate = DateTime.Parse(Session["EndDate2"].ToString());
            List<spSearch_InvoiceList2_Result> model = db.spSearch_InvoiceList2(Search, Status, Type, StartDate, EndDate).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmailRequest()
        {
            Int32 Invoice_ID = Convert.ToInt32(Request.Form["Invoice_ID"]);
            string Data = Request.Form["Data"];
            byte[] data = Convert.FromBase64String(Data);

            ExchangeService service = new ExchangeService();
            service.Credentials = new WebCredentials("charles@ticketcity.com", "FOAD123!");
            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
            EmailMessage email = new EmailMessage(service);
            string html = @"<html>
                     <head>
                     </head>
                     <body>
                        <img id=""1"" src=""cid:Invoice.png"">
                     </body>
                     </html>";
            email.Subject = "Invoice #" + Invoice_ID.ToString() + " from 360Access.com";
            email.Body = new MessageBody(BodyType.HTML, html);
            email.ToRecipients.Add(Request.Form["Address"]);
            FileAttachment attachment = email.Attachments.AddFileAttachment("Invoice.png", data);
            attachment.IsInline = true;
            attachment.ContentType = "PNG/Image";
            attachment.ContentId = "Invoice.png";
            email.SendAndSaveCopy();
            return Content("success");
        }
        public ActionResult GetInvoice(Int32 Invoice_ID, Boolean AutoPrint)
        {
            List<spGet_InvoiceBillTo_Result> B = db.spGet_InvoiceBillTo(Invoice_ID).ToList();
            ViewData["BillTo"] = B;
            ViewData["Rep"] = B[0].Rep;
            ViewData["RepPhone"] = B[0].RepPhone;
            List<spGet_InvoiceShipTo_Result> S = db.spGet_InvoiceShipTo(Invoice_ID).ToList();
            ViewData["ShipTo"] = S;
            ViewData["EmailAddress"] = B[0].client_broker_email;

            List<spGet_InvoiceInfo_Result> I = db.spGet_InvoiceInfo(Invoice_ID).ToList();
            ViewData["RepNotes"] = I[0].email_info;
            ViewData["Info"] = I;
            if (string.IsNullOrEmpty(I[0].Stamp))
                ViewData["StampDisplay"] = "none";
            else
                ViewData["StampDisplay"] = "block";
            ViewData["Stamp"] = I[0].Stamp;
            if (I[0].Stamp == "PAID")
                ViewData["StampLoc"] = "342px";
            else
                ViewData["StampLoc"] = "307px";
            ViewData["DisplayedNotes"] = I[0].displayed_notes;

            List<spGet_InvoicePayment_Result> P = db.spGet_InvoicePayment(Invoice_ID).ToList();
            ViewData["Payment"] = P;

            List<spGet_InvoiceItems_Result> model = db.spGet_InvoiceItems(Invoice_ID).ToList();
            ViewData["Items"] = model.Count();
            Int32 Qty = Convert.ToInt32(model.Sum(item => item.Qty));
            ViewData["Qty"] = Qty;
            Double Cost = Convert.ToDouble(model.Sum(item => item.Qty * item.Cost));
            Double Price = Convert.ToDouble(model.Sum(item => item.Qty * item.Price));
            ViewData["Price"] = String.Format("{0:c}", Price);
            ViewData["ShippingTotal"] = String.Format("{0:c}", I[0].invoice_total_shipping_cost);
            ViewData["TaxTotal"] = String.Format("{0:c}", I[0].invoice_total_taxes);
            ViewData["OtherExpensesTotal"] = String.Format("{0:c}", I[0].invoice_total_expense);
            ViewData["GrandTotal"] = String.Format("{0:c}", I[0].invoice_total);
            ViewData["BalanceDue"] = String.Format("{0:c}", I[0].invoice_balance_due);
            ViewData["DueDate"] = I[0].invoice_due_date;
            ViewData["Agent"] = I[0].Agent;
            ViewData["SalesDate"] = I[0].create_date;
            ViewData["Invoice"] = Invoice_ID;
            ViewData["DeliveryMethod"] = I[0].DeliveryMethod;
            ViewData["DisplayNotes"] = I[0].displayed_notes;
            ViewData["ExternalPO"] = I[0].external_PO;
            ViewData["TicketRequestID"] = I[0].ticket_request_id;
            ViewData["StatusDate"] = I[0].StatusDate;
            System.Drawing.Image myimg = Code128Rendering.MakeBarcodeImage(Invoice_ID.ToString(), int.Parse("1"), true);
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(myimg, typeof(byte[]));
            var base64 = Convert.ToBase64String(xByte);
            var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
            ViewData["Barcode"] = imgSrc;
            ViewData["AutoPrint"] = AutoPrint;
            return PartialView(model);
        }
        public ActionResult AutoPrint()
        {
            Int32 Invoice_ID = Convert.ToInt32(Request.Form["Invoice_ID"]);
            String Data = Request.Form["Data"];
            db.spAdd_AutoPrintInfo(Invoice_ID, Data);
            return Content("success");
        }
    }
}