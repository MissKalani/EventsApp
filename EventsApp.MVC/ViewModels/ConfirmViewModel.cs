using EventsApp.DataModels;

namespace EventsApp.MVC.ViewModels
{
    /// <summary>
    /// Inheriting from LoginUserViewModel since this view model is used when logged out.
    /// </summary>
    public class ConfirmViewModel : LoginUserViewModel
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
