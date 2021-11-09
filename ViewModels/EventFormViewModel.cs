using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Models;

namespace WestmeathLibraryEMS.ViewModels
{
    public class EventFormViewModel
    {
        public Event Event { get; set; }
        public EventDay EventDay { get; set; }
        public IEnumerable<EventStatus> EventStatuses { get; set; }
        public IEnumerable<EventType> EventTypes { get; set; }
        public IEnumerable<Venue> Venues { get; set; }
        public IEnumerable<Facilitator> Facilitators { get; set; }
    }
}
