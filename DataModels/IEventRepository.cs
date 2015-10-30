using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
{
    public interface IEventRepository : IRepository<Event>
    {
        /// <summary>
        /// Get all public events.
        /// </summary>
        List<Event> GetAllPublicEvents();
    }
}
