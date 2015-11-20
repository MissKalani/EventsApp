using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataAccess
{
    /// <summary>
    /// Does not implement IRepository since users need to be added through a user manager anyway.
    /// </summary>
    public interface IUserRepository
    {
        AppUser GetUserById(string userId);

        AppUser GetUserByUsername(string username);

        List<AppUser> SearchForUser(string usernameSubstring);
    }
}
