using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.ViewModels
{
    public enum EventUserRelation
    {
        Hosted,
        Invited,
        Public
    }

    public class ViewEvent
    {
        public int EventId { get; set; }
        public string Brief { get; set; }
        public string Detailed { get; set; }
        public string Address { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime StartTime { get; set; }
        public EventUserRelation Relation { get; set; }
    }
}
