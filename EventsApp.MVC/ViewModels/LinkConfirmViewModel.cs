using EventsApp.DataModels;

namespace EventsApp.MVC.ViewModels
{
    public class LinkConfirmViewModel
    {
        /// <summary>
        /// Sent to the view when browsing to the confirm page via an invite link.
        /// </summary>
        public InviteLink Link { get; set; }

        /// <summary>
        /// Sent back to the controller to identify the link.
        /// </summary>
        public string LinkGUID { get; set; }
    }
}
