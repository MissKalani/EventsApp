﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace EventsApp.DataModels
{
    public enum EventVisibility
    {
        Public,
        Private
    }

    public class Event : IModificationState
    {
        public int Id { get; set; }
        public string Brief { get; set; }
        public string Detailed { get; set; }
        public string Address { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime StartTime { get; set; }
        public EventVisibility Visibility { get; set; }
        // TODO: Add optional ticket information.
        public string OwnerId { get; set; }
        public AppUser AppUser { get; set; }
        public ModificationState ModificationState { get; set; }
    }
}
