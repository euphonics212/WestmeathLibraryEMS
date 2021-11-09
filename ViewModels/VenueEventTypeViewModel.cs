using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Models;

namespace WestmeathLibraryEMS.ViewModels
{
    public class VenueEventTypeViewModel
    {
        public IEnumerable<EventType> EventTypes { get; set; }
        public IEnumerable<Venue> Venues { get; set; }
        public IEnumerable<EventStatus> EventStatuses { get; set; }

        public IEnumerable<Event> Events { get; set; }

        public IEnumerable<EventDay> EventDays { get; set; }

    }
}
