                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       using EventsApp.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventsApp.MVC.ViewModels
{
    public class ManageEventViewModel
    {
        public int EventId;

        [Required(AllowEmptyStrings = false, ErrorMessage = "A brief description of the event is required.")]
        [Display(Name = "Brief Description")]
        public string Brief { get; set; }

        [Display(Name = "Detailed Description")]
        [DataType(DataType.MultilineText)]
        public string Detailed { get; set; }

        public string Address { get; set; }

        [Required]
        public EventVisibility Visibility { get; set; }

        [Required]
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify when your event starts.")]
        [DataType(DataType.Date, ErrorMessage = "Must be a valid date (yyyy-MM-dd).")]
        [Display(Name = "Event Date")]
        [DisplayFormat(DataFormatString = "{0}:yyyy-MM-dd")]
        public DateTime StartTime { get; set; }

    }
}