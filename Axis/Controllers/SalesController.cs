using Paymentech;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axis.Controllers
{
    public class SalesController : Controller
    {
        private AxisEntities db = new AxisEntities();
        // GET: Sales
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

                DateTime Start = Convert.ToDateTime(Session["StartDate"]);
                DateTime End = Convert.ToDateTime(Session["EndDate"]);

                List<spGet_Categories_Result> CL = db.spGet_Categories().ToList();
                List<spGet_HeadlinersList_Result> HL = db.spGet_HeadlinersList(Start, End, 0).ToList();
                List<spGet_AssignedOptions_Result> OL = db.spGet_AssignedOptions().ToList();
                List<spGet_Shipping_Result> SM = db.spGet_Shipping().ToList();
                List<spGet_PayType_Result> PT = db.spGet_PayType().ToList();
                List<spGet_StateList_Result> ST = db.spGet_StateList(217).ToList();
                List<spGet_CountryList_Result> CO = db.spGet_CountryList().ToList();
                List<spGet_CCType_Result> CCT = db.spGet_CCType().ToList();
                List<spGet_CCMonth_Result> CCM = db.spGet_CCMonth().ToList();
                List<spGet_CCYear_Result> CCY = db.spGet_CCYear().ToList();

                var CatList = new SelectList(CL, "Cat_ID", "Cat_Desc").ToList();
                var HeadlinerList = new SelectList(HL, "Headliner_ID", "Event_Headliner").ToList();
                var OptionList = new SelectList(OL, "TN_ID", "text").ToList();
                var ShippingList = new SelectList(SM, "Shipping_ID", "Type").ToList();
                var PayTypeList = new SelectList(PT, "payment_type_id", "payment_type_desc").ToList();
                var StateList = new SelectList(ST, "state_id", "state_name").ToList();
                var CountryList = new SelectList(CO, "country_id", "country_name").ToList();
                var CCTList = new SelectList(CCT, "CC_Type", "Desc").ToList();
                var CCMList = new SelectList(CCM, "CCM_ID", "Desc").ToList();
                var CCYList = new SelectList(CCY, "CCY_ID", "Desc").ToList();

                ViewData["CatList"] = CatList;
                ViewData["HeadlinerList"] = HeadlinerList;
                ViewData["OptionList"] = OptionList;
                ViewData["ShippingList"] = ShippingList;
                ViewData["PayTypeList"] = PayTypeList;
                ViewData["StateList"] = StateList;
                ViewData["CountryList"] = CountryList;
                ViewData["CCTList"] = CCTList;
                ViewData["CCMList"] = CCMList;
                ViewData["CCYList"] = CCYList;

                return View();
            }
        }
        public ActionResult ChargeCard()
        {
            Int32 Invoice_ID = Convert.ToInt32(Request.Form["Invoice_ID"]);
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            Int32 Cust_ID = Convert.ToInt32(Request.Form["Customer_ID"]);
            String CardType = Request.Form["CardType"];
            String CardNumber = Request.Form["CardNumber"];
            Decimal PayAmount = Convert.ToDecimal(Request.Form["PayAmount"]);
            String LastFour = CardNumber.Substring(CardNumber.Length - 4, 4);
            String ExpMonth = Request.Form["ExpMonth"];
            String ExpYear = Request.Form["ExpYear"];
            String Name = Request.Form["Name"];
            String Address = Request.Form["Address"];
            String City = Request.Form["City"];
            String State = Request.Form["State"];
            String Zip = Request.Form["Zip"];
            Int32 Country_ID = Convert.ToInt32(Request.Form["Country_ID"]);
            List<spGet_CountryCode_Result> A = db.spGet_CountryCode(Country_ID).ToList();
            String CountryCode = A[0].country_code;
            String CVV = Request.Form["CVV"];
            List<spAdd_CC_Charge2_Result> B = db.spAdd_CC_Charge2(CardType, LastFour, PayAmount, ExpMonth, ExpYear, Name, Address, City, State, Zip, Country_ID, CountryCode).ToList();
            Int32 Order_ID = Convert.ToInt32(B[0].Charge_ID);

            String Message ="";
            try
            {
                Paymentech.Response response; //Declare a response
                Transaction transaction = new Transaction(RequestType.NEW_ORDER_TRANSACTION); //Create an authorize transaction
                transaction["OrbitalConnectionUsername"] = "TICKETCIT2542176073";
                transaction["OrbitalConnectionPassword"] = "Qn67CfDHGLSY852Qqj6";
                transaction["IndustryType"] = "EC";
                transaction["MessageType"] = "AC";
                transaction["MerchantID"] = "800000033597";
                transaction["BIN"] = "000002";
                transaction["TraceNumber"] = Order_ID.ToString();
                transaction["AccountNum"] = CardNumber;
                transaction["OrderID"] = Order_ID.ToString();
                Int32 Amount = Convert.ToInt32(PayAmount * 100);
                transaction["Amount"] = Amount.ToString();
                transaction["Exp"] = ExpMonth + ExpYear;
                String AVSName = Name;
                if (AVSName.Length > 30)
                    AVSName = AVSName.Substring(0, 30);
                transaction["AVSname"] = AVSName;
                transaction["AVSaddress1"] = Address;
                transaction["AVScity"] = City;
                transaction["AVSstate"] = State;
                transaction["AVSzip"] = Zip;
                transaction["AVScountryCode"] = CountryCode;
                transaction["CardSecVal"] = CVV;
                if (CardType == "VISA" || CardType == "DISC")
                    transaction["CardSecValInd"] = "1";
                transaction["CustomerRefNum"] = Cust_ID.ToString();
                //                                            transaction["Comments"] = "Axis Sale created by " + UserName;
                //                                            transaction["ShippingRef"] = ShippingDescription;
                response = transaction.Process();
                if (response.Approved != true)
                {
                    Message = response.Message;
                    db.spUpdate_CC_Charge(Order_ID, Message, "", "", "", "");
                } else {
                    String vTxRefNum = response.TxRefNum;
                    String vAuthCode = response.AuthCode;
                    String AVSRespCode = response.AVSResponseCode;
                    String CVV2RespCode = response.CVV2ResponseCode;
                    Message = "Axis " + CardType + " ****" + LastFour + " " + ExpMonth + "/" + ExpYear + " " + Name + " Auth:" + vAuthCode + " " + AVSRespCode ;
                    db.spUpdate_CC_Charge(Order_ID, response.Message, vTxRefNum, vAuthCode, AVSRespCode, CVV2RespCode);
                }
            }
            catch (Exception e)
            {
                Message = e.ToString();
            }
            return Content(Message);
        }
        public ActionResult GetEventInfo(Int32 Event_ID)
        {
            List<spGet_EventInfo_Result> B = db.spGet_EventInfo(Event_ID).ToList();
            EventInfo data = new EventInfo();
            data.EventName = B[0].Event_Name;
            DateTime CurrentDT = DateTime.ParseExact(B[0].Event_Date.Substring(0, 8), "MM/dd/yy", CultureInfo.InvariantCulture);
            String DOW = CurrentDT.DayOfWeek.ToString();
            data.EventDate = DOW + ' ' + B[0].Event_Date;
            data.DUE = B[0].DaysUntilEvent.ToString() + " Days Until Event";
            data.VenueName = B[0].Venue_Name;
            data.VenueCity = B[0].Venue_City;
            data.Taxable = Convert.ToBoolean(B[0].Taxable);
            String wDate = B[0].Event_Date.Substring(0, 8);
            List<spWeather_GetForecast_Result> W = db.spWeather_GetForecast(B[0].Venue_City, Convert.ToDateTime(wDate)).ToList();
            if (W.Count() > 0)
            {
                data.Low = W[0].Low;
                data.High = W[0].High;
                data.Description = W[0].Description;
                data.Wind = W[0].Wind;
                data.Rain = W[0].Rain;
                data.Chance = W[0].Chance;
                data.Icons = W[0].Icons;
            }
            List<spRanking_GetEventRankings_Result> R = db.spRanking_GetEventRankings(Event_ID).ToList();
            data.Rankings = R[0].Rankings;

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetItemsForEvent(Int32 Event_ID)
        {
            List<spGet_ItemsForEventAvailable_Result> A = db.spGet_ItemsForEventAvailable(Event_ID, 0).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CustomerSearch()
        {
            String Company = Request.Form["Company"];
            String First = Request.Form["First"];
            String Last = Request.Form["Last"];
            String Address1 = Request.Form["Address1"];
            String Address2 = Request.Form["Address2"];
            String City = Request.Form["City"];
            String Zip = Request.Form["Zip"];
            String Email = Request.Form["Email"];
            String Phone = Request.Form["Phone"];
            List<spGet_CustomerSearch_Result> A = db.spGet_CustomerSearch(Company, First, Last, Address1, Address2, City, Zip, Email, Phone).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStateList(Int32 Country_ID)
        {
            List<spGet_StateList_Result> ST = db.spGet_StateList(Country_ID).ToList();
            return Json(ST, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateCustomer()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            String sCustomer_ID = Request.Form["Customer_ID"];
            String CustomerType = Request.Form["CustomerType"];
            String Company = Request.Form["Company"];
            String First = Request.Form["First"];
            String Last = Request.Form["Last"];
            String Address1 = Request.Form["Address1"];
            String Address2 = Request.Form["Address2"];
            String City = Request.Form["City"];
            String sState_ID = Request.Form["State"];
            String Zip = Request.Form["Zip"];
            String sCountry_Id = Request.Form["Country"];
            String Email = Request.Form["Email"];
            String Phone = Request.Form["Phone"];
            String sTaxExempt = Request.Form["TaxExempt"];

            Int32 Customer_ID = Convert.ToInt32(sCustomer_ID);
            Int32 State_ID = Convert.ToInt32(sState_ID);
            Int32 Country_ID = Convert.ToInt32(sCountry_Id);
            Boolean TaxExempt = Convert.ToBoolean(sTaxExempt);

            List<spUpdate_Customer_Result> A = db.spUpdate_Customer(Login_ID, Customer_ID, CustomerType, Company, First, Last, Address1, Address2, City, State_ID, Zip, Country_ID, Email, Phone, TaxExempt).ToList();
            
            return Content(A[0].Customer_ID.ToString());
        }
        public ActionResult HoldSeats()
        {
            String sCustomer_ID = Request.Form["Customer_ID"];
            String CustomerType = Request.Form["CustomerType"];
            String ExternalNotes = Request.Form["ExternalNotes"];
            String sShippingType_ID = Request.Form["ShippingType_ID"];
            String sItems = Request.Form["Items"];

            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            Int32? Client_ID = null;
            Int32? Client_Broker_ID = null;

            if (CustomerType == "Client")
                Client_ID = Convert.ToInt32(sCustomer_ID);
            else
                Client_Broker_ID = Convert.ToInt32(sCustomer_ID);
            Int32 ShippingType_ID = Convert.ToInt32(sShippingType_ID);
            String InternalNotes = DateTime.Now.ToString();
            String Notes = "Axis Invoice";
            Int32 Minutes = 15;
            String ReturnMessage = "";
            int?[] TransArray  = new int?[] { 0 };
            String[] ItemArray = sItems.Split('~');
            foreach (String Item in ItemArray)
            {
                if(Item.Length > 0)
                {
                    String[] EachItem = Item.Split('|');
                    Int32 Listing_ID = Convert.ToInt32(EachItem[0]);
                    Int32 Qty = Convert.ToInt32(EachItem[1]);
                    Decimal SalePrice = Convert.ToDecimal(EachItem[2]);

                    List<spHold_Create2_Result> A = db.spHold_Create2(Login_ID, Listing_ID, Qty, SalePrice, Client_ID, Client_Broker_ID, ShippingType_ID, ExternalNotes, InternalNotes, Notes, Minutes).ToList();

                    if (A[0].Message != "Hold created successfully.")
                    {
                        ReturnMessage = A[0].Message;
                        break;
                    }
                    else
                        TransArray = new int?[] { A[0].Transaction_ID };
                }
            }
            if (ReturnMessage.Length > 0)
            {
                foreach (int? Item in TransArray)
                {
                    if (Item > 0)
                        db.spHold_Release(Item, Login_ID).ToList();
                }
            }
            return Content(ReturnMessage);
        }
        public ActionResult MakeInvoice()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
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

            List<spConvert_Invoice_Result> A = db.spConvert_Invoice(Login_ID, Client_ID, Client_Broker_ID, Company, First, Last, Address1, Address2, City, State_ID, Zip, Country_ID, Email, Phone, InvoiceNotes, ShippingType_ID, InvoiceTotal, InvoiceDue, ShippingAmt, TaxCollected).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
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
    }
}