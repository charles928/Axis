using Microsoft.Ajax.Utilities;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace Axis.Controllers
{
    public class SeasonController : Controller
    {
        private AxisEntities db = new AxisEntities();
        // GET: Season
        public ActionResult AddPO()
        {
            Int32 NewPO_ID = 0;
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            if (Login_ID == 0)
            {
                return Redirect("../Home/Index");
            }
            else
            {
                Int32 Vendor_ID = Convert.ToInt32(Request.Form["VendorID"]);
                String PONotes = Request.Form["PONotes"];
                String Items = Request.Form["Items"];
                String Payments = Request.Form["Payments"];
                //List<spSTH_CreatePO_result> A = db.spSTH_CreatePO(Login_ID, Vendor_ID, PONotes, Items, Payments).ToList();
                //NewPO_ID = A[0].PO_ID;
            }
            return Content(NewPO_ID.ToString());
        }
        public ActionResult AddVendor()
        {
            Int32 Vendor_ID = 0;
            Boolean STHFlag = Convert.ToBoolean(Request.Form["STHFlag"]);
            String Company = Request.Form["Company"];
            String First = Request.Form["First"];
            String Last = Request.Form["Last"];
            String Address1 = Request.Form["Address1"];
            String Address2 = Request.Form["Address2"];
            String City = Request.Form["City"];
            Int32 State_ID = Convert.ToInt32(Request.Form["State_ID"]);
            String Zip = Request.Form["ZipCode"];
            Int32 Country_ID = Convert.ToInt32(Request.Form["Country_ID"]);
            String Email = Request.Form["Email"];
            String Phone = Request.Form["Phone"];
            //List<spCreate_Vendor_result> A = db.spCreate_Vendor(STHFlag, Company, First, Last, Address1, Address2, City, State_ID, Zip, Country_ID, Email, Phone).ToList();
            //Vendor_ID = A[0].Vendor_ID;
            return Content(Vendor_ID.ToString());
        }
        public ActionResult VendorSearch()
        {
            Boolean STHOnly = Convert.ToBoolean(Request.Form["STHOnly"]);
            String Company = Request.Form["Company"];
            String First = Request.Form["First"];
            String Last = Request.Form["Last"];
            String Address1 = Request.Form["Address1"];
            String Address2 = Request.Form["Address2"];
            String City = Request.Form["City"];
            String Zip = Request.Form["Zip"];
            String Email = Request.Form["Email"];
            String Phone = Request.Form["Phone"];
            List<spGet_VendorSearch_Result> A = db.spGet_VendorSearch(STHOnly, Company, First, Last, Address1, Address2, City, Zip, Email, Phone).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult STHEventInfo(Int32 Exchange_ID)
        {
            List<spGet_ExchangeInfo_Result> model = db.spGet_ExchangeInfo(Exchange_ID).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExchangeSearch()
        {
            String Input = Request.Form["Input"];

            String Start = Request.Form["Start"];
            DateTime? dStart = string.IsNullOrEmpty(Start) ? (DateTime?)null : DateTime.Parse(Start);

            String End = Request.Form["End"];
            DateTime? dEnd = string.IsNullOrEmpty(End) ? (DateTime?)null : DateTime.Parse(End);

            Int32 Parent_ID = Convert.ToInt32(Request.Form["ParentID"]);
            Int32 Child_ID = Convert.ToInt32(Request.Form["ChildID"]);
            Int32 Grandchild_ID = Convert.ToInt32(Request.Form["GrandchildID"]);
            IEnumerable<spGet_ExchangeSearch_Result> model = db.spGet_ExchangeSearch(Input, dStart, dEnd, Parent_ID, Child_ID, Grandchild_ID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStockType()
        {
            List<spGet_StockType_Result> ST = db.spGet_StockType().ToList();
            return Json(ST, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetShippingMethod()
        {
            List<spGet_ShippingMethodSpecial_Result> SM = db.spGet_ShippingMethodSpecial().ToList();
            return Json(SM, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreatePO()
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

                List<spGet_STHParentList_Result> PL = db.spGet_STHParentList(0, 0, 0).ToList();
                List<spGet_STHChildList_Result> CL = db.spGet_STHChildList(0, 0, 0).ToList();
                List<spGet_STHGrandchildList_Result> GL = db.spGet_STHGrandchildList(0, 0, 0).ToList();
                List<spGet_StateList_Result> ST = db.spGet_StateList(217).ToList();
                List<spGet_CountryList_Result> CO = db.spGet_CountryList().ToList();
                List<spGet_PayType_Result> PT = db.spGet_PayType().ToList();
                List<spGet_CCList_Result> CC = db.spGet_CCList().ToList();

                var ParentList = new SelectList(PL, "Parent_ID", "Desc").ToList();
                var ChildList = new SelectList(CL, "Child_ID", "Desc").ToList();
                var GrandchildList = new SelectList(GL, "Grandchild_ID", "Desc").ToList();
                var StateList = new SelectList(ST, "state_id", "state_name").ToList();
                var CountryList = new SelectList(CO, "country_id", "country_name").ToList();
                var PayTypeList = new SelectList(PT, "payment_type_id", "payment_type_desc").ToList();
                var CCList = new SelectList(CC, "cc_ID", "Desc").ToList();

                ViewData["ParentList"] = ParentList;
                ViewData["ChildList"] = ChildList;
                ViewData["GrandchildList"] = GrandchildList;
                ViewData["StateList"] = StateList;
                ViewData["CountryList"] = CountryList;
                ViewData["PayTypeList"] = PayTypeList;
                ViewData["CCList"] = CCList;

                List<spSTH_GetVendor_Result> DV = db.spSTH_GetVendor(1).ToList();
                ViewBag.Company = DV[0].client_broker_company_name;
                ViewBag.First = DV[0].client_broker_fname;
                ViewBag.Last = DV[0].client_broker_lname;
                ViewBag.Address1 = DV[0].street_address1;
                ViewBag.Address2 = DV[0].street_address2;
                ViewBag.City = DV[0].city;
                ViewBag.State_ID = DV[0].state_id;
                ViewBag.Zip = DV[0].postal_code;
                ViewBag.Country_ID = DV[0].system_country_id;
                ViewBag.Email = DV[0].client_broker_email;
                ViewBag.Phone = DV[0].phone_number;

                return View();
            }
        }
        public ActionResult MakePO()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            Int32 SourcePO_ID = Convert.ToInt32(Request.Form["SourcePO"]);
            Int32 Event_ID = Convert.ToInt32(Request.Form["Event"]);
            Int32 Agent_ID = Convert.ToInt32(Request.Form["Agent"]);
            Int32 CC_ID = Convert.ToInt32(Request.Form["Payment"]);
            String Items = Request.Form["Items"];
            List<spSTH_MakeClonePO_Result> A = db.spSTH_MakeClonePO(Login_ID, SourcePO_ID, Event_ID, Agent_ID, CC_ID, Items).ToList();
            Int32 PO_ID = Convert.ToInt32(A[0].PO_ID);
            return Content(PO_ID.ToString());
        }
        public ActionResult AddEvent()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            String Name = Request.Form["Name"];
            String Date = Request.Form["Date"];
            Int32 Headliner_ID = Convert.ToInt32(Request.Form["Headliner"]);
            Int32 Parent_ID = Convert.ToInt32(Request.Form["Parent"]);
            Int32 Child_ID = Convert.ToInt32(Request.Form["Child"]);
            Int32 Grandchild_ID = Convert.ToInt32(Request.Form["Grandchild"]);
            Int32 Venue_ID = Convert.ToInt32(Request.Form["Venue"]);
            List<spCreate_NewEvent_Result> A = db.spCreate_NewEvent(Login_ID, Venue_ID, Name, Date, Headliner_ID, Parent_ID, Child_ID, Grandchild_ID).ToList();
            return Content(A[0].Event_ID.ToString());
        }
        public ActionResult STHEventList(Int32 Season, Int32 Performer_ID, Int32 Parent_ID, Int32 Child_ID, Int32 Grandchild_ID)
        {
            List<spGet_STHEventList_Result> A = db.spGet_STHEventList(Season, Performer_ID, Parent_ID, Child_ID, Grandchild_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
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
                List<spGet_SeasonList_Result> model = db.spGet_SeasonList().ToList();
                ViewData["ItemCount"] = model.Count().ToString() + " Item(s)";
                ViewData["TotalQty"] = String.Format("{0:N0}", model.Sum(x => x.Qty));
                ViewData["TicketQty"] = model[0].Qty;
                ViewData["TotalCost"] = String.Format("{0:c}", model.Sum(x => x.TotalCost));
                List<spSeason_History_Result> H = db.spSeason_History().ToList();
                ViewData["SplitCount"] = H.Count().ToString() + " Item(s)";
                ViewData["SplitQty"] = String.Format("{0:N0}", H.Sum(x => x.qty));
                ViewData["SplitCost"] = String.Format("{0:c}", H.Sum(x => x.Cost));
                ViewData["History"] = H;
                return View(model);
            }
        }
        public ActionResult ClonePO(Int32 PO_ID, Int32 Season)
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

                List<spSTH_CloneInfo_Result> B = db.spSTH_CloneInfo(PO_ID).ToList();
                Int32 Client_Broker_ID = Convert.ToInt32(B[0].Buyer_Broker_ID);
                Int32 CC_ID = Convert.ToInt32(B[0].CC_ID);

                List<spSTH_Clone_Result> Model = db.spSTH_Clone(PO_ID).ToList();

                Int32 Parent_ID = (Int32)(Session["STHParent_ID"] ?? 0);
                if (Parent_ID == 0)
                    Parent_ID = B[0].event_parent_category_id;

                Int32 Child_ID = (Int32)(Session["STHChild_ID"] ?? 0);
                if (Child_ID == 0)
                    Child_ID = B[0].event_child_category_id;

                Int32 Grandchild_ID = (Int32)(Session["STHGrandchild_ID"] ?? 0);
                if (Grandchild_ID == 0)
                    Grandchild_ID = B[0].event_grandchild_category_id;

                Int32 Headliner_ID = (Int32)(Session["STHHeadliner_ID"] ?? 0);
                if (Headliner_ID == 0)
                    Headliner_ID = Convert.ToInt32(B[0].performer_id);

                List<spGet_STHDD_Result> AL = db.spGet_STHDD(0, 0, 0, Headliner_ID).ToList(); // Parent_ID, Child_ID, Grandchild_ID, Headliner_ID).ToList();
                List<spGet_STHParentList_Result> PL = db.spGet_STHParentList(Child_ID, Grandchild_ID, Headliner_ID).ToList();
                List<spGet_STHChildList_Result> CL = db.spGet_STHChildList(Parent_ID, Grandchild_ID, Headliner_ID).ToList();
                List<spGet_STHGrandchildList_Result> GL = db.spGet_STHGrandchildList(Parent_ID, Child_ID, Headliner_ID).ToList();
                List<spGet_STHPerformerList_Result> HL = db.spGet_STHPerformerList(Parent_ID, Child_ID, Grandchild_ID).ToList();
                List<spGet_CCList_Result> CC = db.spGet_CCList().ToList();
                List<spGet_STHEventList_Result> EL = db.spGet_STHEventList(Season + 1, Headliner_ID, Parent_ID, Child_ID, Grandchild_ID).ToList();

                ViewBag.BeginMonth = B[0].BeginMonth;
                ViewBag.BeginDay = B[0].BeginDay;
                ViewBag.Venue_ID = B[0].venue_id;
                ViewBag.Season = Season + 1;
                ViewBag.Agent_ID = Client_Broker_ID;
                ViewBag.Headliner_ID = Headliner_ID;
                ViewBag.Parent_ID = Parent_ID;
                ViewBag.Child_ID = Child_ID;
                ViewBag.Grandchild_ID = Grandchild_ID;
                ViewBag.CC_ID = CC_ID;

                ViewBag.RecordCount = Model.Count().ToString() + " Records";
                ViewBag.ItemCount = String.Format("{0:N0}", Model.Sum(x => x.Qty));
                ViewBag.ItemTotal = String.Format("{0:c}", Model.Sum(x => x.Cost));

                var AgentList = new SelectList(AL, "Agent_ID", "Desc").ToList();
                var ParentList = new SelectList(PL, "Parent_ID", "Desc").ToList();
                var ChildList = new SelectList(CL, "Child_ID", "Desc").ToList();
                var GrandchildList = new SelectList(GL, "Grandchild_ID", "Desc").ToList();
                var HeadlinerList = new SelectList(HL, "Performer_ID", "Desc").ToList();
                var CCList = new SelectList(CC, "cc_ID", "Desc").ToList();
                var EventList = new SelectList(EL, "Event_ID", "Desc").ToList();

                ViewData["AgentList"] = AgentList;
                ViewData["HeadlinerList"] = HeadlinerList;
                ViewData["ParentList"] = ParentList;
                ViewData["ChildList"] = ChildList;
                ViewData["GrandchildList"] = GrandchildList;
                ViewData["CCList"] = CCList;
                ViewData["EventList"] = EventList;
                ViewData["SourcePO"] = PO_ID;
                ViewData["PageName"] = "Clone PO#" + PO_ID.ToString();

                return View(Model);
            }
        }
        public ActionResult Dashboard()
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

                Int32 Parent_ID = (Int32)(Session["STHParent_ID"] ?? 0);
                Int32 Child_ID = (Int32)(Session["STHChild_ID"] ?? 0);
                Int32 Grandchild_ID = (Int32)(Session["STHGrandchild_ID"] ?? 0);
                Int32 Headliner_ID = (Int32)(Session["STHHeadliner_ID"] ?? 0);

                List<spGet_STHParentList_Result> PL = db.spGet_STHParentList(Child_ID, Grandchild_ID, Headliner_ID).ToList();
                List<spGet_STHChildList_Result> CL = db.spGet_STHChildList(Parent_ID, Grandchild_ID, Headliner_ID).ToList();
                List<spGet_STHGrandchildList_Result> GL = db.spGet_STHGrandchildList(Parent_ID, Child_ID, Headliner_ID).ToList();
                List<spGet_STHPerformerList_Result> HL = db.spGet_STHPerformerList(Parent_ID, Child_ID, Grandchild_ID).ToList();

                List<spGet_STHPerformerList_Result> HL2 = db.spGet_STHPerformerList(-1, 0, 0).ToList();
                List<spGet_StateList_Result> ST = db.spGet_StateList(217).ToList();
                List<spGet_CCList_Result> CC = db.spGet_CCList().ToList();

                var ParentList = new SelectList(PL, "Parent_ID", "Desc").ToList();
                var ChildList = new SelectList(CL, "Child_ID", "Desc").ToList();
                var GrandchildList = new SelectList(GL, "Grandchild_ID", "Desc").ToList();
                var HeadlinerList = new SelectList(HL, "Performer_ID", "Desc").ToList();
                var HeadlinerList2 = new SelectList(HL2, "Performer_ID", "Desc").ToList();
                var StateList = new SelectList(ST, "state_id", "state_name").ToList();
                var CCList = new SelectList(CC, "cc_ID", "Desc").ToList();

                ViewBag.Parent_ID = Parent_ID;
                ViewBag.Child_ID = Child_ID;
                ViewBag.Grandchild_ID = Grandchild_ID;
                ViewBag.Headliner_ID = Headliner_ID;
                ViewData["ParentList"] = ParentList;
                ViewData["ChildList"] = ChildList;
                ViewData["GrandchildList"] = GrandchildList;
                ViewData["HeadlinerList"] = HeadlinerList;
                ViewData["HeadlinerList2"] = HeadlinerList2;
                ViewData["StateList"] = StateList;
                ViewData["CCList"] = CCList;
 
                List<spGet_STHList_Result> model = db.spGet_STHList(Parent_ID, Child_ID, Grandchild_ID, Headliner_ID).ToList();
                ViewBag.RecordCount = model.Count().ToString() + "   Records";
                return View(model);
            }
        }
        public ActionResult Calendar()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            if (Login_ID == 0)
            {
                return Redirect("../Home/Index");
            }
            else
            {
                string crap = (string)Session["STH_ID"] ?? "empty";

                if (crap == "empty")
                {
                    Session["STH_ID"] = Login_ID.ToString();
                }

                List<spUser_GetLogin_Result> A = db.spUser_GetLogin(Login_ID).ToList();
                List<spUser_GetLogin_Result> B = db.spUser_GetLogin(-1).ToList();
                List<spGet_STHPerformerList_Result> C = db.spGet_STHPerformerList(0, 0, 0).ToList();

                var UserList = new SelectList(B, "Login_ID", "FullName").ToList();
                var HeadlinerList = new SelectList(C, "Performer_ID", "Desc").ToList();

                ViewData["UserList"] = UserList;
                ViewData["HeadlinerList"] = HeadlinerList;
                ViewData["Name"] = A[0].FullName;
                ViewData["Photo"] = @"/Images/" + A[0].Photo;

                return View();
            }
        }
        public ActionResult SendSingleInvite(Int32 Request_ID, String FullName)
        {
            // Get data for appointment from request (Message, EventDate)
            List<spSTH_GetRequest_Result> A = db.spSTH_GetRequest(Request_ID).ToList();
            String Message = A[0].Message;
            DateTime EventDate = A[0].EventDate;

            // Add name to request if it's not already added return Email
            List<spSTH_AddToRequest_Result> B = db.spSTH_AddToRequest(Request_ID, FullName).ToList();
            String Email = B[0].Email;

            ExchangeService service = new ExchangeService();
            service.Credentials = new WebCredentials("axis@ticketcity.com", "aVav04942");
            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");

            Appointment appointment = new Appointment(service);
            appointment.Body = Message.Replace(System.Environment.NewLine, "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
            appointment.Start = EventDate;
            appointment.StartTimeZone = TimeZoneInfo.Local;
            appointment.End = appointment.Start.AddHours(.001);
            appointment.EndTimeZone = TimeZoneInfo.Local;
            appointment.Location = "";
            appointment.ReminderDueBy = DateTime.Now;
            appointment.RequiredAttendees.Add(Email);
            appointment.Subject = "STH Request #" + Request_ID;
            appointment.Save(SendInvitationsMode.SendToAllAndSaveCopy);
            return Content("Success");
        }
        public ActionResult DeleteRequest(Int32 Request_ID)
        {
            List<spSTH_DeleteRequest_Result> A = db.spSTH_DeleteRequest(Request_ID).ToList();
            return Content(A[0].Return);
        }
        public ActionResult GetCalendarDD()
        {
            List<spSTH_NoticePeople_Result> A = db.spSTH_NoticePeople().ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetClientBrokerAddress(Int32 ClientBroker_ID)
        {
            List<spGet_ClientBrokerAddress_Result> A = db.spGet_ClientBrokerAddress(ClientBroker_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRequest(Int32 Request_ID)
        {
            List<spSTH_GetRequest_Result> A = db.spSTH_GetRequest(Request_ID).ToList();
            ViewBag.Request_ID = A[0].Request_ID;
            ViewBag.CreatedBy = A[0].CreatedBy;
            ViewBag.CreatedOn = A[0].Created;
            ViewBag.EventDate = A[0].EventDate;
            ViewBag.Type = A[0].Type;
            ViewBag.Headliner = A[0].performer_name;
            ViewBag.Message = A[0].Message;
            ViewBag.Title = "STH Request #" + A[0].Request_ID.ToString();

            List<spUser_GetLogin_Result> B = db.spUser_GetLogin(-1).ToList();
            var UserList = new SelectList(B, "Login_ID", "FullName").ToList();
            ViewData["UserList"] = UserList;

            List<spSTH_GetRequestUsers_Result> model = db.spSTH_GetRequestUsers(Request_ID).ToList();
            return View(model);
        }
        public JsonResult GetEvents(DateTime start, DateTime end)
        {
            Int32 STH_ID = Convert.ToInt32(Session["STH_ID"]);
            List<spSTH_GetNotices_Result> A = db.spSTH_GetNotices(STH_ID, start, end).ToList();
            foreach (var item in A)
            {
                DateTime before = item.start1 ?? DateTime.Now;
                item.start = before.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz").Remove(26, 1);

                DateTime after = item.end1 ?? DateTime.Now;
                item.end = after.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz").Remove(26, 1);

            }
            return Json(A.ToArray(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangeSTH_ID(Int32 New_ID)
        {
            Session["STH_ID"] = New_ID.ToString();
            return Content("success");
        }
        public ActionResult GetAccountsForHeadliner(Int32 Headliner_ID)
        {
            List<spGet_STHList_Result> model = db.spGet_STHList(0, 0, 0, Headliner_ID).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult POPopup(Int32 PO_ID)
        {
            List<spGet_POItemsSecRow_Result> model = db.spGet_POItemsSecRow(PO_ID).ToList();
            ViewBag.PO_ID = PO_ID;
            ViewBag.Title = "PO# " + PO_ID.ToString();
            Int32 TCount = model.Sum(x => x.Header == 1 ? Convert.ToInt32(x.Qty) : 0);
            Decimal TCost = model.Sum(x => x.Header == 1 ? Convert.ToDecimal(x.TotalCost) : 0);
            Decimal TPrice = model.Sum(x => x.Header == 1 ? Convert.ToDecimal(x.TotalPrice) : 0);

            ViewBag.TotalQty = String.Format("{0:N0}", TCount);
            ViewBag.TotalCost = String.Format("{0:c}", TCost);
            ViewBag.TotalPrice = String.Format("{0:c}", TPrice);
            ViewBag.TotalProfit = String.Format("{0:c}", TPrice - TCost);
            return PartialView(model);
        }
        public ActionResult POPopup2(Int32 PO_ID)
        {
            List<spGet_POItems_Result> model = db.spGet_POItems(PO_ID).ToList();
            ViewBag.PO_ID = PO_ID;
            ViewBag.Title = "PO# " + PO_ID.ToString();
            Int32 TCount = model.Sum(x => x.Header == 1 ?Convert.ToInt32(x.Qty) : 0);
            Decimal TCost = model.Sum(x => x.Header == 1 ? Convert.ToDecimal(x.Total) : 0);
            Decimal TPrice = model.Sum(x => x.Header == 1 ? Convert.ToDecimal(x.TotalPrice) : 0);
            
            ViewBag.TotalQty = String.Format("{0:N0}", TCount);
            ViewBag.TotalCost = String.Format("{0:c}", TCost);
            ViewBag.TotalPrice = String.Format("{0:c}", TPrice);
            ViewBag.TotalProfit = String.Format("{0:c}", TPrice - TCost);
            return PartialView(model);
        }
        public ActionResult SendInvite()
        {
            String SelectedDate = Request.Form["Date"];
            String SelectedTime = Request.Form["Time"];
            DateTime EventDate = Convert.ToDateTime(SelectedDate + ' ' + SelectedTime);
            Int32 Type_ID = Convert.ToInt32(Request.Form["Type"]);
            Int32 Headliner_ID = Convert.ToInt32(Request.Form["Headliner"]);
            String SelectedUsers = Request.Form["Users"];
            String SelectedAccounts = Request.Form["Brokers"];
            String Message = Request.Form["Message"];

            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);

            List<spSTH_AddNotice_Result> A = db.spSTH_AddNotice(Login_ID, EventDate, Type_ID, Headliner_ID, SelectedUsers, SelectedAccounts, Message).ToList();
            
            ExchangeService service = new ExchangeService();
            service.Credentials = new WebCredentials("axis@ticketcity.com", "aVav04942");
            service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");

            Appointment appointment = new Appointment(service);
            appointment.Body = Message.Replace(System.Environment.NewLine, "<br />").Replace("\n", "<br />").Replace("\r", "<br />");
            appointment.Start = EventDate;
            appointment.End = appointment.Start.AddHours(.001);
            appointment.Location = "";
            appointment.ReminderDueBy = DateTime.Now;

            foreach(var item in A)
            {
                appointment.RequiredAttendees.Add(item.Email);
                appointment.Subject = "STH Request #" + item.Request_ID;
            }

            appointment.Save(SendInvitationsMode.SendToAllAndSaveCopy);

            return Content("success");
        }
        public ActionResult GetAccountDetails(Int32 ClientBroker_ID)
        {
            Int32 Parent_ID = Convert.ToInt32(Request.Form["Parent"]);
            Int32 Child_ID = Convert.ToInt32(Request.Form["Child"]);
            Int32 Grandchild_ID = Convert.ToInt32(Request.Form["Grandchild"]);
            Int32 Headliner_ID = Convert.ToInt32(Request.Form["Headliner"]);
            List<spGet_STHAccount_Result> model = db.spGet_STHAccount(ClientBroker_ID, Parent_ID, Child_ID, Grandchild_ID, Headliner_ID).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ResetFilter()
        {
            Session["STHParent_ID"] = 0;
            Session["STHChild_ID"] = 0;
            Session["STHGrandchild_ID"] = 0;
            Session["STHHeadliner_ID"] = 0;
            return Content("success");
        }
        public ActionResult JsonConvertItem(Int32 tg_id)
        {
            List<spGet_SeasonGames_Result> model = db.spGet_SeasonGames(tg_id).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SplitListing(Int32 Listing_ID)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            String Data = Request.Form["SplitString"];
            List<spSplit_New_Result> A = db.spSplit_New(Listing_ID, Data, Login_ID).ToList();
            if (A[0].ReturnCode == true)
                return Content("success");
            else
                return Content("fail");
        }
        public ActionResult AddExchangeEvent(Int32 Ticket_Group_ID)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            String Data = Request.Form["SplitString"];

            List<spExchange_AddEvents_Result> A = db.spExchange_AddEvents(Ticket_Group_ID, Login_ID, Data).ToList();
            if (A[0].ReturnCode == true)
                return Content("success");
            else
                return Content("fail");
        }
        public ActionResult JsonMissingEvents(Int32 Ticket_Group_ID)
        {
            List<spGet_MissingEvents_Result> A = db.spGet_MissingEvents(Ticket_Group_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult STHSearch(String Input)
        {
            List<spGet_STHSearch_Result> A = db.spGet_STHSearch(Input).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangeName(Int32 Client_Broker_ID)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            String NewName = Request.Form["NewName"];
            //db.spUpdate_STHBrokerName(Login_ID, Client_Broker_ID, NewName);
            return Content("success");
        }
        public ActionResult UpdateValue()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            Int32 ClientBroker_ID = Convert.ToInt32(Request.Form["ClientBroker_ID"]);
            String Source = Request.Form["Source"];
            String NewValue = Request.Form["NewValue"];

            List<spUpdate_STHValue_Result> A = db.spUpdate_STHValue(Login_ID, Source, ClientBroker_ID, NewValue).ToList();

            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateHeadliner()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            Int32 Performer_ID = Convert.ToInt32(Request.Form["Performer_ID"]);
            Int32 ClientBroker_ID = Convert.ToInt32(Request.Form["ClientBroker_ID"]);
            db.spUpdate_STHPerformer_ID(Login_ID, Performer_ID, ClientBroker_ID);
            return Content("success");
        }
        public ActionResult UpdateCard()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            Int32 Card_ID = Convert.ToInt32(Request.Form["Card_ID"]);
            Int32 ClientBroker_ID = Convert.ToInt32(Request.Form["ClientBroker_ID"]);
            List<spUpdate_STHCard_ID_Result> A = db.spUpdate_STHCard_ID(Login_ID, Card_ID, ClientBroker_ID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateBroker(Int32 ClientBroker_ID)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            Byte BAction = Convert.ToByte(Request.Form["BrokerAction"]);
            db.spUpdate_STHBroker(Login_ID, ClientBroker_ID, BAction);
            return Content("success");
        }
        public ActionResult UpdateValues(String Type, Int32 NewValue)
        {
            if (Type == "Parent")
                Session["STHParent_ID"] = NewValue;
            if (Type == "Child")
                Session["STHChild_ID"] = NewValue;
            if (Type == "Grandchild")
                Session["STHGrandchild_ID"] = NewValue;
            if (Type == "Headliner")
                Session["STHHeadliner_ID"] = NewValue;

            return Content("success");
        }

    }
}