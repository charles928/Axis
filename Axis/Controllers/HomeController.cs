using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Axis.Controllers
{
    public class HomeController : Controller
    {
        private AxisEntities db = new AxisEntities();
        public ActionResult Index()
        {
            DateTime Start = DateTime.Now;
            DateTime End = DateTime.Now.AddDays(31);
            Session["Login_ID"] = 0;
            Session["StartDate"] = Start.ToString("MM/dd/yyyy");
            Session["EndDate"] = End.ToString("MM/dd/yyyy");

            List<spUser_GetLogin_Result> A = db.spUser_GetLogin(0).ToList();
            ViewData["Photo"] = @"/Images/" + A[0].Photo;
            ViewData["Name"] = A[0].FullName;
            return View();
        }

        public ActionResult Login()
        {
            String UserName = Request.Form["UserName"];
            String Password = Request.Form["Password"];
            List<spUser_Login_Result> A = db.spUser_Login(UserName, Password).ToList();
            Session["SHMarkup"] = A[0].SHMarkup;
            if (A[0].Return == "Success")
            {
                Session["Login_ID"] = A[0].Login_ID;
            }
            return Content(A[0].Return); // return error
        }
        public ActionResult Account()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            if (Login_ID > 0)
            {
                ViewBag.Message = "My Account";
                IEnumerable<spUser_GetLogin_Result> model = db.spUser_GetLogin(Login_ID);
                return View("Account", model);
            }
            else
            {
                Session["UserName"] = "Not Logged In";
                Session["Login_ID"] = 0;
                return View("Index");
            }

        }
        public ActionResult AccountUpdate()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            String Field = Request.Form["Field"];
            String OldValue = Request.Form["OldValue"];
            String NewValue = Request.Form["NewValue"];
            db.spUser_Update(Login_ID, Field, OldValue, NewValue);
            return Content("success");
        }
        public ActionResult CreateAccount()
        {
            Int32 Login_ID = Convert.ToInt32(Session["Login_ID"]);
            String Name = Request.Form["Name"];
            String Email = Request.Form["Email"];
            String Password = Request.Form["Password"];
            Boolean Admin = Convert.ToBoolean(Request.Form["Admin"]);
            db.spUser_CreateLogin(Login_ID, Name, Email, Password, Admin);
            return Content("success");
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

}