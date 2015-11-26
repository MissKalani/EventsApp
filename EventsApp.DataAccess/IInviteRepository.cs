using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataAccess
{
    public interface IInviteRepository : IRepository<Invite>
    {
        /// <summary>
        /// Returns true if the user is invited to the event.
        /// </summary>
        bool IsInvited(Event e, AppUser user);

        /// <summary>
        /// Returns an invite entity (no graph) given an event and a user. Returns null if the user is not invited to the event.
        /// </summary>
        Invite GetInviteByEventAndUser(Event e, AppUser user);

        /// <summary>
        /// Get everyone that is invited to this event. Includes the AppUser node. Does not include the host.
        /// </summary>
        List<Invite> GetInvitedToEvent(Event e);

        /// <summary>
        /// Get the invites this user has not accepted or declined yet. Also includes the event graph node
        /// for all returned invites.
        /// </summary>
        List<Invite> GetPendingInvitesWithEventGraph(AppUser user);

        /// <summary>
        /// Get the number of pending invites that are new since the last time the user checked.
        /// </summary>
        int GetUnseenPendingInvitesCount(AppUser user);

        /// <summary>
        /// Mark all current invites as seen by the user (will influence what is returned by GetUnseenPendingInvitesCount).
        /// </summary>
        void MarkAllInvitesAsSeen(AppUser user);
    }
}
