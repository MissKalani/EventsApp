using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace EventsApp.DataModels
{
    public class InviteLinkRepository : IInviteLinkRepository
    {
        private EventContext context;

        public InviteLinkRepository()
        {
            context = new EventContext();
        }

        public void Attach(InviteLink entity)
        {
            context.InviteLinks.Add(entity);
            ContextStateHelper.ApplyStateChanges(context);
        }



        public void Save()
        {
            context.SaveChanges();
        }

        public InviteLink GetLinkGraphByCode(string code)
        {
            return context.InviteLinks.Include(t => t.Event.AppUser).SingleOrDefault(t => t.LinkGUID == code);
        }

        public void Remove(InviteLink link)
        {
            context.InviteLinks.Remove(link);
        }
    }
}
