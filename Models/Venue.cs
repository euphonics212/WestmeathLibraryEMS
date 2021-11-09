using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WestmeathLibraryEMS.Models
{
    public class Venue
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a name for the venue")]
        [Display(Name = "Venue Name")]
        public string VenueName { get; set; }

        [Required(ErrorMessage = "Please enter an address for the venue")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter the coordinates")]
        [RegularExpression(@"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?),\s*[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$", ErrorMessage = "Invalid Coordinates")]
        public string Coordinates { get; set; }

        [Required(ErrorMessage = "Please enter the Eircode")]
        [RegularExpression("^[A-Z0-9, ]{8}$", ErrorMessage = "Invalid Eircode")]
        public string Eircode { get; set; }

    }
}
