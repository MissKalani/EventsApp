using EventsApp.DataAccess;
using EventsApp.DataModels;
using EventsApp.MVC.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EventsApp.MVC.Controllers
{
    public class AuthController : Controller
    {
        private IEventUnitOfWork eventUoW;
        private IAuthenticationManager AuthenticationManager { get { return HttpContext.GetOwinContext().Authentication; } }

        // GET: Auth
        public AuthController(IEventUnitOfWork eventUoW)
        {
            this.eventUoW = eventUoW;
        }

        [AllowAnonymous]
        public ActionResult LogIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //GET: /Auth/LogIn
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(HellViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await eventUoW.Users.UserManager.FindAsync(model.LoginUserViewModel.UserName, model.LoginUserViewModel.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.LoginUserViewModel.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            return View(model);
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //POST: Auth/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(HellViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser() { UserName = model.RegisterUserViewModel.UserName };
                var result = await eventUoW.Users.UserManager.CreateAsync(user, model.RegisterUserViewModel.Password);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }
            return View(model);

        }

        private async Task SignInAsync(AppUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await eventUoW.Users.UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }


        // GET: /Auth/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        // POST: /Auth/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(HellViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await eventUoW.Users.UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.ManageUserViewModel.OldPassword, model.ManageUserViewModel.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await eventUoW.Users.UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.ManageUserViewModel.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private bool HasPassword()
        {
            var user = eventUoW.Users.UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPassword(AppUser user)
        {
            if(user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Auth", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }            

            var signInManager = new SignInManager<AppUser, string>(eventUoW.Users.UserManager, HttpContext.GetOwinContext().Authentication);

            var result = await signInManager.ExternalSignInAsync(loginInfo, false);

            switch (result)
            {
                case SignInStatus.Success:
                    {
                        var user = UserManager.Find(loginInfo.Login);
                        if (HasPassword(user))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                    return RedirectToAction("ConnectAccount");                    
                        }
                    }

                //case SignInStatus.LockedOut:
                //    return RedirectToAction("Index","Home");
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                    {
                    var user = new AppUser { UserName = loginInfo.DefaultUserName };
                    eventUoW.Users.UserManager.Create(user);
                    eventUoW.Users.UserManager.AddLogin(user.Id, loginInfo.Login);
                    signInManager.ExternalSignIn(loginInfo, false);
                    return RedirectToAction("ConnectAccount");
                    }

                default:
                    return RedirectToAction("Register", "Auth");
            }

        }

        [Authorize]
        public ActionResult ConnectAccount()
        {
            User.Identity.GetUserId();
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConnectNewAccount(HellViewModel model)
        {
            if (ModelState.IsValid)
            {
                var socialUser = eventUoW.Users.GetUserById(User.Identity.GetUserId());
                IList<UserLoginInfo> loginInfo = UserManager.GetLogins(socialUser.Id);

                var newUser = new AppUser() { UserName = model.ConnectNewAccountViewModel.UserName };
                var result = await UserManager.CreateAsync(newUser, model.ConnectNewAccountViewModel.Password);
                eventUoW.Users.RemoveAccount(socialUser);
                eventUoW.Save();
                UserManager.AddLogin(newUser.Id, loginInfo[0]);
                if (result.Succeeded)
                {
                    await SignInAsync(newUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
        {
                    AddErrors(result);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> ConnectExistingAccount(HellViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid username or password.");
            return View("ConnectAccount", model);
        }

            AppUser existingUser = await eventUoW.Users.UserManager.FindAsync(model.ConnectExistingAccountViewModel.UserName, model.ConnectExistingAccountViewModel.Password);
            if (existingUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            AppUser socialUser = eventUoW.Users.UserManager.FindById(User.Identity.GetUserId());
            if (socialUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            IList<UserLoginInfo> logins = await eventUoW.Users.UserManager.GetLoginsAsync(socialUser.Id);
            if (logins.Count > 1)
            {
                throw new InvalidOperationException("A social account was found to have more than one login.");
            }

            // Migrate the invites by the social user. Ignore invites to events hosted by the existingUser, let them be destroyed with the socialUser.
            eventUoW.Invites.TransferInviteOwnership(socialUser, existingUser);

            // Migrate the created events by the social user.
            eventUoW.Events.TransferEventOwnership(socialUser, existingUser);

            // Remove the dedicated social account.
            AuthenticationManager.SignOut();
            eventUoW.Users.UserManager.Delete(socialUser);
            eventUoW.Save();

            // Add this social login to the existing user.
            IdentityResult result = await eventUoW.Users.UserManager.AddLoginAsync(existingUser.Id, logins[0]);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to connect " + logins[0].LoginProvider + " login to existing user.");
            }

            // Login as the existing user.
            await SignInAsync(existingUser, false);

            // Return to the homepage.
            return RedirectToAction("Index", "Home");
        }

        
        public ActionResult RemoveAccount()
        {
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult RemoveAccount(HellViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = eventUoW.Users.GetUserById(User.Identity.GetUserId());
                eventUoW.Users.RemoveAccount(user);
                eventUoW.Save();
                LogOut();
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            { }
            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = eventUoW.Users.UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }
    }
}