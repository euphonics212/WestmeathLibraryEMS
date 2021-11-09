using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WestmeathLibraryEMS.Models
{
    public interface IRepository
    {
        IQueryable<Venue> Venues { get; }

        IQueryable<EventType> EventTypes { get; }

        IQueryable<Facilitator> Facilitators { get; }

        IQueryable<MarketingType> MarketingTypes { get; }

        IQueryable<EventStatus> EventStatuses { get; }

        IQueryable<Event> Events { get; }
        IQueryable<EventDay> EventDays { get; }

        IQueryable<EventMarketing> EventMarketings { get; }
        IQueryable<UserActivity> UserActivities { get; }

        //.....................................

        void SaveUserActivity(UserActivity e);
        void CreateUserActivity(UserActivity e);

        void RemoveUserActivity(UserActivity e);
        void DeleteUserActivity(UserActivity e);

        void UpdateUserActivity(UserActivity e);

        //.....................................

        void SaveEventMarketing(EventMarketing e);
        void CreateEventMarketing(EventMarketing e);

        void RemoveEventMarketing(EventMarketing e);
        void DeleteEventMarketing(EventMarketing e);

        void UpdateEventMarketing(EventMarketing e);

        //.....................................

        void SaveEventDay(EventDay e);
        void CreateEventDay(EventDay e);
        void RemoveEventDay(EventDay e);
        void DeleteEventDay(EventDay e);

        //.....................................

        void SaveEvent(Event e);
        void CreateEvent(Event e);
        void RemoveEvent(Event e);
        void DeleteEvent(Event e);

        //.....................................

        void SaveEventStatus(EventStatus s);
        void CreateEventStatus(EventStatus s);
        void RemoveEventStatus(EventStatus s);
        void DeleteEventStatus(EventStatus s);

        //.....................................

        void SaveMarketingType(MarketingType m);
        void CreateMarketingType(MarketingType m);
        void RemoveMarketingType(MarketingType m);
        void DeleteMarketingType(MarketingType m);


        //.....................................
        void SaveFacilitatorType(Facilitator f);
        void CreateFacilitatorType(Facilitator f);
        void RemoveFacilitatorType(Facilitator f);
        void DeleteFacilitatorType(Facilitator f);

        //.....................................
        void SaveEventType(EventType t);
        void CreateEventType(EventType t);
        void RemoveEventType(EventType t);
        void DeleteEventType(EventType t);
        void UpdateEventType(EventType t);
        //.....................................
        void SaveVenue(Venue v);
        void CreateVenue(Venue v);
        void RemoveVenue(Venue v);
        void DeleteVenue(Venue v);
        void UpdateVenue(Venue v);


        void UpdateEvent(Event e);
        void UpdateEventDay(EventDay e);

        void DetachEventDayEntities(EventDay e);

        //void PendingUpdate(EventDay eventDay);
    }
}
