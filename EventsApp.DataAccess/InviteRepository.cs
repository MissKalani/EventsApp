using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataAccess
{
    public class InviteRepository : IInviteRepository
    {
        private EventContext context;

        public InviteRepository(EventContext context)
        {
            this.context = context;
        }

        public void Attach(Invite entity)
        {
            context.Invites.Add(entity);
            ContextStateHelper.ApplyStateChanges(context);
        }

        public List<Invite> GetInvitedToEvent(Event e)
        {
            return context.Invites.Include(t => t.AppUser).Where(t => t.EventId == e.Id).ToList();
        }

        public bool IsInvited(Event e, AppUser user)
        {
            return context.Invites.Any(t => t.EventId == e.Id && t.AppUserId == user.Id);
        }
    }
}
