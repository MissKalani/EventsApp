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
    public class UserController : Controller
    {
        private IEventUnitOfWork eventUoW;

        public UserController(IEventUnitOfWork eventUoW)
        {
            this.eventUoW = eventUoW;
        }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string usernameSubstring)
        {
            List<AppUser> users = eventUoW.Users.SearchForUser(usernameSubstring);
            List<SearchUserViewModel> result = new List<SearchUserViewModel>();
            foreach (var user in users)
            {
                result.Add(new SearchUserViewModel { Id = user.Id, Username = user.UserName });
            }

            return Json(result);
        }

        [Authorize]
        [HttpPost]
        public ActionResult InviteByUsername(int eventId, string username)
        {
            InviteUsernameViewModel result = new InviteUsernameViewModel();
            Event e = eventUoW.Events.GetEventByID(eventId);
            if (e == null)
            {
                result.InviteResult = InviteUsernameViewModel.Result.EventNotFound;
                return Json(result);
            }

            if (User.Identity.GetUserId() != e.OwnerId)
            {
                result.InviteResult = InviteUsernameViewModel.Result.InviterIsNotOwner;
                return Json(result);
            }

            AppUser user = eventUoW.Users.GetUserByUsername(username);
            if (user == null)
            {
                result.InviteResult = InviteUsernameViewModel.Result.UserNotFound;
                return Json(result);
            }

            if (user.Id == e.OwnerId)
            {
                result.InviteResult = InviteUsernameViewModel.Result.UserIsOwner;
                return Json(result);
            }

            if (eventUoW.Invites.IsInvited(e, user))
            {
                result.InviteResult = InviteUsernameViewModel.Result.UserIsAlreadyInvited;
                return Json(result);
            }

            Invite invite = new Invite { Event = e, AppUser = user, Status = InviteStatus.Pending, ModificationState = ModificationState.Added };
            eventUoW.Invites.Attach(invite);
            eventUoW.Save();

            result.InviteResult = InviteUsernameViewModel.Result.Invited;
            return Json(result);
        }
    }
}