using Axis.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Axis.Controllers
{
    public class InventoryController : Controller
    {
        private AxisEntities db = new AxisEntities();
        // GET: Inventory
        public ActionResult GetEventsForGrandchild(Int32 Parent_ID, Int32 Child_ID, Int32 Grandchild_ID, Int32 Range)
        {
            List<spGet_EventsForGrandchild_Result> model = db.spGet_EventsForGrandchild(Parent_ID, Child_ID, Grandchild_ID, Range).ToList();
                return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateValues(String Type, Int32 NewValue)
        {
            if (Type == "Parent")
                Session["Parent_ID"] = NewValue;
            if (Type == "Child")
                Session["Child_ID"] = NewValue;
            if (Type == "Range")
                Session["Range"] = NewValue;
            if (Type == "Reset")
            {
                Session["Parent_ID"] = 0;
                Session["Child_ID"] = 0;
                Session["Grandchild_ID"] = 0;
                Session["Range"] = 0;
            }
            return Content("success");
        }
        public ActionResult Index2()
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

                Int32 iParent_ID;
                var crap = Session["Parent_ID"];
                if (crap == null)
                    iParent_ID = 0;
                else
                    iParent_ID = Convert.ToInt32(Session["Parent_ID"]);
                Session["Parent_ID"] = iParent_ID;

                Int32 iChild_ID;
                crap = Session["Child_ID"];
                if (crap == null)
                    iChild_ID = 0;
                else
                    iChild_ID = Convert.ToInt32(Session["Child_ID"]);
                Session["Child_ID"] = iChild_ID;

                Int32 iGrandchild_ID;
                crap = Session["Grandchild_ID"];
                if (crap == null)
                    iGrandchild_ID = 0;
                else
                    iGrandchild_ID = Convert.ToInt32(Session["Grandchild_ID"]);
                Session["Grandchild_ID"] = iGrandchild_ID;

                Int32 iRange;
                crap = Session["Range"];
                if (crap == null)
                    iRange = 0;
                else
                    iRange = Convert.ToInt32(Session["Range"]);
                Session["Range"] = iRange;

                List<spGet_Inventory_Result> model = db.spGet_Inventory(iParent_ID, iChild_ID, iGrandchild_ID, iRange).ToList();
                if (iParent_ID > 0)
                    ViewBag.Parent = model[0].event_parent_category_desc;
                if (iChild_ID > 0)
                    ViewBag.Child = model[0].event_child_category_desc;

                Int32 Events = Convert.ToInt32(model.Sum(item => item.Events));
                Int32 UListings = Convert.ToInt32(model.Sum(item => item.UListings));
                Int32 SListings = Convert.ToInt32(model.Sum(item => item.SListings));
                Int32 UTickets = Convert.ToInt32(model.Sum(item => item.UTickets));
                Int32 STickets = Convert.ToInt32(model.Sum(item => item.STickets));
                Decimal UCost = Convert.ToDecimal(model.Sum(item => item.UCost));
                Decimal SCost = Convert.ToDecimal(model.Sum(item => item.SCost));
                Decimal UPrice = Convert.ToDecimal(model.Sum(item => item.UPrice));
                Decimal SPrice = Convert.ToDecimal(model.Sum(item => item.SPrice));
                Decimal UProfit = Convert.ToDecimal(model.Sum(item => item.UProfit));
                Decimal SProfit = Convert.ToDecimal(model.Sum(item => item.SProfit));

                ViewBag.TotalEvents = Events;
                ViewBag.TotalUListings = UListings;
                ViewBag.TotalSListings = SListings;
                ViewBag.TotalUTickets = UTickets;
                ViewBag.TotalSTickets = STickets;
                ViewBag.TotalUCost = Decimal.Round(UCost, 2);
                ViewBag.TotalSCost = Decimal.Round(SCost, 2);
                ViewBag.TotalUPrice = Decimal.Round(UPrice, 2);
                ViewBag.TotalSPrice = Decimal.Round(SPrice, 2);
                ViewBag.TotalUProfit = Decimal.Round(UProfit, 2);
                ViewBag.TotalSProfit = Decimal.Round(SProfit, 2);

                return View(model);
            }
        }
        public ActionResult Index(int? Event_ID, int? PO_ID)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            if (Login_ID == 0 || Event_ID == null)
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
                if (PO_ID > 0)
                    ViewData["EventName"] = B[0].Event_Name + "  (PO:" + PO_ID.ToString() + ")";
                else
                    ViewData["EventName"] = B[0].Event_Name;
                ViewData["PO_ID"] = PO_ID;
                DateTime CurrentDT = DateTime.ParseExact(B[0].Event_Date.Substring(0,8), "MM/dd/yy", CultureInfo.InvariantCulture);
                String DOW = CurrentDT.DayOfWeek.ToString();

                Session["CMin"] = B[0].ChartMin;
                Session["CMax"] = CurrentDT;

                ViewData["EventDate"] = DOW + ' ' + B[0].Event_Date;
                ViewData["DUE"] = B[0].DaysUntilEvent;
                ViewData["VenueName"] = B[0].Venue_Name;
                ViewData["VenueCity"] = B[0].Venue_City;
                ViewData["AvailableQty"] = B[0].Available_Qty;
                ViewData["SoldQty"] = B[0].Sold_Qty;

                ViewData["TotalCost"] = B[0].Total_Cost;
                ViewData["LastSoldDate"] = (B[0].LastSoldDate == "Never") ? "Never" : B[0].LastSoldDate.Substring(0, 8);
                ViewData["LastPricedDate"] = B[0].LastPricedDate;
                ViewData["AvailableCost"] = String.Format("{0:c}", B[0].Available_Cost);
                ViewData["SoldPrice"] = String.Format("{0:c}", B[0].Sold_Price);
                ViewData["SoldCost"] = String.Format("{0:c}", B[0].Sold_Cost);
                ViewData["SoldMargin"] = B[0].SoldMargin;
                ViewData["SoldProfit"] = String.Format("{0:c}", B[0].SoldProfit);
                ViewData["SHEvent_ID"] = B[0].SH_Event_ID;

                string[] Login_IDs = B[0].FollowingID.Split('|');
                if (Login_IDs.Contains(Session["Login_ID"].ToString()))
                {
                    ViewData["FollowText"] = "UNFOLLOW";
                    ViewData["FollowIcon"] = "star";
                }
                else
                {
                    ViewData["FollowText"] = "FOLLOW";
                    ViewData["FollowIcon"] = "star_border";
                }
                ViewData["FollowingText"] = B[0].Following;

                if (B[0].Assigned.Length > 0)
                    ViewData["AssignText"] = "Assigned to " + B[0].Assigned;

                List<spGet_ItemsForEventSold_Result> C = db.spGet_ItemsForEventSold(Event_ID, PO_ID).ToList();
                ViewData["ItemsSold"] = C;
                Int32 InvoicesSoldCount = C.Count();
                Int32 InvoicesSoldQty = Convert.ToInt32(C.Sum(x => x.Qty));
                Decimal InvoicesSoldCost = Convert.ToDecimal(C.Sum(x => x.Cost * x.Qty));
                Decimal InvoicesSoldPrice = Convert.ToDecimal(C.Sum(x => x.Price * x.Qty));
                ViewData["InvoicesSold"] = String.Format("{0:N0}", InvoicesSoldCount) + " Invoices Sold";
                ViewData["InvoicesSoldQty"] = String.Format("{0:N0}", InvoicesSoldQty);
                ViewData["InvoicesSoldCost"] = String.Format("{0:c}", InvoicesSoldCost);
                ViewData["InvoicesSoldPrice"] = String.Format("{0:c}", InvoicesSoldPrice);
                ViewData["InvoicesSoldProfit"] = String.Format("{0:c}", InvoicesSoldPrice - InvoicesSoldCost);

                List<spGet_PresalesForEvent2_Result> D = db.spGet_PresalesForEvent2(Event_ID).ToList();
                Int32 PresaleInvoices = D.Count();
                Int32 PresaleQty = Convert.ToInt32(D.Sum(x => x.Qty));
                Decimal PresalePrice = Convert.ToDecimal(D.Sum(x => x.Qty * x.Price));
                ViewData["PresaleInvoices"] = String.Format("{0:N0}", PresaleInvoices) + " Pending Preorders";
                ViewData["PresaleQty"] = String.Format("{0:N0}", PresaleQty);
                ViewData["PresalePrice"] = String.Format("{0:c}", PresalePrice);
                ViewData["PresaleItems"] = D;
                List<spGet_ItemsForEventAvailable_Result> model = db.spGet_ItemsForEventAvailable(Event_ID, PO_ID).ToList();

                Int32 AvaQty = Convert.ToInt32(model.Sum(x => x.Qty));
                Int32 SpecQty = Convert.ToInt32(model.Sum(x => Convert.ToBoolean(x.Spec) ? x.Qty: 0));
                if (SpecQty > 0)
                    ViewData["QtyTotal"] = "<a href='' style='color:yellow;' title='Spec Qty " + SpecQty.ToString() + "'>" + AvaQty.ToString() + "</a>";
                else
                    ViewData["QtyTotal"] = AvaQty.ToString();

                Decimal AvaCost = Convert.ToDecimal(model.Sum(x => x.Cost * x.Qty));
                Decimal AvaPrice = Convert.ToDecimal(model.Sum(x => x.Price * x.Qty));
                ViewData["AvaQty"] = AvaQty;
                ViewData["AvaCost"] = String.Format("{0:c}", AvaCost);
                ViewData["AvaPrice"] = String.Format("{0:c}", AvaPrice);

                ViewData["Event_ID"] = Event_ID;
                String wDate = B[0].Event_Date.Substring(0,8);
                List<spWeather_GetForecast_Result> W = db.spWeather_GetForecast(B[0].Venue_City, Convert.ToDateTime(wDate)).ToList();
                if (W.Count() > 0)
                {
                    ViewData["Low"] = W[0].Low;
                    ViewData["High"] = W[0].High;
                    ViewData["Description"] = W[0].Description;
                    ViewData["Wind"] = W[0].Wind;
                    ViewData["Rain"] = W[0].Rain;
                    ViewData["Chance"] = W[0].Chance;
                    ViewData["Icons"] = W[0].Icons;
                }
                List<spRanking_GetEventRankings_Result> R = db.spRanking_GetEventRankings(Event_ID).ToList();
                ViewData["Ranking"] = R[0].Rankings;

                return View(model);
            }
        }
 
       public ActionResult JsonChart(Int32 Event_ID, Int32 Source_ID, Int32 Type_ID, String FilterType, String Filter)
        {
            DateTime CMin = DateTime.Parse(Session["CMin"].ToString());
            DateTime CMax = DateTime.Parse(Session["CMax"].ToString());
            if (FilterType == "Section" && Filter == "All Sections")
                Filter = "";
            if (FilterType == "Zone" && Filter == "All Zones")
                Filter = "";

            List<spChart_Gather_Result> model = db.spChart_Gather(Event_ID, Source_ID, Type_ID, FilterType, Filter, CMin, CMax).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadAvailable(Int32 Event_ID, Int32 PO_ID)
        {
            List<spGet_ItemsForEventAvailable_Result> model = db.spGet_ItemsForEventAvailable(Event_ID, PO_ID).ToList();
            Int32 AvaQty = Convert.ToInt32(model.Sum(x => x.Qty));
            Decimal AvaCost = Convert.ToDecimal(model.Sum(x => x.Cost * x.Qty));
            ViewData["AvaQty"] = AvaQty;
            ViewData["AvaCost"] = String.Format("{0:c}", AvaCost);
            ViewData["Available"] = model;
            return PartialView("AvailableInventory");
        }
        public ActionResult BulkUpdate()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            String Title = Request.Form["Title"];
            String Listing_IDs = Request.Form["Listing_IDs"];
            String Transaction_IDs = Request.Form["Transaction_IDs"];
            String DollarChange = Request.Form["DollarChange"];
            String PercentChange = Request.Form["PercentChange"];
            String NotesChange = Request.Form["NotesChange"];
            String HoldChange = Request.Form["HoldChange"];
            String GreenChange = Request.Form["GreenChange"];
            String RedChange = Request.Form["RedChange"];

            if (DollarChange != "")
            {
                Decimal Dollar = 0;
                Dollar = Convert.ToDecimal(DollarChange);
                db.spBulk_Dollar(Listing_IDs, Dollar, Login_ID, Title);
            }
            else
            {
            if (PercentChange != "")
                {
                    Int32 Percent = 0;
                    Percent = Convert.ToInt32(PercentChange);
                    db.spBulk_Percent(Listing_IDs, Percent, Login_ID, Title);
                }
            }
            if (HoldChange != "No Change")
            {
                db.spBulk_Hold(Listing_IDs, Transaction_IDs, HoldChange, Login_ID);
            }
            if (NotesChange != "")
            {
                db.spBulk_Notes(Listing_IDs, NotesChange, Login_ID);
            }
            if (GreenChange != "")
            {
                String[] substrings = GreenChange.Split(',');
                foreach (var substring in substrings)
                {
                    if (substring != "")
                        db.spBulk_Shares(Listing_IDs, "RGB(255, 0, 0)", substring, Login_ID);
                }
            }
            if (RedChange != "")
            {
                String[] substrings = RedChange.Split(',');
                foreach (var substring in substrings)
                {
                    if (substring != "")
                        db.spBulk_Shares(Listing_IDs, "RGB(0, 128, 0)", substring, Login_ID);
                }
            }
            return Content("success");
        }
        public ActionResult SplitSeats()
        {
            Int32 Login_ID = Convert.ToInt32(Request.Form["Login_ID"]);
            Int32 Listing_ID = Convert.ToInt32(Request.Form["Listing_ID"]);
            Int32 QtyToSplit = Convert.ToInt32(Request.Form["QtyToSplit"]);
            //List<spCreate_Split_Result> A = db.spCreate_Split(Listing_ID, QtyToSplit, Login_ID).ToList();
            //return Json(A, JsonRequestBehavior.AllowGet);
            return Content("disabled");
        }
        public ActionResult ReleaseHold (Int32 Transaction_ID)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            List<spHold_Release_Result> A = db.spHold_Release(Transaction_ID, Login_ID).ToList();
            return Content("success");
        }
        public ActionResult HoldSeats(Int32 Listing_ID)
        {
            Int32 Login_ID = Convert.ToInt32(Request.Form["Login_ID"]);
            Int32 HoldTickets = Convert.ToInt32(Request.Form["HoldTickets"]);
            Int32 HoldTime = Convert.ToInt32(Request.Form["HoldTime"]);
            String HoldNotes = Request.Form["HoldNotes"];
            List<spHold_Create_Result> A = db.spHold_Create(Login_ID, Listing_ID, HoldTickets, HoldTime, HoldNotes).ToList();
            return Content(A[0].Message);
        }
        public ActionResult FollowEvent(Int32 Event_ID, String Type)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            List<spFollowEvent_Result> A = db.spFollowEvent(Login_ID, Event_ID, Type).ToList();
            return Content(A[0].Return);
        }
        public ActionResult GetAssignedList(Int32 Event_ID)
        {
            List<spGet_AssignedList_Result> A = db.spGet_AssignedList(Event_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateAssignment(Int32 Event_ID, Int32 Login_ID, Boolean NewValue)
        {
            List<spUpdate_Assignment_Result> A = db.spUpdate_Assignment(Event_ID, Login_ID, NewValue).ToList();
            return Content(A[0].Return);
        }
        public ActionResult GetSuggestedPriceMkt(Int32 Event_ID, Int32 Listing_ID, String Section, Decimal Price, String Exclude, Boolean Singles)
        {
            //List<spES_MinPriceBySection_Result> A = db2.spES_MinPriceBySection(Listing_ID, Price, Event_ID, Section, Exclude, Singles).ToList();
            return Content("Disabled");// Json(A, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetStubhubData(Int32 eventId, String Filter, String FilterID)
        {
            try
            {
                // Declare the list that will hold our finished product
                List<Stubhub.Listing> BigList = new List<Stubhub.Listing>();

                //GET CREDENTIALS
                List<spGet_SHCredintials_Result> x = db.spGet_SHCredintials().ToList();
                String ApplicationToken = x[0].AppToken;
                String ConsumerKey = x[0].Key;
                String ConsumerSecret = x[0].Secret;
                String BearerToken = x[0].BearerToken;
                String SellerID = x[0].SellerID;

                String request;
                Int32 NumOfRecords = 200;
                Int32 TotalRecords = 0;
                String file2;
                String URLFilter = "";
                if (FilterID.Length > 1)
                    URLFilter = "&" + Filter + "=" + FilterID;

                // loop if needed since stubhub limits # of returns to 200
                while (NumOfRecords == 200)
                {
                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                    request = "https://api.stubhub.com/search/inventory/v2?eventid=" + eventId.ToString() + "&sectionstats=false&start=" + TotalRecords.ToString() + "&rows=200" + URLFilter + "&pricingsummary=true&zonestats=false";
                    WebRequest webRequest2 = WebRequest.Create(request);
                    webRequest2.Method = "GET";
                    webRequest2.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                    webRequest2.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + BearerToken);
                    using (WebResponse response2 = webRequest2.GetResponse())
                    {
                        // Get the stream containing content returned by the server.
                        Stream dataStream3 = response2.GetResponseStream();
                        // Open the stream using a StreamReader for easy access.
                        StreamReader reader2 = new StreamReader(dataStream3);
                        // Read the content.
                        file2 = reader2.ReadToEnd();
                    }
                    webRequest2 = null;
                    var k = JsonConvert.DeserializeObject<Stubhub.RootObject>(file2);

                    if (k.totalListings > 0 && k.listing != null)
                    {
                        NumOfRecords = k.listing.Count;
                        TotalRecords += NumOfRecords;

                        // Add this set to the finish list
                        BigList.AddRange(k.listing);
                    }
                    else
                    {
                        TotalRecords = 0;
                        NumOfRecords = 0;
                    }
                }

                if (TotalRecords > 0)
                {
                    foreach (var item in BigList)
                    {
                        if (item.sellerOwnInd == 1) //(x.Any(q => q.listingId == item.listingId))
                        {//Mark Our Items (Local)
                            item.Local = true;
                            item.SH_Section = item.sectionName; //Stubhub sometimes changes the section name so this captures the "Stubhub section name"
                            int listingId = item.listingId;
                            //decimal Price = Convert.ToDecimal(item.currentPrice.amount);
                            decimal Price = Convert.ToDecimal(item.listingPrice.amount);
                            List<spGet_Listing_ID_Result> A = db.spGet_Listing_ID(listingId).ToList();
                            if (A.Count() > 0)
                                item.Listing_ID = Convert.ToInt32(A[0].Listing_ID);
                        }
                    }
                    // Get a distinct list of Section to Zone and add to our SH_SectionZone table
                    var seczone = (from bl in BigList select new { sectionId = bl.sectionId, section = bl.sectionName, zoneId = bl.zoneId, zone = bl.zoneName }).Distinct();
                    String SecZone = "";
                    foreach (var Item in seczone)
                    {
                        SecZone += Item.sectionId + "~" + Item.section + "~" + Item.zoneId + "~" + Item.zone + "|";
                    }
                    db.spUpdate_SectionZone(eventId, SecZone);
                }
                var jsonResult = Json(BigList, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
            catch (WebException webEx)
            {
                using (WebResponse response = webEx.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        return Json(text, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
        public ActionResult GetItemDetails(Int32 Listing_ID)
        {
            //            List<spGet_ListingDetails_Result> A = db.spGet_ListingDetails(Listing_ID).ToList();
            //            return Json(A, JsonRequestBehavior.AllowGet);

            return Content("pending");
        }
        public ActionResult UpdateDetail(Int32 Listing_ID)
        {
            String Source = Request.Form["Source"];
            String NewData = Request.Form["NewData"];
            if (Source == "Face")
            {
                NewData = NewData.Replace("$", "");
                NewData = NewData.Replace(",", "");
            }
            //db.spUpdate_Listing(Listing_ID, Source, NewData);            
            return Content("success");
        }
        public ActionResult UpdateShare(Int32 Listing_ID)
        {
            String Source = Request.Form["Source"];
            String Current = Request.Form["Current"];
            Int32 Login_ID = Convert.ToInt32(Request.Form["Login_ID"]);
            List<spUpdate_Share_Result> A = db.spUpdate_Share(Current, Listing_ID, Source, Login_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Green714(Int32 ListingID) //Green(Int32 ListingID) // not really all it's 714 (All is AllGoGreen)
        {
            Int32 Login_ID = Convert.ToInt32(Request.Form["Login_ID"]);
            db.spUpdate_714("Green", ListingID, Login_ID);
            return Content("success");
        }
        public ActionResult Red714(Int32 ListingID) //AllRed(Int32 ListingID) // not really all it's 714 (All is AllGoRed)
        {
            Int32 Login_ID = Convert.ToInt32(Request.Form["Login_ID"]);
            db.spUpdate_714("Red", ListingID, Login_ID);
            return Content("success");
        }
        public ActionResult AllGoGreen(Int32 ListingID)
        {
            Int32 Login_ID = Convert.ToInt32(Request.Form["Login_ID"]);
            db.spUpdate_AllShares("Green", ListingID, Login_ID);
            return Content("success");
        }
        public ActionResult AllGoRed(Int32 ListingID)
        {
            Int32 Login_ID = Convert.ToInt32(Request.Form["Login_ID"]);
            db.spUpdate_AllShares("Red", ListingID, Login_ID);
            return Content("success");
        }
        public ActionResult Get714Shares(Int32 ListingID)
        {
            List<spGet_714Shares_Result> A = db.spGet_714Shares(ListingID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdatePrice(Int32 Listing_ID)
        {
            Decimal Current = Convert.ToDecimal(Request.Form["Current"]);
            String Title = Request.Form["Title"];
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            List<spUpdate_Price_Result> A = db.spUpdate_Price(Listing_ID, Current, Title, Login_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateSHPrice(Int32 Listing_ID)
        {
            Decimal Current = Convert.ToDecimal(Request.Form["Current"]);
            String Title = Request.Form["Title"];
            Int32 Login_ID = Convert.ToInt32(Request.Form["Login_ID"]);
            List<spUpdate_SHPrice_Result> A = db.spUpdate_SHPrice(Listing_ID, Current, Title, Login_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetActivityList(Int32 Event_ID)
        {
            List<spGet_Activity_Result> A = db.spGet_Activity(Event_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEventSearch()
        {
            try
            {
                Int32 Event_ID = Convert.ToInt32(Request.Form["Event_ID"]);
                String EventName = Request.Form["EventName"];
                String CityState = Request.Form["CityState"];
                String State = "";
                String request;
                String file2;

                List<spGet_EventInfo_Result> I = db.spGet_EventInfo(Event_ID).ToList();
                String Performer = I[0].Event_Headliner;
                String Venue = I[0].Venue_Name;
                String Parking = I[0].Parking.ToString();
                if (CityState.IndexOf(",") > 0)
                {
                    State = CityState.Substring(CityState.IndexOf(",") + 1, 3).Trim();
                    request = "https://api.stubhub.com/search/catalog/events/v3/?rows=100&parking=" + Parking + "&state=\"" + State + "\"&q=\"" + Performer + " \"&fieldList=id,name,eventDateLocal,venue";
                }
                else
                {
                    request = "https://api.stubhub.com/search/catalog/events/v3/?rows=100&parking=" + Parking + "&city=" + CityState + "&q=" + Performer + "&fieldList=id,name,eventDateLocal,venue";
                }
                //GET CREDENTIALS
                List<spGet_SHCredintials_Result> x = db.spGet_SHCredintials().ToList();
                String ApplicationToken = x[0].AppToken;
                String ConsumerKey = x[0].Key;
                String ConsumerSecret = x[0].Secret;
                String BearerToken = x[0].BearerToken;
                String SellerID = x[0].SellerID;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                WebRequest webRequest2 = WebRequest.Create(request);
                webRequest2.Method = "GET";
                webRequest2.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                webRequest2.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + BearerToken);
                WebResponse response2 = webRequest2.GetResponse();
                Stream dataStream3 = response2.GetResponseStream();
                StreamReader reader2 = new StreamReader(dataStream3);
                file2 = reader2.ReadToEnd();
                var k = JsonConvert.DeserializeObject<EventSearch.RootObject>(file2);
                return Json(k.events, JsonRequestBehavior.AllowGet);
            }
            catch (WebException webEx)
            {
                using (WebResponse response = webEx.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        return Json(text, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }
        public ActionResult UpdateSHEvent(Int32 eventId)
        {
            String eventName = Request.Form["EventName"];
            String eventDate = Request.Form["EventDate"];
            Int32 Event_ID = Convert.ToInt32(Request.Form["Event_ID"]);
            string sa = @"""" + eventDate + @"""";
            DateTime ed = JsonConvert.DeserializeObject<DateTime>(sa);
            db.spUpdate_SHEvent(Event_ID, eventId, eventName, ed);
            return Content("done");
        }
        public ActionResult FillPPO(Int32 PresaleID, Int32 ListingID)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            db.spFill_CategoryTicketGroup(Login_ID, PresaleID, ListingID);

            return Content("Success");
        }
    }

}