using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Facebook;
using EventsApp.DataAccess;
using Microsoft.AspNet.Identity.Owin;
using EventsApp.DataModels;

[assembly: OwinStartup(typeof(EventsApp.MVC.Startup))]

namespace EventsApp.MVC
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/auth/login")
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseFacebookAuthentication("1483775658597403", "3e0a203bfe9283d4e1a3f5d8a89aad7f");



        }
    }
}
