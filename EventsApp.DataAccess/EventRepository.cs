using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataAccess
{
    public class EventRepository : IEventRepository
    {
        private EventContext context;

        public EventRepository(EventContext context)
        {
            this.context = context;
        }

        public void Attach(Event entity)
        {
            context.Events.Add(entity);
            ContextStateHelper.ApplyStateChanges(context);
        }

        public void DeleteEvent(Event entity)
        {
            // Delete all invites to this event.
            var invites = context.Invites.Where(i => i.EventId == entity.Id).ToList();
            foreach (var invite in invites)
            {
                context.Invites.Remove(invite);
            }

            // Delete the event.
            entity.ModificationState = ModificationState.Deleted;
            Attach(entity);
        }

        public List<Event> GetAllPublicEvents()
        {
            return context.Events.Where(t => t.Visibility == EventVisibility.Public).ToList();
        }

        public List<Event> GetAllPublicEventsOfUser(AppUser user)
        {
            return context.Events.Where(e => e.OwnerId == user.Id).Where(t => t.Visibility == EventVisibility.Public).ToList();
        }

        public List<Event> GetAllInvitedEvents(AppUser user)
        {
            var query = from e in context.Events
                        join i in context.Invites on e.Id equals i.EventId
                        where i.AppUserId == user.Id
                        select e;
            return query.ToList();
        }
        
        public List<Event> GetAllCreatedEvents(AppUser user)
        {
            return context.Events.Where(t => t.OwnerId == user.Id).ToList();
        }

        public AppUser LoadUserGraph(Event e)
        {
            context.Entry(e).Reference(t => t.AppUser).Load();
            return e.AppUser;
        }

        public Event GetEventByID(int i)
        {
            return context.Events.Find(i); 
        }

        public void TransferEventOwnership(AppUser previousOwner, AppUser newOwner)
        {
            List<Event> events = context.Events.Where(t => t.OwnerId == previousOwner.Id).ToList();
            foreach (var e in events)
            {
                e.AppUser = newOwner;
                e.ModificationState = ModificationState.Modified;
            }
        }
    }
}
