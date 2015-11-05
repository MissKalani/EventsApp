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
        private IInviteLinkRepository linkRepository;
        private IInviteRepository inviteRepository;

        public EventController(IEventRepository eventRepository, IUserRepository userRepository, IInviteLinkRepository linkRepository, IInviteRepository inviteRepository)
        {
            this.eventRepository = eventRepository;
            this.userRepository = userRepository;
            this.linkRepository = linkRepository;
            this.inviteRepository = inviteRepository;
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
            List<ViewEvent> events = new List<ViewEvent>();
            var publicEvents = eventRepository.GetAllPublicEvents();
            if (User.Identity.IsAuthenticated)
            {
                var user = userRepository.GetUserById(User.Identity.GetUserId());
                var hostedEvents = eventRepository.GetAllCreatedEvents(user);
                var invitedEvents = eventRepository.GetAllInvitedEvents(user);

                // Prioritize in order (hosted, invited, public) for setting a user-event relation.
                foreach (var e in hostedEvents)
                {
                    events.Add(new ViewEvent { Brief = e.Brief, Detailed = e.Detailed, Address = e.Address, Latitude = e.Latitude, Longitude = e.Longitude, StartTime = e.StartTime, Relation = EventUserRelation.Hosted });
                    invitedEvents.Remove(e);
                    publicEvents.Remove(e);
                }

                foreach (var e in invitedEvents)
                {
                    events.Add(new ViewEvent { Brief = e.Brief, Detailed = e.Detailed, Address = e.Address, Latitude = e.Latitude, Longitude = e.Longitude, StartTime = e.StartTime, Relation = EventUserRelation.Invited });
                    publicEvents.Remove(e);
                }
            }
            
            foreach (var e in publicEvents)
            {
                events.Add(new ViewEvent { Brief = e.Brief, Detailed = e.Detailed, Address = e.Address, Latitude = e.Latitude, Longitude = e.Longitude, StartTime = e.StartTime, Relation = EventUserRelation.Public });
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
        public ActionResult ManageEvent(int eventId)
        {
            return View(new ManageEventViewModel { EventId = eventId });
        }

        [Authorize]
        [HttpPost]
        public ActionResult ManageEvent(ManageEventViewModel model)
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult GenerateLink(int eventId)
        {
            // Generate a new invitation link.
            Guid code = Guid.NewGuid();
            InviteLink invite = new InviteLink { EventId = eventId, LinkGUID = code.ToString(), ModificationState = ModificationState.Added };
            linkRepository.Attach(invite);
            linkRepository.Save();

            // Create a link to this event.
            UrlHelper url = new UrlHelper(HttpContext.Request.RequestContext);
            string link = url.Action("ConfirmLink", "Event", new { code = code.ToString() }, url.RequestContext.HttpContext.Request.Url.Scheme);

            return Json(new { link = link });
        }

        public ActionResult ConfirmLink(string code)
        {
            InviteLink invite = linkRepository.GetLinkGraphByCode(code);
         
            if (invite != null)
            {
                return View("Confirm", new ConfirmViewModel { Link = invite, LinkGUID = invite.LinkGUID });
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Accept(ConfirmViewModel model)
        {
            // Get the invite link.
            InviteLink link = linkRepository.GetLinkGraphByCode(model.LinkGUID);
            if (link == null)
                throw new InvalidOperationException();

            // Create an invite.
            Invite invite = new Invite { AppUserId = User.Identity.GetUserId(), Event = link.Event, Status = InviteStatus.Accepted, ModificationState = ModificationState.Added };
            inviteRepository.Attach(invite);
            inviteRepository.Save();

            // Remove the invitation link (it is a one-time use after all).
            linkRepository.Remove(model.Link);
            linkRepository.Save();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Reject(ConfirmViewModel model)
        {
            // TODO: Remove invite link.
            return RedirectToAction("Index", "Home");
        }
    }
   
}