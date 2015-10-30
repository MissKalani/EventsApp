using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsApp.ViewModels
{
    public class CreateViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "A brief description of the event is required.")]
        [Display(Name ="Brief Description")]
        public string Brief { get; set; }

        [Display(Name = "Detailed Description")]
        [DataType(DataType.MultilineText)]
        public string Detailed { get; set; }

        public string Address { get; set; }

        [Required]
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify when your event starts.")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }
    }
}
