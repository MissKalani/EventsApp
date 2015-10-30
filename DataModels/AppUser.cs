using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
{
    public class AppUser : IdentityUser, IModificationState
    {
        //public string Firstname { get; set; }
        //public string Lastname { get; set; }
        public List<Event> Events { get; set; }
        public ModificationState ModificationState { get; set; }
    }

}
