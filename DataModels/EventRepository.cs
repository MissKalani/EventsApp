using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
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
            context.Events.Attach(entity);
            ContextStateHelper.ApplyStateChanges(context);
        }

        public List<Event> GetAllPublicEvents()
        {
            // TODO: Ensure that the events returned actually ARE public!
            return context.Events.ToList();
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
