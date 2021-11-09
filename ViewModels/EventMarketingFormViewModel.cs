using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Models;

namespace WestmeathLibraryEMS.ViewModels
{
    public class EventMarketingFormViewModel
    {
        public EventMarketing EventMarketing { get; set; }

        public IEnumerable<Event> Events { get; set; }
        public IEnumerable<MarketingType> MarketingTypes { get; set; }
    }
}
