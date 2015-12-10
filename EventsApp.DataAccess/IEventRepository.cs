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
        /// Get all public events created by the user 
        /// </summary>
        List<Event> GetAllPublicEventsOfUser(AppUser user);


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

        /// <summary>
        /// Loads the AppUser node in the specified event graph.
        /// </summary>
        AppUser LoadUserGraph(Event e);

        /// <summary>
        /// Returns an event by its primary key.
        /// </summary>
        Event GetEventByID(int id);

        /// <summary>
        /// Transfers the ownership of all events created by previousOwner to newOwner.
        /// </summary>
        void TransferEventOwnership(AppUser previousOwner, AppUser newOwner);
    }
}
