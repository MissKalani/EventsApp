using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
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
    }
}
