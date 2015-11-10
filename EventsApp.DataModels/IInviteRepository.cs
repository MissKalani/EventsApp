using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
{
    public interface IInviteRepository : IRepository<Invite>
    {
        bool IsInvited(Event e, AppUser user);
    }
}
