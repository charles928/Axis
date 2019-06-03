using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axis.Controllers
{
    public class ReferenceController : Controller
    {
        private AxisEntities db = new AxisEntities();
        // GET: Reference
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

                List<spGet_ESPerformers_Result> P = db.spGet_ESPerformers(DateTime.Now, DateTime.Now.AddYears(1), 0, 0).ToList();
                var PerformerList = new SelectList(P, "Performer_ID", "Performer").ToList();
                ViewData["PerformerList"] = PerformerList;

                List<spGet_Exceptions_Result> X = db.spGet_Exceptions().ToList();
                ViewBag.ETotal = X.Count.ToString() + " Records";
                ViewData["Exceptions"] = X;

                List<spGet_MissingFromStubhub_Result> M = db.spGet_MissingFromStubhub().ToList();
                ViewBag.MTotal = M.Count.ToString() + " Events Listed";
                ViewBag.MListings = String.Format("{0:###,###}", M.Sum(item => item.Listings));
                ViewBag.MTickets = String.Format("{0:###,###}", M.Sum(item => item.Tickets));
                ViewBag.MCost = String.Format("{0:c}", M.Sum(item => item.Cost));
                ViewBag.MPrice = String.Format("{0:c}", M.Sum(item => item.Price));
                ViewData["Missing"] = M;

                List<spGet_714List_Result> model = db.spGet_714List().ToList();
                ViewBag.STotal = model.Count.ToString() + " Records";
                return View(model);
            }
        }
        public ActionResult NewException(Int32 Performer_ID, String Exception)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            db.spException_Add(Performer_ID, Exception, Login_ID);
            return Content("success");
        }
        public ActionResult RemoveException(Int32 Exception_ID)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            db.spException_Remove(Exception_ID, Login_ID);
            return Content("success");
        }
        public ActionResult NewBrokerShare(String Name, String Code)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            List<sp714_AddShare_Result> A = db.sp714_AddShare(Name, Code, Login_ID).ToList();
            return Content(A[0].Return.ToString());
        }
        public ActionResult RemoveBrokerShare(Int32 Code_ID)
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            db.sp714_RemoveShare(Code_ID, Login_ID);
            return Content("success");
        }
    }
}