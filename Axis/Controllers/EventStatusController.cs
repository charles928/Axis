using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axis.Controllers
{
    public class EventStatusController : Controller
    {
        private AxisEntities db = new AxisEntities();
        // GET: EventStatus
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

                DateTime Start = (DateTime)(Session["esStart"] ?? DateTime.Today.AddDays(-45));
                DateTime End = (DateTime)(Session["esEnd"] ?? DateTime.Today.AddDays(45));
                Int32 Cat_ID = (Int32)(Session["esCat_ID"] ?? 0);
                Int32 Performer_ID = (Int32)(Session["esPerformer_ID"] ?? 0);
                Int32 Venue_ID = (Int32)(Session["esVenue_ID"] ?? 0);
                Int32 Parent_ID = (Int32)(Session["esParent_ID"] ?? 0);

                List<spGet_CatParents_Result> P = db.spGet_CatParents().ToList();
                var ParentList = new SelectList(P, "Parent_ID", "Parent").ToList();
                ViewData["ParentList"] = ParentList;

                List<spGet_ESVenues_Result> V = db.spGet_ESVenues(Start, End, 0, 0).ToList();
                var VenueList = new SelectList(V, "Venue_ID", "Venue").ToList();
                ViewData["VenueList"] = VenueList;

                Session["esStart"] = Start;
                Session["esEnd"] = End;
                Session["esVenue_ID"] = Venue_ID;
                Session["esParent_ID"] = Parent_ID;

                List<spGet_EventStatus1_Result> model = db.spGet_EventStatus1(Start, End, Venue_ID, Parent_ID).ToList();

                Int32 fCount = model.Count();
                Int32 fPerformers = Convert.ToInt32(model.Sum(item => item.Performers));
                Int32 fEvents = Convert.ToInt32(model.Sum(item => item.Events));
                Int32 fUnsoldQty = Convert.ToInt32(model.Sum(item => item.Unsold));
                Double fUnsoldCost = Convert.ToDouble(model.Sum(item => item.UnsoldCost));
                Double fUnsoldPrice = Convert.ToDouble(model.Sum(item => item.UnsoldPrice));
                Int32 fSoldQty = Convert.ToInt32(model.Sum(item => item.Sold));
                Double fSoldCost = Convert.ToDouble(model.Sum(item => item.SoldCost));
                Double fSoldPrice = Convert.ToDouble(model.Sum(item => item.SoldPrice));
                Double fSoldPrice2 = Convert.ToDouble(model.Sum(item => item.SoldPrice2));
                Double fSoldProfit = fSoldPrice - fSoldCost;
                Double fProfit = fSoldPrice - fUnsoldCost - fSoldCost;
                Double fSoldMargin;
                if (fSoldCost == 0)
                {
                    if (fSoldPrice == 0)
                    {
                        fSoldMargin = 0.00;
                    }
                    else
                    {
                        fSoldMargin = 100;
                    }
                }
                else
                {
                    if (fSoldPrice == 0)
                    {
                        fSoldMargin = 0.00;
                    }
                    else
                    {
                        if (fSoldPrice > fSoldCost)
                        {
                            if (fSoldPrice == 0)
                            {
                                fSoldMargin = 0.00;
                            }
                            else
                            {
                                fSoldMargin = (fSoldPrice - fSoldCost) / fSoldPrice * 100;
                            }
                        }
                        else
                        {
                            if (fSoldCost == 0)
                            {
                                fSoldMargin = 0.00;
                            }
                            else
                            {
                                fSoldMargin = (fSoldPrice - fSoldCost) / (fSoldCost) * 100;
                            }
                        }
                    }
                }
                Double fMargin;
                if (fSoldCost + fUnsoldCost == 0)
                {
                    if (fSoldPrice == 0)
                    {
                        fMargin = 0.00;
                    } else
                    {
                        fMargin = 100;
                    }

                } else
                {
                    if (fSoldPrice == 0)
                    {
                        fMargin = 0.00;
                    } else
                    {
                        if (fSoldPrice > fUnsoldCost + fSoldCost)
                        {
                            if (fSoldPrice == 0)
                            {
                                fMargin = 0.00;
                            } else
                            {
                                fMargin = (fSoldPrice - fUnsoldCost - fSoldCost) / fSoldPrice * 100;
                            }
                        } else
                        {
                            if (fUnsoldCost + fSoldCost == 0)
                            {
                                fMargin = 0.00;
                            } else
                            {
                                fMargin = (fSoldPrice - fUnsoldCost - fSoldCost) / (fUnsoldCost + fSoldCost) * 100;
                            }
                        }
                    }
                }
                ViewData["fCount"] = fCount;
                ViewData["fPerformers"] = fPerformers;
                ViewData["fEvents"] = fEvents;
                ViewData["fUnsoldQty"] = fUnsoldQty;
                ViewData["fUnsoldCost"] = fUnsoldCost;
                ViewData["fUnsoldPrice"] = fUnsoldPrice;
                ViewData["fSoldQty"] = fSoldQty;
                ViewData["fSoldCost"] = fSoldCost;
                ViewData["fSoldPrice"] = fSoldPrice;
                ViewData["fSoldPrice2"] = String.Format("Price including Category, Consign (not suitable for profit calculations) = {0:C}", fSoldPrice2);
                ViewData["fSoldProfit"] = fSoldProfit;
                ViewData["fSoldMargin"] = fSoldMargin;
                ViewData["fProfit"] = fProfit;
                ViewData["fMargin"] = fMargin;

                return View(model);
            }
        }
        public ActionResult UpdateVenue(Int32 Venue_ID)
        {
            Session["esVenue_ID"] = Venue_ID;
            return Content("success");
        }
        public ActionResult UpdateParent(Int32 Parent_ID)
        {
            Session["esParent_ID"] = Parent_ID;
            return Content("success");
        }
        public ActionResult UpdateStart(DateTime Date)
        {
            Session["esStart"] = Date;
            return Content("success");
        }
        public ActionResult UpdateEnd(DateTime Date)
        {
            Session["esEnd"] = Date;
            return Content("success");
        }
        public ActionResult Level2()
        {
            DateTime StartDate = DateTime.Parse(Request.Form["Start"]);
            DateTime EndDate = DateTime.Parse(Request.Form["End"]);
            Int32 Venue_ID = Convert.ToInt32(Request.Form["Venue_ID"]);
            Int32 Parent_ID = Convert.ToInt32(Request.Form["Parent_ID"]);
            Int32 Cat_ID = Convert.ToInt32(Request.Form["Cat_ID"]);
            Session["Cat_ID"] = Cat_ID;
            IEnumerable<spGet_EventStatus2_Result> model = db.spGet_EventStatus2(StartDate, EndDate, Cat_ID, Venue_ID, Parent_ID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Level3()
        {
            DateTime StartDate = DateTime.Parse(Request.Form["Start"]);
            DateTime EndDate = DateTime.Parse(Request.Form["End"]);
            Int32 Venue_ID = Convert.ToInt32(Request.Form["Venue_ID"]);
            Int32 Cat_ID = Convert.ToInt32(Session["Cat_ID"]);
            Int32 Performer_ID = Convert.ToInt32(Request.Form["Performer_ID"]);
            IEnumerable<spGet_EventStatus3_Result> model = db.spGet_EventStatus3(StartDate, EndDate, Cat_ID, Performer_ID, Venue_ID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Search()
        {
            DateTime StartDate = DateTime.Parse(Request.Form["Start"]);
            DateTime EndDate = DateTime.Parse(Request.Form["End"]);
            String Search = Request.Form["Search"];
            IEnumerable<spSearch_Performers_Result> model = db.spSearch_Performers(Search, StartDate, EndDate);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchPerformer()
        {
            DateTime StartDate = DateTime.Parse(Request.Form["Start"]);
            DateTime EndDate = DateTime.Parse(Request.Form["End"]);
            Int32 Venue_ID = Convert.ToInt32(Request.Form["Venue_ID"]);
            Int32 Cat_ID = Convert.ToInt32(Request.Form["Cat_ID"]);
            Int32 Performer_ID = Convert.ToInt32(Request.Form["Performer_ID"]);
            IEnumerable<spGet_EventStatus3_Result> model = db.spGet_EventStatus3(StartDate, EndDate, Cat_ID, Performer_ID, Venue_ID);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GenerateReport3(DateTime Start, DateTime End, Int32 Cat_ID, Int32 Performer_ID, Int32 Venue_ID)
        {
            ExcelPackage pck = new ExcelPackage();
            String Worksheet = Start.ToString("MM/dd/yy") + "-" + End.ToString("MM/dd/yy");
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Worksheet);
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet    
            ExcelWorksheetView wv = ws.View;
            wv.ZoomScale = 100;
            String PerformerName = "Missing";
            List<spGet_ESPerformers_Result> N = db.spGet_ESPerformers(Start, End, Cat_ID, Venue_ID).ToList();
            foreach (var item in N)
            {
                if (item.Performer_ID == Performer_ID)
                    PerformerName = item.Performer;
            }
            DataTable dt = new DataTable(); // Read records from database here
            DataColumn[] cols = {new DataColumn("Event_Date", typeof(String)),
                new DataColumn("Event_Name", typeof(String)),
                new DataColumn("Venue_Name", typeof(String)),
                new DataColumn("Section", typeof(String)),
                new DataColumn("Row", typeof(String)),
                new DataColumn("Seats", typeof(String)),
                new DataColumn("Qty", typeof(Int16)),
                new DataColumn("Cost", typeof(Decimal)),
                new DataColumn("Price", typeof(Decimal)),
                new DataColumn("Profit", typeof(Decimal)),
                new DataColumn("TotalCost", typeof(Decimal)),
                new DataColumn("TotalPrice", typeof(Decimal)),
                new DataColumn("TotalProfit", typeof(Decimal)),
                new DataColumn("Vendor", typeof(String)),
                new DataColumn("PO_Date", typeof(DateTime)),
                new DataColumn("Status", typeof(String)),
                new DataColumn("Invoice_ID", typeof(Int32)),
                new DataColumn("Invoice_Date", typeof(DateTime)),
                new DataColumn("Customer", typeof(String)),
                new DataColumn("W/O", typeof(int))};
            dt.Columns.AddRange(cols);
            List<spExport_EventStatus3_Result> model = db.spExport_EventStatus3(Start, End, Cat_ID, Performer_ID, Venue_ID).ToList();
            Int32 Count = model.Count() + 1;
            Int32 Qty = 0;
            Double Cost = 0;
            Double Price = 0;
            Double Profit = 0;
            foreach (var item in model)
            {
                Qty += Convert.ToInt32(item.Qty);
                Cost += Convert.ToDouble(item.Cost * item.Qty);
                Price += Convert.ToDouble(item.Price * item.Qty);
                Profit += Convert.ToDouble((item.Price - item.Cost) * item.Qty);
                dt.Rows.Add(item.Event_Date, item.Event_Name, item.Venue, item.Section, item.Row, item.Seats, item.Qty, item.Cost, item.Price, item.Price - item.Cost, item.Cost * item.Qty, item.Price * item.Qty, item.Qty * (item.Price - item.Cost), item.Vendor, item.PO_Date, item.Status, item.Invoice_ID, item.Invoice_Date, item.Customer, item.Writeoff);
            }
            Int32 plustwo = Count + 1;
            ws.Cells["A1"].LoadFromDataTable(dt, true);
            ws.View.FreezePanes(2, 1);
            ws.Cells["G" + plustwo.ToString()].Value = Qty;
            ws.Cells["G" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Cells["K" + plustwo.ToString()].Value = Cost;
            ws.Cells["K" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Cells["L" + plustwo.ToString()].Value = Price;
            ws.Cells["L" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Cells["M" + plustwo.ToString()].Value = Profit;
            ws.Cells["M" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Column(8).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(9).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(10).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(11).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(12).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(13).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(15).Style.Numberformat.Format = "MM/dd/yyyy";
            ws.Column(18).Style.Numberformat.Format = "MM/dd/yyyy";
            ws.Cells.AutoFitColumns();
            return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", PerformerName + ".xlsx");
        }
        public ActionResult GenerateReport2(DateTime Start, DateTime End, Int32 Cat_ID, Int32 Venue_ID)
        {
            ExcelPackage pck = new ExcelPackage();
            String Worksheet = Start.ToString("MM/dd/yy") + "-" + End.ToString("MM/dd/yy");
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Worksheet);
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet    
            ExcelWorksheetView wv = ws.View;
            wv.ZoomScale = 100;
            String CategoryName = "Missing";
            List<spGet_ESCategories_Result> N = db.spGet_ESCategories(Start, End, 0, Venue_ID).ToList();
            foreach(var item in N)
            {
                if (item.Cat_ID == Cat_ID)
                    CategoryName = item.Category;
            }
            DataTable dt = new DataTable(); // Read records from database here
            DataColumn[] cols = {new DataColumn("Event_Date", typeof(String)),
                new DataColumn("Event_Name", typeof(String)),
                new DataColumn("Venue_Name", typeof(String)),
                new DataColumn("Section", typeof(String)),
                new DataColumn("Row", typeof(String)),
                new DataColumn("Seats", typeof(String)),
                new DataColumn("Qty", typeof(Int16)),
                new DataColumn("Cost", typeof(Decimal)),
                new DataColumn("Price", typeof(Decimal)),
                new DataColumn("Profit", typeof(Decimal)),
                new DataColumn("TotalCost", typeof(Decimal)),
                new DataColumn("TotalPrice", typeof(Decimal)),
                new DataColumn("TotalProfit", typeof(Decimal)),
                new DataColumn("Vendor", typeof(String)),
                new DataColumn("PO_Date", typeof(DateTime)),
                new DataColumn("Status", typeof(String)),
                new DataColumn("Invoice_ID", typeof(Int32)),
                new DataColumn("Invoice_Date", typeof(DateTime)),
                new DataColumn("Customer", typeof(String)),
                new DataColumn("W/O", typeof(int))};
            dt.Columns.AddRange(cols);
            List<spExport_EventStatus2_Result> model = db.spExport_EventStatus2(Start, End, Cat_ID, Venue_ID, 0).ToList();
            Int32 Count = model.Count() + 1;
            Int32 Qty = 0;
            Double Cost = 0;
            Double Price = 0;
            Double Profit = 0;
            foreach (var item in model)
            {
                Qty += Convert.ToInt32(item.Qty);
                Cost += Convert.ToDouble(item.Cost * item.Qty);
                Price += Convert.ToDouble(item.Price * item.Qty);
                Profit += Convert.ToDouble((item.Price - item.Cost) * item.Qty);
                dt.Rows.Add(item.Event_Date, item.Event_Name, item.Venue, item.Section, item.Row, item.Seats, item.Qty, item.Cost, item.Price, item.Price - item.Cost, item.Cost * item.Qty, item.Price * item.Qty, (item.Price - item.Cost) * item.Qty, item.Vendor, item.PO_Date, item.Status, item.Invoice_ID, item.Invoice_Date, item.Customer, item.Writeoff);
            }
            Int32 plustwo = Count + 1;
            ws.Cells["A1"].LoadFromDataTable(dt, true);
            ws.View.FreezePanes(2, 1);
            ws.Cells["G" + plustwo.ToString()].Value = Qty;
            ws.Cells["G" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Cells["K" + plustwo.ToString()].Value = Cost;
            ws.Cells["K" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Cells["L" + plustwo.ToString()].Value = Price;
            ws.Cells["L" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Cells["M" + plustwo.ToString()].Value = Profit;
            ws.Cells["M" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Column(8).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(9).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(10).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(11).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(12).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(13).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(15).Style.Numberformat.Format = "MM/dd/yyyy";
            ws.Column(18).Style.Numberformat.Format = "MM/dd/yyyy";
            ws.Cells.AutoFitColumns();
            return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", CategoryName + ".xlsx");
        }
        public ActionResult GenerateReport(DateTime Start, DateTime End, Int32 Cat_ID, Int32 Venue_ID)
        {
            ExcelPackage pck = new ExcelPackage();
            String Worksheet = Start.ToString("MM/dd/yy") + "-" + End.ToString("MM/dd/yy");
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(Worksheet);
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet    
            ExcelWorksheetView wv = ws.View;
            wv.ZoomScale = 100;
            DataTable dt = new DataTable(); // Read records from database here
            DataColumn[] cols = {new DataColumn("Event_Date", typeof(String)),
                new DataColumn("Category", typeof(String)),
                new DataColumn("Event_Name", typeof(String)),
                new DataColumn("Venue_Name", typeof(String)),
                new DataColumn("Section", typeof(String)),
                new DataColumn("Row", typeof(String)),
                new DataColumn("Seats", typeof(String)),
                new DataColumn("Qty", typeof(Int16)),
                new DataColumn("Cost", typeof(Decimal)),
                new DataColumn("Price", typeof(Decimal)),
                new DataColumn("Profit", typeof(Decimal)),
                new DataColumn("TotalCost", typeof(Decimal)),
                new DataColumn("TotalPrice", typeof(Decimal)),
                new DataColumn("TotalProfit", typeof(Decimal)),
                new DataColumn("Vendor", typeof(String)),
                new DataColumn("PO_Date", typeof(DateTime)),
                new DataColumn("Status", typeof(String)),
                new DataColumn("Invoice_ID", typeof(Int32)),
                new DataColumn("Invoice_Date", typeof(DateTime)),
                new DataColumn("Customer", typeof(String)),
                new DataColumn("W/O", typeof(int))};
            dt.Columns.AddRange(cols);

            List<spExport_EventStatus1_Result> model = db.spExport_EventStatus1(Start, End, Venue_ID, Cat_ID).ToList();
            Int32 Count = model.Count() + 1;
            Int32 Qty = 0;
            Double Cost = 0;
            Double Price = 0;
            Double Profit = 0;
            foreach (var item in model)
            {
                Qty += Convert.ToInt32(item.Qty);
                Cost += Convert.ToDouble(item.Cost * item.Qty);
                Price += Convert.ToDouble(item.Price * item.Qty);
                Profit += Convert.ToDouble((item.Price - item.Cost) * item.Qty);
                dt.Rows.Add(item.Event_Date, item.Category, item.Event_Name, item.Venue, item.Section, item.Row, item.Seats, item.Qty, item.Cost, item.Price, item.Price - item.Cost, item.Cost * item.Qty, item.Price * item.Qty, (item.Price - item.Cost) * item.Qty, item.Vendor, item.PO_Date, item.Status, item.Invoice_ID, item.Invoice_Date, item.Customer, item.Writeoff);
            }
            Int32 plustwo = Count + 1;
            ws.Cells["A1"].LoadFromDataTable(dt, true);
            ws.View.FreezePanes(2, 1);
            ws.Cells["H" + plustwo.ToString()].Value = Qty;
            ws.Cells["H" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Cells["L" + plustwo.ToString()].Value = Cost;
            ws.Cells["L" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Cells["M" + plustwo.ToString()].Value = Price;
            ws.Cells["M" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Cells["N" + plustwo.ToString()].Value = Profit;
            ws.Cells["N" + plustwo.ToString()].Style.Font.Bold = true;
            ws.Column(9).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(10).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(11).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(12).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(13).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(14).Style.Numberformat.Format = "$#,##0.00";
            ws.Column(16).Style.Numberformat.Format = "MM/dd/yyyy";
            ws.Column(19).Style.Numberformat.Format = "MM/dd/yyyy";
            ws.Cells.AutoFitColumns();
            return File(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EventStatus" + ".xlsx");
        }
    }
}
