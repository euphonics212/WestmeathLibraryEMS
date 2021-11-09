using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WestmeathLibraryEMS.Models
{
    public class MarketingType
    {
        public int Id { get; set; }
        [Required]
        [Display(Name ="Marketing Type Name")]
        public string MarketingTypeName { get; set; }
    }
}
