using EventsApp.DataModels;
using EventsApp.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EventsApp.Controllers
{
    public class EventController : Controller
    {
        private EventContext db = new EventContext();
        private IEventUnitOfWork eventUoW;

        public EventController(IEventUnitOfWork eventUoW)
        {
            this.eventUoW = eventUoW;
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

                eventUoW.Events.Attach(e);
                eventUoW.Save();

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
            var publicEvents = eventUoW.Events.GetAllPublicEvents();
            if (User.Identity.IsAuthenticated)
            {
                var user = eventUoW.Users.GetUserById(User.Identity.GetUserId());
                var hostedEvents = eventUoW.Events.GetAllCreatedEvents(user);
                var invitedEvents = eventUoW.Events.GetAllInvitedEvents(user);

                // Prioritize in order (hosted, invited, public) for setting a user-event relation.
                foreach (var e in hostedEvents)
                {
                    events.Add(new ViewEvent { Id = e.Id, Brief = e.Brief, Detailed = e.Detailed, Address = e.Address, Latitude = e.Latitude, Longitude = e.Longitude, StartTime = e.StartTime, Visibility = e.Visibility, Relation = EventUserRelation.Hosted });
                    invitedEvents.Remove(e);
                    publicEvents.Remove(e);
                }

                foreach (var e in invitedEvents)
                {
                    events.Add(new ViewEvent { Id = e.Id, Brief = e.Brief, Detailed = e.Detailed, Address = e.Address, Latitude = e.Latitude, Longitude = e.Longitude, StartTime = e.StartTime, Visibility = e.Visibility, Relation = EventUserRelation.Invited });
                    publicEvents.Remove(e);
                }
            }

            foreach (var e in publicEvents)
            {
                events.Add(new ViewEvent { Id = e.Id, Brief = e.Brief, Detailed = e.Detailed, Address = e.Address, Latitude = e.Latitude, Longitude = e.Longitude, StartTime = e.StartTime, Visibility = e.Visibility, Relation = EventUserRelation.Public });
            }

            return Json(events);
        }


        [Authorize]
        public ActionResult CreatedEvents()
        {
            var user = eventUoW.Users.GetUserById(User.Identity.GetUserId());
            var events = new List<Event>();
            events = eventUoW.Events.GetAllCreatedEvents(user).ToList();
            return View(events);
        }

        [Authorize]
        public ActionResult EventDetails()
        {
            return View();
        }


        [Authorize]
        public ActionResult ManageEvent(int eventId)
        {
            return View(new ManageEventViewModel { EventId = eventId });
        }

        [Authorize]
        public async Task<ActionResult> EditEvent(int? eventId)
        {
            if (eventId == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event _event = await db.Events.FindAsync(eventId);
            if (_event == null)
            {
                return HttpNotFound();
            }
            return View(_event);
        }


        [HttpPost]
        public async Task<ActionResult> EditEvent([Bind(Include = "Brief, Detailed, Address, Latitude, Longitude, StartTime, Visibility")] Event _event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(_event).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(_event);
        }

        [Authorize]
        [HttpPost]
        public ActionResult GenerateLink(int eventId)
        {
            // Generate a new invitation link.
            Guid code = Guid.NewGuid();
            InviteLink invite = new InviteLink { EventId = eventId, LinkGUID = code.ToString(), ModificationState = ModificationState.Added };
            eventUoW.InviteLinks.Attach(invite);
            eventUoW.Save();

            // Create a link to this event.
            UrlHelper url = new UrlHelper(HttpContext.Request.RequestContext);
            string link = url.Action("ConfirmLink", "Event", new { code = code.ToString() }, url.RequestContext.HttpContext.Request.Url.Scheme);

            return Json(new { link = link });
        }

        public ActionResult ConfirmLink(string code)
        {
            InviteLink invite = eventUoW.InviteLinks.GetLinkGraphByCode(code);

            if (invite != null)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return View("Confirm", new ConfirmViewModel { Link = invite, LinkGUID = invite.LinkGUID });
                }

                if (invite.Event.OwnerId == User.Identity.GetUserId())
                {
                    return View("ConfirmOwner", new ConfirmViewModel { Link = invite, LinkGUID = invite.LinkGUID });
                }

                AppUser user = eventUoW.Users.GetUserById(User.Identity.GetUserId());
                if (eventUoW.Invites.IsInvited(invite.Event, user))
                {
                    return View("ConfirmInvited", new ConfirmViewModel { Link = invite, LinkGUID = invite.LinkGUID });
                }

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
            InviteLink link = eventUoW.InviteLinks.GetLinkGraphByCode(model.LinkGUID);
            if (link == null)
                throw new InvalidOperationException();

            // Create an invite.
            Invite invite = new Invite { AppUserId = User.Identity.GetUserId(), Event = link.Event, Status = InviteStatus.Accepted, ModificationState = ModificationState.Added };
            eventUoW.Invites.Attach(invite);

            // Remove the invitation link (it is a one-time use after all).
            eventUoW.InviteLinks.Remove(link);
            eventUoW.Save();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Reject(ConfirmViewModel model)
        {
            // Remove invite link.
            InviteLink link = eventUoW.InviteLinks.GetLinkGraphByCode(model.LinkGUID);
            if (link == null)
                throw new InvalidOperationException();
            eventUoW.InviteLinks.Remove(link);
            eventUoW.Save();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult DeactivateLink(ConfirmViewModel model)
        {
            // Remove invite link.
            InviteLink link = eventUoW.InviteLinks.GetLinkGraphByCode(model.LinkGUID);
            if (link == null)
                throw new InvalidOperationException();
            eventUoW.InviteLinks.Remove(link);
            eventUoW.Save();

            return RedirectToAction("Index", "Home");
        }
    }

}