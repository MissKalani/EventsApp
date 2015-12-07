using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.DataModels
{
    public class InviteLink : IModificationState
    {
        public string LinkGUID { get; set; }
        public int EventId { get; set; }
        public ModificationState ModificationState { get; set; }
        public bool OneTimeUse { get; set; }

        public Event Event { get; set; }
    }
}
