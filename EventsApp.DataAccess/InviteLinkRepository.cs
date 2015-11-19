using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using EventsApp.DataModels;

namespace EventsApp.DataAccess
{
    public class InviteLinkRepository : IInviteLinkRepository
    {
        private EventContext context;

        public InviteLinkRepository(EventContext context)
        {
            this.context = context;
        }

        public void Attach(InviteLink entity)
        {
            context.InviteLinks.Add(entity);
            ContextStateHelper.ApplyStateChanges(context);
        }

        public InviteLink GetLinkGraphByGuid(string guid)
        {
            return context.InviteLinks.Include(t => t.Event.AppUser).SingleOrDefault(t => t.LinkGUID == guid);
        }

        public void Remove(InviteLink link)
        {
            context.InviteLinks.Remove(link);
        }
    }
}
