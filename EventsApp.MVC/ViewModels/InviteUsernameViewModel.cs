using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventsApp.MVC.ViewModels
{ 
    public class InviteUsernameViewModel
    {
        public enum Result { Invited, UserNotFound, EventNotFound, InviterIsNotOwner, UserIsAlreadyInvited, UserIsOwner };

        public Result InviteResult { get; set; }
    }
}