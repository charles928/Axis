using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axis.Controllers
{
    public class PurchaseController : Controller
    {
        private AxisEntities db = new AxisEntities();
        // GET: Purchase
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

                String sStart;
                DateTime dStart;
                if (string.IsNullOrEmpty(Session["StartDate5"] as string))
                {
                    dStart = DateTime.Now;
                    sStart = dStart.ToString("MM/dd/yyyy");
                    Session["StartDate5"] = sStart;
                } else {
                    sStart = Session["StartDate5"].ToString();
                    dStart = DateTime.Parse(sStart);
                }
                String sEnd;
                DateTime dEnd;
                if (string.IsNullOrEmpty(Session["EndDate5"] as string))
                {
                    dEnd = DateTime.Now;
                    sEnd = dEnd.ToString("MM/dd/yyyy");
                    Session["EndDate5"] = sEnd;
                } else {
                    sEnd = Session["EndDate5"].ToString();
                    dEnd = DateTime.Parse(sEnd);
                }
                Boolean bAllDates;
                if (!string.IsNullOrEmpty(Session["AllDates5"] as string))
                    bAllDates = Convert.ToBoolean(Session["AllDates5"]);
                else
                    bAllDates = false;

                Boolean bEventOnly;
                if (!string.IsNullOrEmpty(Session["EventOnly5"] as string))
                    bEventOnly = Convert.ToBoolean(Session["EventOnly5"]);
                else
                    bEventOnly = false;
                Boolean bVendorOnly;
                if (!string.IsNullOrEmpty(Session["VendorOnly5"] as string))
                    bVendorOnly = Convert.ToBoolean(Session["VendorOnly5"]);
                else
                    bVendorOnly = false;
                Boolean bAgentOnly;
                if (!string.IsNullOrEmpty(Session["AgentOnly5"] as string))
                    bAgentOnly = Convert.ToBoolean(Session["AgentOnly5"]);
                else
                    bAgentOnly = false;
                Boolean bVoidsOnly;
                if (!string.IsNullOrEmpty(Session["VoidsOnly5"] as string))
                    bVoidsOnly = Convert.ToBoolean(Session["VoidsOnly5"]);
                else
                    bVoidsOnly = false;

                String sSearch;
                if (string.IsNullOrEmpty(Session["Search5"] as string))
                    sSearch = "";
                else
                    sSearch = Session["Search5"].ToString();

                List<spGet_POList_Result> C = db.spGet_POList(dStart, dEnd, bAllDates, bEventOnly, bVendorOnly, bAgentOnly, bVoidsOnly, sSearch).ToList();
                ViewBag.FooterTitle = C.Count().ToString() + " Records";

                Int32 QUnsold = Convert.ToInt32(C.Sum(item => item.Status=="Consign" ? 0 : item.Status=="Void" ? 0 : item.Unsold));
                Int32 QSold = Convert.ToInt32(C.Sum(item => item.Status == "Consign" ? 0 : item.Status=="Void" ? 0 : item.Sold));
                Decimal QUnsoldCost = Convert.ToDecimal(C.Sum(item => item.Status == "Consign" ? 0 : item.Status=="Void" ? 0 : item.UnsoldCost));
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
                } else
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
                        } else
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

                ViewBag.Profit = String.Format("{0:c}",QSoldPrice - QSoldCost);
                ViewBag.ProjectedProfit = String.Format("{0:c}", QUnsoldPrice - QUnsoldCost);

                ViewBag.TotalQty = String.Format("{0:###,###}", QSold + QUnsold);
                ViewBag.TotalRevenue = String.Format("{0:c}", QSoldPrice + QUnsoldPrice);

                ViewBag.PercentSold = String.Format("{0:P}", QPercentSold);

                ViewBag.AvgPrice = String.Format("{0:c}", QAvgPrice);
                ViewBag.AvgCost = String.Format("{0:c}", QAvgCost);

                String tStart = DateTime.Now.ToString("MM/dd/yyyy");

                if (sStart == tStart && sEnd == tStart && bAllDates == false && bEventOnly == false && bVendorOnly == false && bAgentOnly == false && bVoidsOnly == false && sSearch == "")
                    ViewBag.Changed = "False";
                else
                    ViewBag.Changed = "True";

                    return View(C);
            }
        }
        public ActionResult UpdateValues()
        {
            String sStart = Request.Form["Begin"];
            if (sStart == "NULL")
                 sStart = DateTime.Now.ToString("MM/dd/yyyy");
            Session["StartDate5"] = sStart;

            String sEnd = Request.Form["End"];
            if (sEnd == "NULL")
                sEnd = DateTime.Now.ToString("MM/dd/yyyy");
            Session["EndDate5"] = sEnd;

            String sAllDates = Request.Form["AllDates"];
            Session["AllDates5"] = sAllDates;

            String sEventOnly = Request.Form["EventOnly"];
            Session["EventOnly5"] = sEventOnly;
            String sVendorOnly = Request.Form["VendorOnly"];
            Session["VendorOnly5"] = sVendorOnly;
            String sAgentOnly = Request.Form["AgentOnly"];
            Session["AgentOnly5"] = sAgentOnly;
            String sVoidsOnly = Request.Form["VoidsOnly"];
            Session["VoidsOnly5"] = sVoidsOnly;

            String sSearch = Request.Form["Search"];
            Session["Search5"] = sSearch;

            return Content("success");
        }
        public ActionResult Search2(Int32 PO_ID)
        {
            List<spGet_POSearch_Result> C = db.spGet_POSearch(PO_ID).ToList();
            return Json(C, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPO(Int32 PO_ID)
        {
            ViewBag.PO_ID = PO_ID;

            List<spGet_POPayment_Result> P = db.spGet_POPayment(PO_ID).ToList();
            Decimal TotalPaid = Convert.ToDecimal(P.Sum(item => item.amt));
            ViewBag.TotalPaid = String.Format("{0:c}", TotalPaid);
            ViewData["Payment"] = P;

            List<spGet_POInfo_Result> I = db.spGet_POInfo(PO_ID).ToList();
            ViewBag.Status = I[0].Status;
            ViewBag.Company = I[0].client_broker_company_name;
            ViewBag.Address1 = I[0].street_address1;
            ViewBag.Address2 = I[0].street_address2;
            ViewBag.Address3 = I[0].city + ", " + I[0].state_short_name + "  " + I[0].postal_code;
            ViewBag.Address4 = I[0].phone_number;
            ViewBag.Address5 = I[0].client_broker_email;
            ViewBag.Created = I[0].create_date;
            ViewBag.ExternalPO = I[0].external_po_number;
            ViewBag.Agent = I[0].system_user_fname + " " + I[0].system_user_lname;
            ViewBag.ShipMethod = I[0].shipping_account_type_name;
            ViewBag.ShipType = I[0].shipping_account_delivery_method_desc;
            ViewBag.Tracking = I[0].shipping_tracking_number;
            ViewBag.Arrival = I[0].estimated_arrival_date;
            ViewBag.Shipping = @String.Format("{0:c}", I[0].total_shipping);
            ViewBag.Expenses = @String.Format("{0:c}", I[0].total_expenses);
            ViewBag.TotalDue = String.Format("{0:c}", I[0].total_due);
            ViewBag.Taxes = @String.Format("{0:c}", I[0].total_taxes);
            ViewBag.POTotal = @String.Format("{0:c}", I[0].po_total);
            ViewBag.BalanceDue = @String.Format("{0:c}", I[0].balance_due);
            ViewBag.PONotes = I[0].displayed_notes;
            ViewBag.ShippingNotes = I[0].shipping_notes;
            ViewBag.TicketCost = @String.Format("{0:c}", I[0].TicketCost);
            ViewBag.SoldPrice = String.Format("{0:c}", I[0].SoldPrice);
            ViewBag.Tickets = I[0].Tickets;
            ViewBag.SettledDate = I[0].settled_date;
            ViewBag.VendorPrepayment = String.Format("{0:c}", I[0].VendorPrepayment);
            ViewBag.Closeout = I[0].closeout;

            if (TotalPaid == I[0].po_total && TotalPaid > 0) //Paid
                ViewBag.Stamp = "PAID";

            List<spGet_POVendor_Result> V = db.spGet_POVendor(PO_ID).ToList();
            ViewBag.VCompany = V[0].Company;
            if (V[0].Company != V[0].Name)
                ViewBag.VName = V[0].Name;
            ViewBag.VAddress1 = V[0].street_address1;
            ViewBag.VAddress2 = V[0].street_address2;
            ViewBag.VAddress3 = V[0].city + ", " + V[0].state_short_name + "  " + V[0].postal_code;
            ViewBag.Vendor_ID = V[0].vendor_id;
            ViewBag.VendorContact = V[0].employee_fname + " " + V[0].employee_lname;
            ViewBag.VendorPhone = V[0].employee_phone;
            ViewBag.VendorEmail = V[0].employee_email;

            List<spGet_POItems_Result> model = db.spGet_POItems(PO_ID).ToList();
            return PartialView(model);
        }
    }
}