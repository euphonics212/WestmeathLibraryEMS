using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WestmeathLibraryEMS.Models
{
    public class EventDay
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd/MM/yyyy}")]
        [JsonProperty("start")]
        [Display(Name = "Start Date")]
        public DateTime EventDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd/MM/yyyy}")]
        [JsonProperty("endDate")]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }


        [Required]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; } = new TimeSpan(10, 0, 0);

        [Required]
        [JsonProperty("end")]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; } = new TimeSpan(17, 0, 0);


        public Event Event { get; set; }

        [Required]
        [JsonProperty("title")]
        [Display(Name = "Event")]
        public int EventId { get; set; }


        public EventStatus EventStatus { get; set; }

        [Required]
        [Display(Name = "Event Status")]
        public int? EventStatusId { get; set; } = 4;

        public DateTime? DateAdded { get; set; }

        public int? ActualAttendees { get; set; } = 0;
        public string Feedback { get; set; } = null ?? "None Given";
    }
}
