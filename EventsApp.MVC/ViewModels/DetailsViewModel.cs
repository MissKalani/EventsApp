using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.MVC.ViewModels
{
    public class DetailsViewModel
    {
        public Event Event { get; set; }
        public List<Invite> Invites { get; set; }
        public bool IsOwner { get; set; }
    }
}
