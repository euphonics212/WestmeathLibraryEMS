using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WestmeathLibraryEMS.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Display(Name = "Event Id Code")]
        public string Guid { get; set; }


        [Required(ErrorMessage = "Please enter a name for the event")]
        [Display(Name = "Event Name")]
        public String EventName { get; set; }

        [Display(Name = "Requirements")]
        public string Requirements { get; set; }

        [Required(ErrorMessage = "Please enter a description for the event")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Ties in with")]
        public string TiesInWith { get; set; }

        [Required]
        [Range(0.0, 99999, ErrorMessage = "Input exceeds the allowed value")]
        [DataType(DataType.Currency)]
        public decimal? Cost { get; set; } = 0;

        [Display(Name = "Booked Event")]
        public bool BookedEvent { get; set; }

        [Display(Name = "Online Event")]
        public bool OnlineEvent { get; set; }

        [Required(ErrorMessage = "Must be a number")]
        [Range(0, int.MaxValue)]
        [Display(Name = "Max Attendees")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only numbers allowed")]
        public int MaxAttendees { get; set; }

        public EventType EventType { get; set; }

        [Required]
        [Display(Name = "Event Type")]
        public int EventTypeId { get; set; }

        public Venue Venue { get; set; }

        [Required]
        [Display(Name = "Venue")]
        public int VenueId { get; set; }

        public Facilitator Facilitator { get; set; }

        [Required]
        [Display(Name = "Facilitator")]
        public int FacilitatorId { get; set; }

        //[Required]
        [Display(Name = "Contact's First Name")]
        public string ContactFirstName { get; set; }

        //[Required]
        [Display(Name = "Contact's Last Name")]
        public string ContactLastName { get; set; }

        //[Required]
        //[EmailAddress]
        [Display(Name = "Contact's Email")]
        public string ContactEmail { get; set; }

        //[Required]
        //[Phone]
        [Display(Name = "Contact's Phone Number")]
        public string ContactPhoneNumber { get; set; }

        [Display(Name = "Date Added")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateAdded { get; set; }



    }
}
