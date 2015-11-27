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
        public LinkConfirmViewModel LinkConfirmViewModel { get; set; }
        public CreateViewModel CreateViewModel { get; set; }
        public EditViewModel EditViewModel { get; set; }
        public DetailsViewModel DetailsViewModel { get; set; }
        public UserDetailsViewModel UserDetailsViewModel { get; set; }
        public ConnectExistingAccountViewModel ConnectExistingAccountViewModel { get; set; }
        public ConnectNewAccountViewModel ConnectNewAccountViewModel { get; set; }
        public ExternalLoginListViewModel ExternalLoginListViewModel { get; set; }
        public ManageLoginsViewModel ManageLoginsViewModel { get; set; }
        //public ViewEvent ViewEvent { get; set; }
        //public List<Event> Events { get; set; }
        public Event Event { get; set; }

        public AppUser User { get; set; }
    }
}