using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        

        public AppUser GetUserByUsername(string username)
        {
            var userDetails = context.Users.Include(e => e.Events).Where(u => u.UserName == username);
            return userDetails.SingleOrDefault();
        }

        public List<AppUser> SearchForUser(string usernameSubstring)
        {
            return context.Users.Where(t => t.UserName.Contains(usernameSubstring)).ToList();
        }

        public void RemoveAccount(AppUser user)
        {
            context.Entry(user).State = EntityState.Deleted;            
        }
    }
}
