using EventsApp.DataAccess;
using EventsApp.DataModels;
using EventsApp.MVC.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EventsApp.MVC.Controllers
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
        public ActionResult Create(HellViewModel model)
        {
            if (ModelState.IsValid)
            {
                Event e = new Event();
                e.Brief = model.CreateViewModel.Brief;
                e.Detailed = model.CreateViewModel.Detailed;
                e.Visibility = model.CreateViewModel.Visibility;
                e.Address = model.CreateViewModel.Address;
                e.Latitude = model.CreateViewModel.Latitude;
                e.Longitude = model.CreateViewModel.Longitude;
                e.StartTime = model.CreateViewModel.StartTime;
                e.ModificationState = ModificationState.Added;
                e.OwnerId = User.Identity.GetUserId();

                eventUoW.Events.Attach(e);
                eventUoW.Save();

                return RedirectToAction("Details", "Event", new { id = e.Id });
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
        public ActionResult Details(int id)
        {
            var e = eventUoW.Events.GetEventByID(id);
            return View(new HellViewModel { Event = e });
        }


        //    [Authorize]
        //    public ActionResult ManageEvent(int eventId)
        //    {
        //        return View(new HellViewModel { ManageUserViewModel new ManageEventViewModel { EventId = eventId });
        //    }

        [Authorize]
        public ActionResult Edit(int id)
        {          
            Event _event = eventUoW.Events.GetEventByID(id);
            if (_event == null)
            {
                return HttpNotFound();
            }
            return View(new HellViewModel { Event = _event });
        }


        [HttpPost]
        public ActionResult Edit(HellViewModel model)
        {
            if (ModelState.IsValid)
            {
                Event e = eventUoW.Events.GetEventByID(model.Event.Id);
                e.Brief = model.Event.Brief;
                e.Detailed = model.Event.Detailed;
                e.Visibility = model.Event.Visibility;
                e.Address = model.Event.Address;
                e.Latitude = model.Event.Latitude;
                e.Longitude = model.Event.Longitude;
                e.StartTime = model.Event.StartTime;
                e.ModificationState = ModificationState.Modified;

                eventUoW.Events.Attach(e);
                eventUoW.Save();

                return RedirectToAction("Details", "Event", new { id = e.Id });
            }
            return View(model);
        }

        //    [Authorize]
        //    [HttpPost]
        //    public ActionResult GenerateLink(int eventId)
        //    {
        //        // Generate a new invitation link.
        //        Guid code = Guid.NewGuid();
        //        InviteLink invite = new InviteLink { EventId = eventId, LinkGUID = code.ToString(), ModificationState = ModificationState.Added };
        //        eventUoW.InviteLinks.Attach(invite);
        //        eventUoW.Save();

        //        // Create a link to this event.
        //        UrlHelper url = new UrlHelper(HttpContext.Request.RequestContext);
        //        string link = url.Action("ConfirmLink", "Event", new { code = code.ToString() }, url.RequestContext.HttpContext.Request.Url.Scheme);

        //        return Json(new { link = link });
        //    }

        //    public ActionResult ConfirmLink(string code)
        //    {
        //        InviteLink invite = eventUoW.InviteLinks.GetLinkGraphByCode(code);

        //        if (invite != null)
        //        {
        //            if (!User.Identity.IsAuthenticated)
        //            {
        //                return View("Confirm", new ConfirmViewModel { Link = invite, LinkGUID = invite.LinkGUID });
        //            }

        //            if (invite.Event.OwnerId == User.Identity.GetUserId())
        //            {
        //                return View("ConfirmOwner", new ConfirmViewModel { Link = invite, LinkGUID = invite.LinkGUID });
        //            }

        //            AppUser user = eventUoW.Users.GetUserById(User.Identity.GetUserId());
        //            if (eventUoW.Invites.IsInvited(invite.Event, user))
        //            {
        //                return View("ConfirmInvited", new ConfirmViewModel { Link = invite, LinkGUID = invite.LinkGUID });
        //            }

        //            return View("Confirm", new ConfirmViewModel { Link = invite, LinkGUID = invite.LinkGUID });
        //        }
        //        else
        //        {
        //            return new HttpNotFoundResult();
        //        }
        //    }

        //    [Authorize]
        //    [HttpPost]
        //    public ActionResult Accept(ConfirmViewModel model)
        //    {
        //        // Get the invite link.
        //        InviteLink link = eventUoW.InviteLinks.GetLinkGraphByCode(model.LinkGUID);
        //        if (link == null)
        //            throw new InvalidOperationException();

        //        // Create an invite.
        //        Invite invite = new Invite { AppUserId = User.Identity.GetUserId(), Event = link.Event, Status = InviteStatus.Accepted, ModificationState = ModificationState.Added };
        //        eventUoW.Invites.Attach(invite);

        //        // Remove the invitation link (it is a one-time use after all).
        //        eventUoW.InviteLinks.Remove(link);
        //        eventUoW.Save();

        //        return RedirectToAction("Index", "Home");
        //    }

        //    [HttpPost]
        //    public ActionResult Reject(ConfirmViewModel model)
        //    {
        //        // Remove invite link.
        //        InviteLink link = eventUoW.InviteLinks.GetLinkGraphByCode(model.LinkGUID);
        //        if (link == null)
        //            throw new InvalidOperationException();
        //        eventUoW.InviteLinks.Remove(link);
        //        eventUoW.Save();

        //        return RedirectToAction("Index", "Home");
        //    }

        //    [HttpPost]
        //    public ActionResult DeactivateLink(ConfirmViewModel model)
        //    {
        //        // Remove invite link.
        //        InviteLink link = eventUoW.InviteLinks.GetLinkGraphByCode(model.LinkGUID);
        //        if (link == null)
        //            throw new InvalidOperationException();
        //        eventUoW.InviteLinks.Remove(link);
        //        eventUoW.Save();

        //        return RedirectToAction("Index", "Home");
        //    }
    }

}