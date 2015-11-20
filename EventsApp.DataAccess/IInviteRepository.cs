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
        bool IsInvited(Event e, AppUser user);

        /// <summary>
        /// Get everyone that is invited to this event. Includes the AppUser node. Does not include the host.
        /// </summary>
        List<Invite> GetInvitedToEvent(Event e);
    }
}
