using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
{
    public class UserRepository : IUserRepository
    {
        private EventContext context;

        public UserRepository()
        {
            context = new EventContext();
        }

        public AppUser GetUserById(string userId)
        {
            return context.Users.Find(userId);
        }
    }
}
