using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
{
    public interface IEventUnitOfWork
    {
        IEventRepository Events { get; set; }
        IUserRepository Users { get; set; }
        IInviteRepository Invites { get; set; }
        IInviteLinkRepository InviteLinks { get; set; }
        void Save();
    }
}
