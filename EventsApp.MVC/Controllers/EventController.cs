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
            List<EventListViewModel> events = new List<EventListViewModel>();
            var publicEvents = eventUoW.Events.GetAllPublicEvents();
            if (User.Identity.IsAuthenticated)
            {
                var user = eventUoW.Users.GetUserById(User.Identity.GetUserId());
                var hostedEvents = eventUoW.Events.GetAllCreatedEvents(user);
                var invitedEvents = eventUoW.Events.GetAllInvitedEvents(user);

                // Prioritize in order (hosted, invited, public) for setting a user-event relation.
                foreach (var e in hostedEvents)
                {
                    var host = eventUoW.Events.LoadUserGraph(e);
                    events.Add(new EventListViewModel { Id = e.Id, Brief = e.Brief, Detailed = e.Detailed, HostName = host.UserName, Address = e.Address, Latitude = e.Latitude, Longitude = e.Longitude, StartTime = e.StartTime, Visibility = e.Visibility, Relation = EventUserRelation.Hosted });
                    invitedEvents.Remove(e);
                    publicEvents.Remove(e);
                }

                foreach (var e in invitedEvents)
                {
                    var host = eventUoW.Events.LoadUserGraph(e);
                    events.Add(new EventListViewModel { Id = e.Id, Brief = e.Brief, Detailed = e.Detailed, HostName = host.UserName, Address = e.Address, Latitude = e.Latitude, Longitude = e.Longitude, StartTime = e.StartTime, Visibility = e.Visibility, Relation = EventUserRelation.Invited });
                    publicEvents.Remove(e);
                }
            }

            foreach (var e in publicEvents)
            {
                var host = eventUoW.Events.LoadUserGraph(e);
                events.Add(new EventListViewModel { Id = e.Id, Brief = e.Brief, Detailed = e.Detailed, HostName = host.UserName, Address = e.Address, Latitude = e.Latitude, Longitude = e.Longitude, StartTime = e.StartTime, Visibility = e.Visibility, Relation = EventUserRelation.Public });
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

        public ActionResult Details(int id)
        {
            var vm = new DetailsViewModel();
            vm.Event = eventUoW.Events.GetEventByID(id);

            if (vm.Event == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            vm.Event.AppUser = eventUoW.Users.GetUserById(vm.Event.OwnerId);
            vm.InvitedUsers = eventUoW.Events.GetInvitedUsers(vm.Event);
            vm.IsOwner = User.Identity.IsAuthenticated && User.Identity.GetUserId() == vm.Event.OwnerId;

            return View(new HellViewModel { DetailsViewModel = vm });
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
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (_event.OwnerId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var vm = new EditViewModel { EventId = _event.Id, Brief = _event.Brief, Detailed = _event.Detailed, Address = _event.Address, Latitude = _event.Latitude, Longitude = _event.Longitude, StartTime = _event.StartTime, Visibility = _event.Visibility };
            return View(new HellViewModel { EditViewModel = vm });
        }


        [Authorize]
        [HttpPost]
        public ActionResult Edit(HellViewModel model)
        {
            if (ModelState.IsValid)
            {
                Event e = eventUoW.Events.GetEventByID(model.EditViewModel.EventId);

                if (e == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }

                if (e.OwnerId != User.Identity.GetUserId())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                e.Brief = model.EditViewModel.Brief;
                e.Detailed = model.EditViewModel.Detailed;
                e.Visibility = model.EditViewModel.Visibility;
                e.Address = model.EditViewModel.Address;
                e.Latitude = model.EditViewModel.Latitude;
                e.Longitude = model.EditViewModel.Longitude;
                e.StartTime = model.EditViewModel.StartTime;
                e.ModificationState = ModificationState.Modified;

                eventUoW.Events.Attach(e);
                eventUoW.Save();

                return RedirectToAction("Details", "Event", new { id = e.Id });
            }

            return View(model);
        }

        [Authorize]
        public ActionResult Delete(int id)
        {
            Event e = eventUoW.Events.GetEventByID(id);
            if (e == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (e.OwnerId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(new HellViewModel { Event = e });
        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete(HellViewModel model)
        {
            Event e = eventUoW.Events.GetEventByID(model.Event.Id);
            if (e == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (e.OwnerId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            e.ModificationState = ModificationState.Deleted;
            eventUoW.Events.Attach(e);
            eventUoW.Save();

            return RedirectToAction("Index", "Home");
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