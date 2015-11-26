using EventsApp.DataModels;
using Microsoft.AspNet.Identity;
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
        public UserManager<AppUser, string> UserManager { get; private set; }

        public UserRepository(EventContext context, UserManager<AppUser, string> userManager)
        {
            this.context = context;
            UserManager = userManager;
        }

        public AppUser GetUserById(string userId)
        {
            return UserManager.FindById(userId);
        }
        
        public AppUser GetUserByUsername(string username)
        {
            return context.Users.Include(e => e.Events).Where(u => u.UserName == username).SingleOrDefault();
        }

        public List<AppUser> SearchForUser(string usernameSubstring)
        {
            return context.Users.Where(t => t.UserName.Contains(usernameSubstring)).ToList();
        }

        public void RemoveAccount(AppUser user)
        {
            UserManager.Delete(user);
        }

        //public AppUser GetUserByProviderId()
        //{
        //    context.Entry.
        //    return null;
        //}
    }
}
