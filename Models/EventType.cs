using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WestmeathLibraryEMS.Models
{
    public class EventType
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Event Type Name")]
        public string TypeName { get; set; }
    }
}
