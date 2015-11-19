
﻿using EventsApp.DataAccess;

using EventsApp.MVC.ViewModels;

﻿using EventsApp.DataModels;
using Microsoft.AspNet.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
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

        public UserManager<AppUser> UserManager { get; private set; }

        // GET: User
        public ActionResult Details()
        {
            var userId = User.Identity.GetUserId();
            return View();
        }

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
    }
}