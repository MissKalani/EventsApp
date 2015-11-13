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

        public List<Event> GetAllPublicEvents()
        {
            return context.Events.Where(t => t.Visibility == EventVisibility.Public).ToList();
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

        public List<AppUser> GetInvitedUsers(Event e)
        {
            var query = from u in context.Users
                        join i in context.Invites on u.Id equals i.AppUserId
                        where i.EventId == e.Id
                        select u;
            return query.ToList();
        }

        public Event GetEventByID(int i)
        {
            return context.Events.Find(i); 
        }
    }
}
