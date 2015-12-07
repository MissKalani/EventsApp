﻿using EventsApp.DataAccess;
using EventsApp.DataModels;
using EventsApp.MVC.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EventsApp.MVC.Controllers
{
    public class LinkController : Controller
    {
        private IEventUnitOfWork eventUoW;

        public LinkController(IEventUnitOfWork eventUoW)
        {
            this.eventUoW = eventUoW;
        }

        [Authorize]
        [HttpPost]
        public ActionResult Generate(int eventId)
        {
            Event e = eventUoW.Events.GetEventByID(eventId);
            if (e == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (User.Identity.GetUserId() != e.OwnerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            Guid guid = Guid.NewGuid();
            InviteLink link = new InviteLink { EventId = eventId, LinkGUID = guid.ToString(), OneTimeUse = true, ModificationState = ModificationState.Added };
            eventUoW.InviteLinks.Attach(link);
            eventUoW.Save();

            UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
            string url = urlHelper.Action("Details", "Event", new { id = eventId, guid = guid.ToString() }, urlHelper.RequestContext.HttpContext.Request.Url.Scheme);

            return Json(new { url = url });
        }

        [Authorize]
        [HttpPost]
        public ActionResult GenerateMultiuse(int eventId)
        {
            Event e = eventUoW.Events.GetEventByID(eventId);
            if (e == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (User.Identity.GetUserId() != e.OwnerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            Guid guid = Guid.NewGuid();
            InviteLink link = new InviteLink { EventId = eventId, LinkGUID = guid.ToString(), OneTimeUse = false, ModificationState = ModificationState.Added };
            eventUoW.InviteLinks.Attach(link);
            eventUoW.Save();

            UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
            string url = urlHelper.Action("Details", "Event", new { id = eventId, guid = guid.ToString() }, urlHelper.RequestContext.HttpContext.Request.Url.Scheme);

            return Json(new { url = url });
        }

        [Authorize]
        [HttpGet]
        public ActionResult Accept(string guid)
        {
            InviteLink link = eventUoW.InviteLinks.GetLinkGraphByGuid(guid);
            if (link == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Make sure the user is not the owner.
            if (link.Event.OwnerId == User.Identity.GetUserId())
            {
                return RedirectToAction("Details", "Event", new { id = link.EventId });
            }

            // Make sure the user is not already invited.
            AppUser user = eventUoW.Users.GetUserById(User.Identity.GetUserId());
            if (eventUoW.Invites.IsInvited(link.Event, user))
            {
                return RedirectToAction("Details", "Event", new { id = link.EventId });
            }

            // Create an invite.
            eventUoW.Invites.Attach(new Invite { Event = link.Event, AppUser = user, Status = InviteStatus.Accepted, Seen = true, ModificationState = ModificationState.Added });

            // Remove the invite if it is one time use.
            if (link.OneTimeUse)
            {
                eventUoW.InviteLinks.Remove(link);
            }
            
            eventUoW.Save();

            return RedirectToAction("Details", "Event", new { id = link.EventId });
        }

        [Authorize]
        [HttpGet]
        public ActionResult Decline(string guid)
        {
            InviteLink link = eventUoW.InviteLinks.GetLinkGraphByGuid(guid);
            if (link == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Make sure the user is not the owner.
            if (link.Event.OwnerId == User.Identity.GetUserId())
            {
                return RedirectToAction("Details", "Event", new { id = link.EventId });
            }

            // Make sure the user is not already invited.
            AppUser user = eventUoW.Users.GetUserById(User.Identity.GetUserId());
            if (eventUoW.Invites.IsInvited(link.Event, user))
            {
                return RedirectToAction("Details", "Event", new { id = link.EventId });
            }

            // Create an invite.
            eventUoW.Invites.Attach(new Invite { Event = link.Event, AppUser = user, Status = InviteStatus.Declined, Seen = true, ModificationState = ModificationState.Added });

            // Remove the invite if it is one time use.
            if (link.OneTimeUse)
            {
                eventUoW.InviteLinks.Remove(link);
            }
            
            eventUoW.Save();

            return RedirectToAction("Details", "Event", new { id = link.EventId });
        }

        [Authorize]
        [HttpGet]
        public ActionResult Delete(string guid)
        {
            InviteLink link = eventUoW.InviteLinks.GetLinkGraphByGuid(guid);
            if (link == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (User.Identity.GetUserId() != link.Event.OwnerId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            // Simply remove the link.
            eventUoW.InviteLinks.Remove(link);
            eventUoW.Save();

            return RedirectToAction("Details", "Event", new { id = link.EventId });
        }
    }
}