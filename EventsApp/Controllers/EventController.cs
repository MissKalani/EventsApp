using EventsApp.DataModels;
using EventsApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace EventsApp.Controllers
{
    public class EventController : Controller
    {
        private IEventRepository eventRepository;
        private IUserRepository userRepository;

        public EventController(IEventRepository eventRepository, IUserRepository userRepository)
        {
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
        }

        // GET: Event
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Event e = new Event();
                e.Brief = model.Brief;
                e.Detailed = model.Detailed;               
                e.Visibility = model.Visibility;
                e.Address = model.Address;
                e.Latitude = model.Latitude;
                e.Longitude = model.Longitude;
                e.StartTime = model.StartTime;
                e.ModificationState = ModificationState.Added;
                e.OwnerId = User.Identity.GetUserId();

                eventRepository.Attach(e);
                eventRepository.Save();

                // TODO: Redirect to event management page?
                return RedirectToAction("Manage", "Event");
            }

            return View();
        }

        /// <summary>
        /// Returns a JSON formatted string with all visible events (i.e. public events + those that we created and is invited to).
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Visible()
        {
            // Add all public events to the list regardless whether the user is logged in or not.
            List<Event> events = eventRepository.GetAllPublicEvents();

            if (User.Identity.IsAuthenticated)
            {
                var user = userRepository.GetUserById(User.Identity.GetUserId());

                // Add all events that the user created.
                var hostedEvents = eventRepository.GetAllCreatedEvents(user);
                events = events.Union(eventRepository.GetAllCreatedEvents(user)).ToList();

                // Add all events that the user is invited to.
                events = events.Union(eventRepository.GetAllInvitedEvents(user)).ToList();
            }

            return Json(events);
        }


        [Authorize]
        public ActionResult CreatedEvents()
        {
            var user = userRepository.GetUserById(User.Identity.GetUserId());
            var events = new List<Event>();
            events = eventRepository.GetAllCreatedEvents(user).ToList(); 
            return View(events);
        }


        [Authorize]
        public ActionResult ManageEvent()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ManageEvent(ManageEventViewModel model)
        {
            
            
            return View();
        }
    }
   
}