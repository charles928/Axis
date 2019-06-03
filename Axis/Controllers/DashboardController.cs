using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axis.Controllers
{
    public class DashboardController : Controller
    {
        private AxisEntities db = new AxisEntities();
        // GET: Dashboard
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

                DateTime vFrom = DateTime.Now;
                DateTime vTo = DateTime.Now;
                List<spGet_POList_Result> C = db.spGet_POList(vFrom, vTo, false, false, false, false, false, "").ToList();
                
                ViewBag.FooterTitle = C.Count().ToString() + " Records";

                Int32 QUnsold = Convert.ToInt32(C.Sum(item => item.Status == "Consign" ? 0 : item.Status == "Void" ? 0 : item.Unsold));
                Int32 QSold = Convert.ToInt32(C.Sum(item => item.Status == "Consign" ? 0 : item.Status == "Void" ? 0 : item.Sold));
                Decimal QUnsoldCost = Convert.ToDecimal(C.Sum(item => item.Status == "Consign" ? 0 : item.Status == "Void" ? 0 : item.UnsoldCost));
                Decimal QSoldCost = Convert.ToDecimal(C.Sum(item => item.Status == "Consign" ? 0 : item.Status == "Void" ? 0 : item.SoldCost));
                Decimal QUnsoldPrice = Convert.ToDecimal(C.Sum(item => item.Status == "Consign" ? 0 : item.Status == "Void" ? 0 : item.UnsoldPrice));
                Decimal QSoldPrice = Convert.ToDecimal(C.Sum(item => item.Status == "Consign" ? 0 : item.Status == "Void" ? 0 : item.SoldPrice));

                Decimal QMargin = 0;
                if (QSoldCost == 0)
                {
                    if (QSoldPrice == 0)
                        QMargin = 0;
                    else
                        QMargin = 100;
                }
                else
                {
                    if (QSoldPrice == 0)
                        QMargin = 0;
                    else
                    {
                        if (QSoldPrice >= QSoldCost)
                        {
                            if (QSoldPrice == 0)
                                QMargin = 0;
                            else
                                QMargin = (QSoldPrice - QSoldCost) / QSoldPrice;
                        }
                        else
                        {
                            if (QSoldCost == 0)
                                QMargin = 0;
                            else
                                QMargin = (QSoldPrice - QSoldCost) / QSoldCost;
                        }
                    }
                }
                ViewBag.Margin = String.Format("{0:P}", QMargin);
                Decimal QPercentSold = 0;
                Decimal QAvgPrice = 0;
                Decimal QAvgCost = 0;
                if ((QSold + QUnsold) > 0)
                {
                    QPercentSold = Convert.ToDecimal(QSold) / Convert.ToDecimal(QSold + QUnsold);
                    QAvgPrice = Decimal.Round((QUnsoldPrice + QSoldPrice) / (QSold + QUnsold), 2);
                    QAvgCost = Decimal.Round((QUnsoldCost + QSoldCost) / (QSold + QUnsold), 2);
                }
                ViewBag.UnsoldQty = String.Format("{0: ###,###}", QUnsold);
                ViewBag.SoldQty = String.Format("{0: ###,###}", QSold);

                ViewBag.SoldCost = String.Format("{0:c}", QSoldCost);
                ViewBag.UnsoldCost = String.Format("{0:c}", QUnsoldCost);

                ViewBag.UnsoldPrice = String.Format("{0:c}", QUnsoldPrice);
                ViewBag.SoldPrice = String.Format("{0:c}", QSoldPrice);

                ViewBag.Profit = String.Format("{0:c}", QSoldPrice - QSoldCost);
                ViewBag.ProjectedProfit = String.Format("{0:c}", QUnsoldPrice - QUnsoldCost);

                ViewBag.TotalQty = String.Format("{0:###,###}", QSold + QUnsold);
                ViewBag.TotalRevenue = String.Format("{0:c}", QSoldPrice + QUnsoldPrice);

                ViewBag.PercentSold = String.Format("{0:P}", QPercentSold);

                ViewBag.AvgPrice = String.Format("{0:c}", QAvgPrice);
                ViewBag.AvgCost = String.Format("{0:c}", QAvgCost);
                                
                List<spGet_ExecptionTotals_Result> B = db.spGet_ExecptionTotals().ToList();
                ViewData["999Listings"] = @String.Format("{0:#,###,###}", B[0].C999Listings) + " Listings";
                ViewData["999Items"] = String.Format("{0:#,###,###}", B[0].C999Tickets) + " Items";
                ViewData["999EVENTS"] = String.Format("{0:#,###,###}", B[0].C999Events) + "  EVENTS";
                ViewData["UnsharedListings"] = @String.Format("{0:#,###,###}", B[0].UnsharedListings) + " Listings";
                ViewData["UnsharedItems"] = String.Format("{0:#,###,###}", B[0].UnsharedTickets) + " Items";
                ViewData["UnsharedEVENTS"] = String.Format("{0:#,###,###}", B[0].UnsharedEvents) + "  EVENTS";
                ViewData["UnpricedListings"] = @String.Format("{0:#,###,###}", B[0].UnpricedListings) + " Listings";
                ViewData["UnpricedItems"] = String.Format("{0:#,###,###}", B[0].UnpricedTickets) + " Items";
                ViewData["UnpricedEVENTS"] = String.Format("{0:#,###,###}", B[0].UnpricedEvents) + "  EVENTS";

                ViewData["SoldQty"] = QSold;
                ViewData["UnsoldQty"] = QUnsold;
                //ViewData["SharedQty"] = Convert.ToInt32(C.Sum(item => item.Shared));
                //ViewData["SharedPrice"] = Convert.ToDecimal(C.Sum(item => item.SharedPrice));
                ViewData["SoldCost"] = QSoldCost;
                ViewData["UnsoldCost"] = QUnsoldCost;

                ViewData["PercentSold"] = Decimal.Round(QPercentSold, 2);
                ViewData["AvailablePrice"] = QUnsoldPrice;
                ViewData["Profit"] = QSoldPrice - QSoldCost;
                ViewData["ProjectedProfit"] = QUnsoldPrice - QUnsoldCost;
                ViewData["AvgPrice"] = QAvgPrice;
                ViewData["AvgCost"] = QAvgCost;
                ViewData["TotalQty"] = QSold + QUnsold;
                ViewData["TotalProfit"] = QSoldPrice - QSoldCost + QUnsoldPrice - QUnsoldCost;
                ViewData["POList"] = C;
                IEnumerable<spGet_MyEvents_Result> Model = db.spGet_MyEvents(Login_ID);

                DateTime Start = Convert.ToDateTime(Session["StartDate"]);
                DateTime End = Convert.ToDateTime(Session["EndDate"]);

                List<spGet_Categories_Result> CL = db.spGet_Categories().ToList();
                List<spGet_HeadlinersList_Result> HL = db.spGet_HeadlinersList(Start, End, 0).ToList();
                List<spGet_AssignedOptions_Result> OL = db.spGet_AssignedOptions().ToList();

                var CatList = new SelectList(CL, "Cat_ID", "Cat_Desc").ToList();
                var HeadlinerList = new SelectList(HL, "Headliner_ID", "Event_Headliner").ToList();
                var OptionList = new SelectList(OL, "TN_ID", "text").ToList();

                ViewData["CatList"] = CatList;
                ViewData["HeadlinerList"] = HeadlinerList;
                ViewData["OptionList"] = OptionList;

                return View(Model);
            }
        }
        public ActionResult GetSales(Int32 Login_ID)
        {
            IEnumerable<spGet_AgentInvoices_Result> Model = db.spGet_AgentInvoices(Login_ID);
            return Json(Model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMyEvents(Int32 Login_ID)
        {
            IEnumerable<spGet_MyEvents_Result> Model = db.spGet_MyEvents(Login_ID);
            return Json(Model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEventsImFollowing(Int32 Login_ID)
        {
            IEnumerable<spGet_EventsImFollowing_Result> Model = db.spGet_EventsImFollowing(Login_ID);
            return Json(Model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNotes()
        {
            IEnumerable<spGet_Notes_Result> Model = db.spGet_Notes();
            return Json(Model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNineList()
        {
            IEnumerable<spGet_999List_Result> model = db.spGet_999List();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUnsharedList(Int16 Cat_ID)
        {
            IEnumerable<spGet_UnsharedList_Result> model = db.spGet_UnsharedList(Cat_ID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUnpricedList()
        {
            IEnumerable<spGet_UnpricedList_Result> model = db.spGet_UnpricedList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPeopleAssigned()
        {
            IEnumerable<spGet_PeopleAssigned_Result> model = db.spGet_PeopleAssigned();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPeopleSales()
        {
            IEnumerable<spGet_PeopleSales_Result> model = db.spGet_PeopleSales();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Search(String Input)
        {
            IEnumerable<spGet_Search_Result> model = db.spGet_Search(Input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AdvancedSearch()
        {
            DateTime Start = Convert.ToDateTime(Request.Form["Start"]);
            DateTime End = Convert.ToDateTime(Request.Form["End"]);
            Int32 CatID = Convert.ToInt32(Request.Form["CatID"]);
            Int32 HeadlinerID = Convert.ToInt32(Request.Form["HeadlinerID"]);
            Int32 OptionID = Convert.ToInt32(Request.Form["OptionID"]);
            List<spGet_AdvancedSearch_Result> A = db.spGet_AdvancedSearch(Start, End, CatID, HeadlinerID, OptionID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddNote()
        {
            String NewNote = Request.Form["Message"];
            Int32 Login_ID = Convert.ToInt32(Request.Form["Login_ID"]);
            db.spAddNewNote(Login_ID, NewNote);
            return Content("success");
        }
        public ActionResult BuildHeadlinerList()
        {
            DateTime Start = Convert.ToDateTime(Request.Form["Start"]);
            DateTime End = Convert.ToDateTime(Request.Form["End"]);
            Int32 CatID = Convert.ToInt32(Request.Form["CatID"]);
            List<spGet_HeadlinersList_Result> A = db.spGet_HeadlinersList(Start, End, CatID).ToList();
            return Json(A, JsonRequestBehavior.AllowGet);
        }
    }
}