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

        public List<Invite> GetAllInvitedEvents(AppUser user)
        {
            return context.Invites.Include(i => i.Event.AppUser).Where(i => i.AppUserId == user.Id).ToList();
        }

        public bool IsInvited(Event e, AppUser user)
        {
            return context.Invites.Any(t => t.EventId == e.Id && t.AppUserId == user.Id);
        }

        public Invite GetInviteByEventAndUser(Event e, AppUser user)
        {
            return context.Invites.SingleOrDefault(t => t.EventId == e.Id && t.AppUserId == user.Id);
        }

        public List<Invite> GetPendingInvitesWithEventGraph(AppUser user)
        {
            return context.Invites.Include(t => t.Event).Where(t => t.Status == InviteStatus.Pending && t.AppUserId == user.Id).ToList();
        }
        
        public int GetUnseenPendingInvitesCount(AppUser user)
        {
            return context.Invites.Count(t => t.AppUserId == user.Id && !t.Seen);
        }

        public void MarkAllInvitesAsSeen(AppUser user)
        {
            var invites = context.Invites.Where(t => t.AppUserId == user.Id && !t.Seen).ToList();
            foreach (var invite in invites)
            {
                invite.Seen = true;
            }
        }

        public void TransferInviteOwnership(AppUser previousUser, AppUser newUser)
        {
            var invites = context.Invites.Include(t => t.Event).Where(t => t.AppUserId == previousUser.Id).ToList();
            foreach (var invite in invites)
            {
                if (invite.Event.OwnerId != newUser.Id)
                {
                    context.Invites.Add(new Invite { AppUserId = newUser.Id, EventId = invite.EventId, Status = invite.Status, Seen = invite.Seen, ModificationState = invite.ModificationState });
                    context.Invites.Remove(invite);
                }
            }
        }

        public void RemoveInvitesByHost(AppUser invited, AppUser host)
        {
            var invites = context.Invites.Include(t => t.Event).Where(t => t.AppUserId == invited.Id && t.Event.OwnerId == host.Id).ToList();
            context.Invites.RemoveRange(invites);
        }
    }
}
