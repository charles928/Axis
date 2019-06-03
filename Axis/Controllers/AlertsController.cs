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
    public class AlertsController : Controller
    {
        private AxisEntities db = new AxisEntities();

        // GET: Alerts
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
    }
}
