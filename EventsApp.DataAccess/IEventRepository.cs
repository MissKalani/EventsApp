using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataAccess
{
    public interface IEventRepository : IRepository<Event>
    {
        /// <summary>
        /// Get all public events.
        /// </summary>
        List<Event> GetAllPublicEvents();

        /// <summary>
        /// Get all events that a user is invited to.
        /// </summary>
        /// <param name="user">The user who is invited.</param>
        List<Event> GetAllInvitedEvents(AppUser user);

        /// <summary>
        /// Get all events that a user has created.
        /// </summary>
        /// <param name="user">The user who created the events.</param>
        List<Event> GetAllCreatedEvents(AppUser user);

        Event GetEventByID(int id);
    }
}
