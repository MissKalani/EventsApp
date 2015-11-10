using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataAccess
{
    public class UserRepository : IUserRepository
    {
        private EventContext context;

        public UserRepository(EventContext context)
        {
            this.context = context;
        }

        public AppUser GetUserById(string userId)
        {
            return context.Users.Find(userId);
        }
    }
}
