using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventsApp.MVC.ViewModels
{
    public class HellViewModel
    {
        public RegisterUserViewModel RegisterUserViewModel { get; set; }
        public LoginUserViewModel LoginUserViewModel { get; set; }
        public ManageUserViewModel ManageUserViewModel { get; set; }
        //public ConfirmViewModel ConfirmViewModel { get; set; }
        public CreateViewModel CreateViewModel { get; set; }
        //public ViewEvent ViewEvent { get; set; }
        //public List<Event> Events { get; set; }
        //public Event Event { get; set; }
    }
}