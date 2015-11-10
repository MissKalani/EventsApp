using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
{
    public interface IInviteLinkRepository : IRepository<InviteLink>
    {
        /// <summary>
        /// Get the link and the associated event and user graph.
        /// </summary>
        InviteLink GetLinkGraphByCode(string code);

        /// <summary>
        /// Remove a link.
        /// </summary>
        void Remove(InviteLink link);
    }
}
