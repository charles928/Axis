using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axis.Controllers
{
    public class MastersController : Controller
    {
        private AxisEntities db = new AxisEntities();

        // GET: Masters
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

                List<spGet_MastersMain_Result> model = db.spGet_MastersMain().ToList();
                return View(model);
            }
        }
        public ActionResult EventPage(String Source, Int32 Event_ID)
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

                List<spGet_EventInfo_Result> B = db.spGet_EventInfo(Event_ID).ToList();
                ViewData["Event_ID"] = Event_ID;
                DateTime CurrentDT = DateTime.ParseExact(B[0].Event_Date.Substring(0, 8), "MM/dd/yy", CultureInfo.InvariantCulture);
                String DOW = CurrentDT.DayOfWeek.ToString();
                ViewData["EventName"] = B[0].Event_Name;
                ViewData["ShortEventName"] = B[0].Event_Name.Replace("Golf Tournament: Practice Round", DOW);
                ViewData["EventDate"] = DOW + ' ' + B[0].Event_Date;
                ViewData["DUE"] = B[0].DaysUntilEvent;
                ViewData["VenueName"] = B[0].Venue_Name;
                ViewData["VenueCity"] = B[0].Venue_City;
                ViewData["Event_ID"] = Event_ID;

                if (B[0].Event_Name.Contains("Four Day"))
                    ViewData["FourDay"] = true;
                else
                    ViewData["FourDay"] = false;

                List<spGet_ItemsForEventSold_Result> C = db.spGet_ItemsForEventSold(Event_ID, 0).ToList();
                ViewData["ItemsSold"] = C;
                Int32 InvoicesSoldCount = C.Count();
                Int32 InvoicesSoldQty = Convert.ToInt32(C.Sum(x => x.Qty));
                ViewData["InvoicesSold"] = String.Format("{0:N0}", InvoicesSoldCount) + " Invoices Sold";
                ViewData["InvoicesSoldQty"] = String.Format("{0:N0}", InvoicesSoldQty);

                List<spGet_PresalesForEvent2_Result> D = db.spGet_PresalesForEvent2(Event_ID).ToList();
                ViewData["PresaleItems"] = D;
                Int32 PresaleInvoices = D.Count();
                Int32 PresaleQty = Convert.ToInt32(D.Sum(x => x.Qty));
                ViewData["PresaleInvoices"] = String.Format("{0:N0}", PresaleInvoices) + " Pending Preorders";
                ViewData["PresaleQty"] = String.Format("{0:N0}", PresaleQty);

                List<spGet_ItemsForMasters_Result> E = db.spGet_ItemsForMasters(Event_ID).ToList();
                ViewData["Available"] = E;
                Int32 AvailableItems = E.Count();
                Int32 AvailableQty = Convert.ToInt32(E.Sum(x => x.Qty));
                Int32 InHand = Convert.ToInt32(E.Sum(x => x.InHand));
                ViewData["AvailabelItems"] = String.Format("{0:N0}", AvailableItems) + " Listings";
                ViewData["AvailableQty"] = String.Format("{0:N0}", AvailableQty);
                ViewData["InHand"] = String.Format("{0:N0}", InHand);
                ViewData["NotInHand"] = String.Format("{0:N0}", AvailableQty - InHand);
                List<spGet_PayType_Result> PT = db.spGet_PayType().ToList();
                List<spGet_StateList_Result> ST = db.spGet_StateList(217).ToList();
                List<spGet_CountryList_Result> CO = db.spGet_CountryList().ToList();
                List<spGet_CCType_Result> CCT = db.spGet_CCType().ToList();
                List<spGet_CCMonth_Result> CCM = db.spGet_CCMonth().ToList();
                List<spGet_CCYear_Result> CCY = db.spGet_CCYear().ToList();

                var PayTypeList = new SelectList(PT, "payment_type_id", "payment_type_desc").ToList();
                var StateList = new SelectList(ST, "state_id", "state_name").ToList();
                var CountryList = new SelectList(CO, "country_id", "country_name").ToList();
                var CCTList = new SelectList(CCT, "CC_Type", "Desc").ToList();
                var CCMList = new SelectList(CCM, "CCM_ID", "Desc").ToList();
                var CCYList = new SelectList(CCY, "CCY_ID", "Desc").ToList();

                ViewData["PayTypeList"] = PayTypeList;
                ViewData["StateList"] = StateList;
                ViewData["CountryList"] = CountryList;
                ViewData["CCTList"] = CCTList;
                ViewData["CCMList"] = CCMList;
                ViewData["CCYList"] = CCYList;

                return View();
            }
        }
        public ActionResult UpdateNotes(Int32 CTG_ID)
        {
            String NewNotes = Request.Form["NewNotes"];
            List<spMasters_UpdatePresaleNotes_Result> A = db.spMasters_UpdatePresaleNotes(CTG_ID, NewNotes).ToList();
            return Content(A[0].ShortNote);
        }
        public ActionResult AddPayment()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            String sInvoice_ID = Request.Form["Invoice_ID"];
            String sPay_Type_ID = Request.Form["PayType_ID"];
            String sPay_Amt = Request.Form["PayAmt"];
            String sPay_Note = Request.Form["PayNote"];
            String sAuthCode = Request.Form["AuthCode"];
            Int32 Invoice_ID = Convert.ToInt32(sInvoice_ID);
            Int32 PayType_ID = Convert.ToInt32(sPay_Type_ID);
            Decimal Amt = Convert.ToDecimal(sPay_Amt);

            db.spAdd_Invoice_Payment(Login_ID, Invoice_ID, PayType_ID, Amt, sPay_Note, sAuthCode);

            return Content("success");

        }
        public ActionResult MakeInvoice()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            String ItemPrice = Request.Form["ItemPrice"];
            String sCustomer_ID = Request.Form["Customer_ID"];
            String CustomerType = Request.Form["CustomerType"];
            String Company = Request.Form["Company"];
            String First = Request.Form["First"];
            String Last = Request.Form["Last"];
            String Address1 = Request.Form["Address1"];
            String Address2 = Request.Form["Address2"];
            String City = Request.Form["City"];
            String sState_ID = Request.Form["State_ID"];
            String Zip = Request.Form["Zip"];
            String sCountry_ID = Request.Form["Country_ID"];
            String Email = Request.Form["Email"];
            String Phone = Request.Form["Phone"];
            String InvoiceNotes = Request.Form["InvoiceNotes"];
            String sShippingType_ID = Request.Form["ShippingType_ID"];
            String sShippingAmt = Request.Form["ShippingAmt"];
            String sTaxCollected = Request.Form["TaxCollected"];
            String sInvoiceTotal = Request.Form["InvoiceTotal"];
            String sInvoiceDue = Request.Form["InvoiceDue"];

            Int32 State_ID = Convert.ToInt32(sState_ID);
            Int32 Country_ID = Convert.ToInt32(sCountry_ID);
            Int32 ShippingType_ID = Convert.ToInt32(sShippingType_ID);
            Decimal ShippingAmt = Convert.ToDecimal(sShippingAmt);
            Decimal TaxCollected = Convert.ToDecimal(sTaxCollected);
            Decimal InvoiceTotal = Convert.ToDecimal(sInvoiceTotal);
            Decimal InvoiceDue = Convert.ToDecimal(sInvoiceDue);
            Int32? Client_ID = null;
            Int32? Client_Broker_ID = null;
            if (CustomerType == "Broker")
                Client_Broker_ID = Convert.ToInt32(sCustomer_ID);
            else
                Client_ID = Convert.ToInt32(sCustomer_ID);

            List<spBadge_Convert_Invoice_Result> A = db.spBadge_Convert_Invoice(Login_ID, ItemPrice, Client_ID, Client_Broker_ID, Company, First, Last, Address1, Address2, City, State_ID, Zip, Country_ID, Email, Phone, InvoiceNotes, ShippingType_ID, InvoiceTotal, InvoiceDue, ShippingAmt, TaxCollected).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ScanBadgeToSales(String Badge, Int32 Event_ID)
        {
            List<spBadge_AddForSale_Result> A = db.spBadge_AddForSale(Event_ID, Badge).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ScanIn(String Badge)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            List<spBadge_ScanIn_Result> A = db.spBadge_ScanIn(Login_ID, Badge).ToList();
            return Content(A[0].Result);
        }
        public ActionResult FillSeat(Int32 CT_ID, String Badge)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            List<spBadge_AssignSeat_Result> A = db.spBadge_AssignSeat(Login_ID, CT_ID, Badge).ToList();
            return Content(A[0].Result);
        }
        public ActionResult BadgeTotals(Boolean Th, Boolean Fr, Boolean Sa, Boolean Su, Boolean Fd)
        {
            List<spBadge_List_Result> A = db.spBadge_List(Th, Fr, Sa, Su, Fd).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FillList(Int32 CTG_ID, Int32 Invoice_ID)
        {
            List<spBadge_NeededForEventIvoice_Result> A = db.spBadge_NeededForEventIvoice(CTG_ID, Invoice_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SplitBadge(Int32 TG_ID)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            db.spBadge_Split(Login_ID, TG_ID);
            return Content("Success");
        }
        public ActionResult UpdateBadge(Int32 Ticket_ID, String Value)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            List<spBadge_Create_Result> A = db.spBadge_Create(Value, Ticket_ID, Login_ID).ToList();
            return Content(A[0].Return);
            //return Content("Green");
        }
        public ActionResult BadgeHistory(String Badge)
        {
            List<spBadge_History_Result> A = db.spBadge_History(Badge).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBadges(Int32 TicketGroup_ID)
        {
            List<spBadges_ByTicketGroup_Result> A = db.spBadges_ByTicketGroup(TicketGroup_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadAvailable(Int32 Event_ID)
        {
            List<spGet_ItemsForMasters_Result> model = db.spGet_ItemsForMasters(Event_ID).ToList();
            Int32 AvaQty = Convert.ToInt32(model.Sum(x => x.Qty));
            ViewData["AvaQty"] = AvaQty;
            ViewData["Available"] = model;
            return PartialView("AvailableInventory");
        }
        public ActionResult GenerateReport(Int32 Event_ID, String Name)
        {
            ExcelPackage pck = new ExcelPackage();
            String Worksheet = Name;
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Worksheet);
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet    
            ExcelWorksheetView wv = ws.View;
            wv.ZoomScale = 100;
            DataTable dt = new DataTable(); // Read records from database here
            dt.Columns.Add("Vendor", typeof(string));
            dt.Columns.Add("Badge", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            DataColumn dc = dt.Columns.Add("Invoice", typeof(Int32));
            dc.AllowDBNull = true;
            dt.Columns.Add("Customer", typeof(string));

            List<spBadge_Export_Result> model = db.spBadge_Export(Event_ID).ToList();
            Int32 Count = model.Count() + 1;
            foreach (var item in model)
            {
                dt.Rows.Add(item.Vendor, item.Badge, item.Status, item.Invoice, item.Customer);
            }
            Int32 plustwo = Count + 1;
            ws.Cells["A1"].LoadFromDataTable(dt, true);
            ws.View.FreezePanes(2, 1);
            ws.Cells.AutoFitColumns();
            return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Name + ".xlsx");
        }
    }
}