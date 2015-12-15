using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventsApp.MVC.ViewModels
{
    public class UserDetailsViewModel
    {
        public AppUser User { get; set; }
   
        public List<Invite> Invites { get; set; }
    }
}