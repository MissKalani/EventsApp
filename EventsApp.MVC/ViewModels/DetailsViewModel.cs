using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.MVC.ViewModels
{
    public enum InvitationType
    {
        NotInvited,
        IsInvited,
        HasInvitationLink
    }

    public class DetailsViewModel
    {
        /// <summary>
        /// The event that should be displayed.
        /// </summary>
        public Event Event { get; set; }

        /// <summary>
        /// A list of all invited people to this event.
        /// </summary>
        public List<Invite> Invites { get; set; }

        /// <summary>
        /// True if the user is hosting the event.
        /// </summary>
        public bool IsOwner { get; set; }

        /// <summary>
        /// True if invited. False if the host.
        /// </summary>
        public bool IsInvited { get; set; }

        /// <summary>
        /// Valid if the user is explicitly invited to the event.
        /// </summary>
        public Invite UserInvite { get; set; }

        /// <summary>
        /// The link provided when browsing to the details page.
        /// </summary>
        public InviteLink Link { get; set; }

        /// <summary>
        /// The persistent link used to share this event on social networks.
        /// </summary>
        public InviteLink ShareLink { get; set; }
    }
}
