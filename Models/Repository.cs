using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WestmeathLibraryEMS.Models
{
    public class Repository : IRepository
    {
        private EMSContext context;

        public Repository(EMSContext ctx)
        {
            context = ctx;
        }


        //.....................................
        public IQueryable<Venue> Venues => context.Venues;

        public IQueryable<EventType> EventTypes => context.EventTypes;

        public IQueryable<Facilitator> Facilitators => context.Facilitators;

        public IQueryable<MarketingType> MarketingTypes => context.MarketingTypes;

        public IQueryable<EventStatus> EventStatuses => context.EventStatuses;

        public IQueryable<Event> Events => context.Events;

        public IQueryable<EventDay> EventDays => context.EventDays;

        public IQueryable<EventMarketing> EventMarketings => context.EventMarketings;

        public IQueryable<UserActivity> UserActivities => context.UserActivities;


        //.....................................
        public void SaveEventMarketing(EventMarketing m)
        {
            context.Add(m);
        }

        public void CreateEventMarketing(EventMarketing m)
        {
            context.SaveChanges();
        }
        public void RemoveEventMarketing(EventMarketing m)
        {
            context.Remove(m);
        }
        public void DeleteEventMarketing(EventMarketing m)
        {
            context.Remove(m);
            context.SaveChanges();
        }

        public void UpdateEventMarketing(EventMarketing m)
        {
            context.Update(m);
        }
        //.

        //.....................................
        public void SaveMarketingType(MarketingType m)
        {
            context.Add(m);
        }

        public void CreateMarketingType(MarketingType m)
        {
            context.SaveChanges();
        }
        public void RemoveMarketingType(MarketingType m)
        {
            context.Remove(m);
        }
        public void DeleteMarketingType(MarketingType m)
        {
            context.Remove(m);
            context.SaveChanges();
        }
        //.....................................



        public void SaveFacilitatorType(Facilitator f)
        {
            context.Add(f);
        }

        public void CreateFacilitatorType(Facilitator f)
        {
            context.SaveChanges();
        }
        public void RemoveFacilitatorType(Facilitator f)
        {
            context.Remove(f);
        }
        public void DeleteFacilitatorType(Facilitator f)
        {
            context.Remove(f);
            context.SaveChanges();
        }

        //.....................................
        public void SaveVenue(Venue v)
        {
            context.Add(v);
        }

        public void CreateVenue(Venue v)
        {
            context.SaveChanges();

        }
        public void RemoveVenue(Venue v)
        {
            context.Remove(v);
        }
        public void DeleteVenue(Venue v)
        {
            context.Remove(v);
            context.SaveChanges();
        }
        public void UpdateVenue(Venue v)
        {
            context.Update(v);
        }

        //.....................................
        public void SaveEventType(EventType t)
        {
            context.Add(t);
        }

        public void CreateEventType(EventType t)
        {
            context.SaveChanges();
        }
        public void RemoveEventType(EventType t)
        {
            context.Remove(t);
        }
        public void DeleteEventType(EventType t)
        {
            context.Remove(t);
            context.SaveChanges();
        }
        public void UpdateEventType(EventType t)
        {
            context.Update(t);

        }
        //.....................................
        public void SaveEventStatus(EventStatus s)
        {
            context.Add(s);
        }

        public void CreateEventStatus(EventStatus s)
        {
            context.SaveChanges();
        }

        public void RemoveEventStatus(EventStatus s)
        {
            context.Remove(s);
        }

        public void DeleteEventStatus(EventStatus s)
        {
            context.Remove(s);
            context.SaveChanges();
        }


        //-----------------------------------------
        public void SaveEvent(Event e)
        {
            context.Add(e);
        }

        public void CreateEvent(Event e)
        {
            context.SaveChanges();
        }

        public void RemoveEvent(Event e)
        {
            context.Remove(e);
        }

        public void DeleteEvent(Event e)
        {
            context.Remove(e);
            context.SaveChanges();
        }


        //-----------------------------------------
        public void SaveEventDay(EventDay e)
        {
            context.Add(e);
        }

        public void CreateEventDay(EventDay e)
        {
            context.SaveChanges();
        }

        public void RemoveEventDay(EventDay e)
        {
            context.Remove(e);
        }

        public void DeleteEventDay(EventDay e)
        {
            context.Remove(e);
            context.SaveChanges();
        }



        public void UpdateEvent(Event e)
        {
            context.Update(e);

        }
        public void UpdateEventDay(EventDay e)
        {
            context.Update(e);

        }


        public void DetachEventDayEntities(EventDay e)
        {
            context.Entry(e).State = EntityState.Detached;
        }

        public void SaveUserActivity(UserActivity e)
        {
            context.Add(e);
        }

        public void CreateUserActivity(UserActivity e)
        {
            context.SaveChanges();
        }

        public void RemoveUserActivity(UserActivity e)
        {
            context.Remove(e);
        }

        public void DeleteUserActivity(UserActivity e)
        {
            context.Remove(e);
            context.SaveChanges();
        }

        public void UpdateUserActivity(UserActivity e)
        {
            context.Update(e);
        }
    }
}
