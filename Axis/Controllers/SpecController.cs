using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axis.Controllers
{
    public class SpecController : Controller
    {
        private AxisEntities db = new AxisEntities();
        // GET: Spec
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

                List<spGet_Spec_Result> model = db.spGet_Spec(iParent_ID, iChild_ID, iGrandchild_ID, iRange).ToList();
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
        public ActionResult GetEventsForGrandchild(Int32 Parent_ID, Int32 Child_ID, Int32 Grandchild_ID, Int32 Range)
        {
            List<spGet_SpecEventsForGrandchild_Result> model = db.spGet_SpecEventsForGrandchild(Parent_ID, Child_ID, Grandchild_ID, Range).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}