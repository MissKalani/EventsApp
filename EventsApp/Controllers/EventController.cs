using EventsApp.DataModels;
using EventsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventsApp.Controllers
{
    public class EventController : Controller
    {
        private IEventRepository repository;

        public EventController(IEventRepository repository)
        {
            this.repository = repository;
        }

        // GET: Event
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Event e = new Event();
                e.Brief = model.Brief;
                e.Detailed = model.Detailed;
                e.Address = model.Address;
                e.Latitude = model.Latitude;
                e.Longitude = model.Longitude;
                e.StartTime = model.StartTime;
                e.ModificationState = ModificationState.Added;

                repository.Attach(e);
                repository.Save();

                // TODO: Redirect wherever is proper.
                return View("Index", "Home");
            }

            return View();
        }
    }
}