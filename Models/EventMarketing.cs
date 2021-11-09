using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WestmeathLibraryEMS.Models
{
    public class EventMarketing
    {
        public int Id { get; set; }

        public MarketingType MarketingType { get; set; }

        [Required]
        [Display(Name = "Marketing Type")]
        public int MarketingTypeId { get; set; }


        public Event Event { get; set; }

        [Required]
        [Display(Name = "Event")]
        public int EventId { get; set; }

        [Url]
        public string Url { get; set; }

        [Display(Name = "Date Added")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DateAdded { get; set; }

    }
}
