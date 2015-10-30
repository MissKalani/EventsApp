using EventsApp.DataModels;
using EventsApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EventsApp.Controllers
{
    public class AuthController : Controller
    {
        public static bool IsAuthenticated = false;

        // GET: Auth
        public AuthController()
            :this(new UserManager<AppUser>(new UserStore<AppUser>(new EventContext())))
        {         
        }

        public AuthController(UserManager<AppUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<AppUser> UserManager { get; private set; }

        [HttpPost]
        public ActionResult LogIn()
        {
            IsAuthenticated = true;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOut()
        {
            IsAuthenticated = false;
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = new AppUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
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
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}