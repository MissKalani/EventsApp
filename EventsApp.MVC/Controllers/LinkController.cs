using EventsApp.DataAccess;
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
            InviteLink link = new InviteLink { EventId = eventId, LinkGUID = guid.ToString(), ModificationState = ModificationState.Added };
            eventUoW.InviteLinks.Attach(link);
            eventUoW.Save();

            UrlHelper urlHelper = new UrlHelper(HttpContext.Request.RequestContext);
            string url = urlHelper.Action("Confirm", "Link", new { guid = guid.ToString() }, urlHelper.RequestContext.HttpContext.Request.Url.Scheme);

            return Json(new { url = url });
        }

        [HttpGet]
        public ActionResult Confirm(string guid)
        {
            InviteLink link = eventUoW.InviteLinks.GetLinkGraphByGuid(guid);
            if (link == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (User.Identity.IsAuthenticated)
            {
                // Show a management page for the owner.
                if (link.Event.OwnerId == User.Identity.GetUserId())
                {
                    return View("Owner", new HellViewModel { LinkConfirmViewModel = new LinkConfirmViewModel { Link = link, LinkGUID = link.LinkGUID } });
                }

                // Show a different page if the user is already invited to this event.
                AppUser user = eventUoW.Users.GetUserById(User.Identity.GetUserId());
                if (eventUoW.Invites.IsInvited(link.Event, user))
                {
                    return View("Invited", new HellViewModel { LinkConfirmViewModel = new LinkConfirmViewModel { Link = link, LinkGUID = link.LinkGUID } });
                }
            }

            return View("Confirm", new HellViewModel { LinkConfirmViewModel = new LinkConfirmViewModel { Link = link, LinkGUID = link.LinkGUID } });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Accept(HellViewModel model)
        {
            InviteLink link = eventUoW.InviteLinks.GetLinkGraphByGuid(model.LinkConfirmViewModel.LinkGUID);
            if (link == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Make sure the user is not the owner.
            if (link.Event.OwnerId == User.Identity.GetUserId())
            {
                return View("Owner", new HellViewModel { LinkConfirmViewModel = new LinkConfirmViewModel { Link = link, LinkGUID = link.LinkGUID } });
            }

            // Make sure the user is not already invited.
            AppUser user = eventUoW.Users.GetUserById(User.Identity.GetUserId());
            if (eventUoW.Invites.IsInvited(link.Event, user))
            {
                return View("Invited", new HellViewModel { LinkConfirmViewModel = new LinkConfirmViewModel { Link = link, LinkGUID = link.LinkGUID } });
            }

            // Create an invite and remove this link.
            eventUoW.Invites.Attach(new Invite { Event = link.Event, AppUser = user, Status = InviteStatus.Accepted, Seen = true, ModificationState = ModificationState.Added });
            eventUoW.InviteLinks.Remove(link);
            eventUoW.Save();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Delete(HellViewModel model)
        {
            InviteLink link = eventUoW.InviteLinks.GetLinkGraphByGuid(model.LinkConfirmViewModel.LinkGUID);
            if (link == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            // Simply remove the link.
            eventUoW.InviteLinks.Remove(link);
            eventUoW.Save();

            return RedirectToAction("Index", "Home");
        }
    }
}