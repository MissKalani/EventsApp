using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventsApp.Controllers
{
    public class AuthController : Controller
    {
        public static bool IsAuthenticated = false;

        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn()
        {
            IsAuthenticated = true;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOut()
        {
            IsAuthenticated = false;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }
    }
}