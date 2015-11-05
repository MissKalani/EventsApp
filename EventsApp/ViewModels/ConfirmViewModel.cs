using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.ViewModels
{
    public class ConfirmViewModel
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
