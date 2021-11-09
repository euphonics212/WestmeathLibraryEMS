using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WestmeathLibraryEMS.Models
{
    public class EventStatus
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="Event Status Name")]
        public string EventStatusName { get; set; }
    }
}
