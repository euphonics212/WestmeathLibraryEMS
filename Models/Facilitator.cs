using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WestmeathLibraryEMS.Models
{
    public class Facilitator
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Facilitator Type")]
        public string FacilitatorType { get; set; }
    }
}
