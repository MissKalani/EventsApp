using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventsApp.Controllers
{
    public class EventController : Controller
    {
        // GET: Event
        public ActionResult Create()
        {
            return View();
        }
    }
}