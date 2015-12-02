using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
{
    public enum InviteStatus
    {
        Pending,
        Accepted,
        Declined
    }

    public class Invite : IModificationState
    {
        public int EventId { get; set; }
        public string AppUserId { get; set; }
        public InviteStatus Status { get; set; }
        public bool Seen { get; set; }
        public ModificationState ModificationState { get; set; }

        public Event Event { get; set; }
        public AppUser AppUser { get; set; }
    }
}
