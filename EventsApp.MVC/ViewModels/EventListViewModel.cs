using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.MVC.ViewModels
{
    /// <summary>
    /// Specifies how a user is related to an event: whether they
    /// have created it, have been invited to it, or whether the event
    /// is public to all.
    /// </summary>
    public enum EventUserRelation
    {
        Hosted,
        Invited,
        Public
    }

    /// <summary>
    /// Used for populating the list and map on the home page. Currently only
    /// serialized to JSON and parsed after an AJAX call on the client.
    /// </summary>
    public class EventListViewModel
    {
        public int Id { get; set; }
        public string Brief { get; set; }
        public string Detailed { get; set; }
        public string HostName { get; set; }
        public string Address { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime StartTime { get; set; }
        public EventVisibility Visibility { get; set; }
        public EventUserRelation Relation { get; set; }
    }
}
