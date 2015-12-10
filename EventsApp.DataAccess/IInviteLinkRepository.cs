using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataAccess
{
    public interface IInviteLinkRepository : IRepository<InviteLink>
    {
        /// <summary>
        /// Get the link and the associated event and user graph.
        /// </summary>
        InviteLink GetLinkGraphByGuid(string guid);

        /// <summary>
        /// Get the share link (i.e. the first multi-use link) for a given event. The share link
        /// is used to share an event on social networks.
        /// </summary>
        InviteLink GetShareLink(Event e);

        /// <summary>
        /// Remove a link.
        /// </summary>
        void Remove(InviteLink link);
    }
}
