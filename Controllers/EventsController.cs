using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Models;
using WestmeathLibraryEMS.ViewModels;

namespace WestmeathLibraryEMS.Controllers
{
    public class EventsController : Controller
    {

        private IRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventsController(IRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            repository = repo;
            _httpContextAccessor = httpContextAccessor;
        }


        //GUID generator
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public IActionResult Index()
        {
            CheckPendingEvents();
            CheckUpcomingEvents();
            var eventDay = repository.EventDays
                .Include(e => e.Event)
                .Include(e => e.Event.Venue)
                .Include(e => e.EventStatus)
                .ToList();
            return View(eventDay);
        }

        public IActionResult New()
        {
            var viewModel = new EventFormViewModel
            {
                Event = new Event(),

                EventDay = new EventDay(),


                EventStatuses = repository.EventStatuses.Where(e => e.Id == 4).ToList(),
                EventTypes = repository.EventTypes.OrderBy(m => m.TypeName).ToList(),
                Venues = repository.Venues.OrderBy(m => m.VenueName).ToList(),
                Facilitators = repository.Facilitators.OrderBy(m => m.FacilitatorType).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Edit(int id)
        {

            var eventDay = repository.EventDays
                   .Include(e => e.Event)
                   .Include(e => e.Event.Venue)
                   .Include(e => e.Event.EventType)
                   .Include(e => e.EventStatus)
                   .SingleOrDefault(e => e.Id == id);



            if (eventDay == null)
                return RedirectToAction("Error", "Shared");

            var viewModel = new EventFormViewModel()
            {
                EventDay = eventDay,

                Event = eventDay.Event,

                EventStatuses = repository.EventStatuses.ToList(),
                EventTypes = repository.EventTypes.OrderBy(m => m.TypeName).ToList(),
                Venues = repository.Venues.OrderBy(m => m.VenueName).ToList(),
                Facilitators = repository.Facilitators.OrderBy(m => m.FacilitatorType).ToList()


            };

            return View(viewModel);
        }

        public ActionResult MultiDay(int id)
        {
            CheckPendingEvents();

            var eventDay = repository.EventDays
                    .Include(e => e.Event)
                    .Include(e => e.Event.Venue)
                    .Include(e => e.Event.EventType)
                    .Include(e => e.EventStatus)
                    .SingleOrDefault(e => e.Id == id);



            if (eventDay == null)
                return RedirectToAction("Error", "Shared");

            var viewModel = new EventFormViewModel()
            {
                EventDay = eventDay,

                Event = eventDay.Event,

                EventStatuses = repository.EventStatuses.ToList(),
                EventTypes = repository.EventTypes.OrderBy(m => m.TypeName).ToList(),
                Venues = repository.Venues.OrderBy(m => m.VenueName).ToList(),
                Facilitators = repository.Facilitators.OrderBy(m => m.FacilitatorType).ToList()


            };

            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            CheckPendingEvents();
            var eventDay = repository.EventDays
                   .Include(e => e.Event)
                   .Include(e => e.Event.Venue)
                   .Include(e => e.Event.Facilitator)
                   .Include(e => e.Event.EventType)
                   .Include(e => e.EventStatus)
                   .SingleOrDefault(e => e.Event.Id == id);



            if (eventDay == null)
                return RedirectToAction("Error", "Shared");

            var viewModel = new EventFormViewModel()
            {
                EventDay = eventDay,

                Event = eventDay.Event,

                EventStatuses = repository.EventStatuses.ToList(),
                EventTypes = repository.EventTypes.OrderBy(m => m.TypeName).ToList(),
                Venues = repository.Venues.OrderBy(m => m.VenueName).ToList(),
                Facilitators = repository.Facilitators.OrderBy(m => m.FacilitatorType).ToList()


            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EventFormViewModel viewModel)
        {
            if (viewModel.EventDay.EndDate < viewModel.EventDay.EventDate || viewModel.EventDay.EndDate == null)
            {
                viewModel.EventDay.EndDate = viewModel.EventDay.EventDate;
            }
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New", "Events", viewModel);
            }



            if (viewModel.EventDay.Id == 0 && viewModel.Event.Id == 0)
            {

                viewModel.Event.DateAdded = DateTime.Now;
                viewModel.EventDay.DateAdded = DateTime.Now;

                //Id code for events 
                if (String.IsNullOrWhiteSpace(viewModel.Event.Guid))
                    viewModel.Event.Guid = RandomString(8);


                if (viewModel.Event.Requirements == null)
                    viewModel.Event.Requirements = "no requirements";

                //default values for contact details
                if (viewModel.Event.ContactFirstName == null)
                    viewModel.Event.ContactFirstName = "not given";

                if (viewModel.Event.ContactLastName == null)
                    viewModel.Event.ContactLastName = "not given";

                if (viewModel.Event.ContactEmail == null)
                    viewModel.Event.ContactEmail = "not given";

                if (viewModel.Event.ContactPhoneNumber == null)
                    viewModel.Event.ContactPhoneNumber = "not given";



                viewModel = CheckEventDuplicates(viewModel);
                if (viewModel != null)
                {
                    repository.SaveEvent(viewModel.Event);
                    repository.CreateEvent(viewModel.Event);

                    viewModel.EventDay.EventId = viewModel.Event.Id;

                    repository.SaveEventDay(viewModel.EventDay);
                    repository.CreateEventDay(viewModel.EventDay);


                    LogActivity("New event added: " + viewModel.Event.EventName + " | Code:" + viewModel.Event.Guid);
                }
                else
                {
                    ModelState.AddModelError("Error", "An Event already exists with that name, date and location");
                    string msg = ModelState.ToString();
                    return View("CustomError", msg);
                }


            }
            else
            {


                //set Event Id in EventDay to the edited event Id
                viewModel.EventDay.EventId = viewModel.Event.Id;


                if (viewModel.Event.Requirements == null)
                    viewModel.Event.Requirements = "no requirements";

                if (viewModel.Event.TiesInWith == null)
                    viewModel.Event.TiesInWith = "does not tie in";

                //default values for contact details
                if (viewModel.Event.ContactFirstName == null)
                    viewModel.Event.ContactFirstName = "no name given";

                if (viewModel.Event.ContactLastName == null)
                    viewModel.Event.ContactLastName = "no name given";

                if (viewModel.Event.ContactEmail == null)
                    viewModel.Event.ContactEmail = "no email given";

                if (viewModel.Event.ContactPhoneNumber == null)
                    viewModel.Event.ContactPhoneNumber = "no number given";

                //track event status changes log
                var eventStatusLog = "No Change";

                var eventDayInDb = repository.EventDays.Single(e => e.Id == viewModel.EventDay.Id);


                //if changes were made to the event status then log the changes to the activity log
                if (viewModel.EventDay.EventStatusId != eventDayInDb.EventStatusId)
                {

                    eventDayInDb.EventStatusId = viewModel.EventDay.EventStatusId;
                    var statusId = repository.EventStatuses.Where(e => e.Id == eventDayInDb.EventStatusId).SingleOrDefault();
                    eventStatusLog = statusId.EventStatusName;
                }



                eventDayInDb.DateAdded = viewModel.EventDay.DateAdded;
                eventDayInDb.EventId = viewModel.Event.Id;
                eventDayInDb.EndTime = viewModel.EventDay.EndTime;
                eventDayInDb.StartTime = viewModel.EventDay.StartTime;
                eventDayInDb.EventDate = viewModel.EventDay.EventDate;
                eventDayInDb.EventStatusId = viewModel.EventDay.EventStatusId;



                repository.UpdateEventDay(viewModel.EventDay);
                repository.CreateEventDay(viewModel.EventDay);

                var eventInDb = repository.Events.Single(e => e.Id == viewModel.Event.Id);
                eventInDb.EventName = viewModel.Event.EventName;
                eventInDb.Requirements = viewModel.Event.Requirements;
                eventInDb.Description = viewModel.Event.Description;
                eventInDb.TiesInWith = viewModel.Event.TiesInWith;
                eventInDb.Cost = viewModel.Event.Cost;
                eventInDb.BookedEvent = viewModel.Event.BookedEvent;
                eventInDb.OnlineEvent = viewModel.Event.OnlineEvent;
                eventInDb.MaxAttendees = viewModel.Event.MaxAttendees;
                eventInDb.EventTypeId = viewModel.Event.EventTypeId;
                eventInDb.VenueId = viewModel.Event.VenueId;
                eventInDb.FacilitatorId = viewModel.Event.FacilitatorId;
                eventInDb.DateAdded = viewModel.Event.DateAdded;
                eventInDb.ContactFirstName = viewModel.Event.ContactFirstName;
                eventInDb.ContactLastName = viewModel.Event.ContactLastName;
                eventInDb.ContactEmail = viewModel.Event.ContactEmail;
                eventInDb.ContactPhoneNumber = viewModel.Event.ContactPhoneNumber;

                repository.UpdateEvent(viewModel.Event);
                repository.CreateEvent(viewModel.Event);

                LogActivity("Event edited: " + viewModel.Event.EventName + " | Code:" + viewModel.Event.Guid + " | " + "Changed status: " + eventStatusLog);


            }
            return RedirectToAction("Index", "Events");
        }

        public ActionResult Delete(int? id)
        {
            var eventDay = repository.EventDays
                   .Include(e => e.Event)
                   .Include(e => e.Event.Venue)
                   .Include(e => e.Event.Facilitator)
                   .Include(e => e.Event.EventType)
                   .Include(e => e.EventStatus)
                   .SingleOrDefault(e => e.Id == id);

            var viewModel = new EventFormViewModel()
            {
                EventDay = eventDay,

                Event = eventDay.Event,

                EventStatuses = repository.EventStatuses.ToList(),
                EventTypes = repository.EventTypes.OrderBy(m => m.TypeName).ToList(),
                Venues = repository.Venues.OrderBy(m => m.VenueName).ToList(),
                Facilitators = repository.Facilitators.OrderBy(m => m.FacilitatorType).ToList()


            };

            if (eventDay == null)
                return RedirectToAction("Error", "Shared");
            else
            {
                LogActivity("Event deleted: " + eventDay.Event.EventName + " | " + eventDay.Event.Guid);
                repository.RemoveEventDay(viewModel.EventDay);
                repository.RemoveEvent(viewModel.Event);


                return View(viewModel);
            }

        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            EventDay eventDay = repository.EventDays.Single(t => t.Id == id);
            Event eEvent = repository.Events.Single(t => t.Id == eventDay.EventId);



            repository.DeleteEvent(eEvent);


            return RedirectToAction("Index");
        }



        public IActionResult UpcomingEvents()
        {

            var eventDay = repository.EventDays
                .Include(e => e.Event)
                .Include(e => e.Event.Venue)
                .Include(e => e.EventStatus)
                .Where(e => e.EventStatus.EventStatusName == "Upcoming")
                .ToList();
            return View(eventDay);
        }

        public IActionResult PendingEventsStatus()
        {
            var events = repository.EventDays
               .Include(e => e.Event)
               .Where(e => e.EventStatus.EventStatusName == "Upcoming" && e.EventDate <= DateTime.Now)
               .ToList();

            if (events != null)
            {
                foreach (var item in events)
                {
                    repository.EventStatuses.Where(e => e.EventStatusName == "Pending").Select(e => e.Id).Single();
                    repository.UpdateEventDay(item);
                    repository.CreateEventDay(item);
                }

            }

            var eventDay = repository.EventDays
                .Include(e => e.Event)
                .Include(e => e.Event.Venue)
                .Include(e => e.EventStatus)
                .Where(e => e.EventStatus.EventStatusName == "Pending")
                .ToList();
            return View(eventDay);
        }

        public IActionResult CanceledEvents()
        {

            var eventDay = repository.EventDays
                .Include(e => e.Event)
                .Include(e => e.Event.Venue)
                .Include(e => e.EventStatus)
                .Where(e => e.EventStatus.EventStatusName == "Canceled")
                .ToList();
            return View(eventDay);
        }

        public IActionResult ClosedEvents()
        {

            var eventDay = repository.EventDays
                .Include(e => e.Event)
                .Include(e => e.Event.Venue)
                .Include(e => e.EventStatus)
                .Where(e => e.EventStatus.EventStatusName == "Closed")
                .ToList();
            return View(eventDay);
        }


        public IActionResult CancelEvent(int id)
        {
            var eventDay = repository.EventDays
                   .Include(e => e.Event)
                   .Include(e => e.Event.Venue)
                   .Include(e => e.Event.EventType)
                   .SingleOrDefault(e => e.Id == id);

            eventDay.EventStatusId = 3;
            eventDay.Event.ContactEmail = "data expired";
            eventDay.Event.ContactFirstName = "data expired";
            eventDay.Event.ContactLastName = "data expired";
            eventDay.Event.ContactPhoneNumber = "data expired";

            if (eventDay == null)
                return RedirectToAction("Error", "Shared");

            var viewModel = new EventFormViewModel()
            {

                EventDay = eventDay,

                Event = eventDay.Event,

                EventStatuses = repository.EventStatuses.Where(e => e.EventStatusName == "Canceled").ToList(),
                EventTypes = repository.EventTypes.OrderBy(m => m.TypeName).ToList(),
                Venues = repository.Venues.OrderBy(m => m.VenueName).ToList(),
                Facilitators = repository.Facilitators.OrderBy(m => m.FacilitatorType).ToList()


            };

            return View(viewModel);
        }

        public IActionResult CloseEvent(int id)
        {
            var eventDay = repository.EventDays
                   .Include(e => e.Event)
                   .Include(e => e.Event.Venue)
                   .Include(e => e.Event.EventType)
                   .Include(e => e.EventStatus)
                   .SingleOrDefault(e => e.Id == id);
            eventDay.EventStatusId = 1;

            eventDay.Event.ContactEmail = "data expired";
            eventDay.Event.ContactFirstName = "data expired";
            eventDay.Event.ContactLastName = "data expired";
            eventDay.Event.ContactPhoneNumber = "data expired";

            if (eventDay == null)
                return RedirectToAction("Error", "Shared");

            var viewModel = new EventFormViewModel()
            {
                EventDay = eventDay,

                Event = eventDay.Event,

                EventStatuses = repository.EventStatuses.Where(e => e.EventStatusName == "Closed").ToList(),
                EventTypes = repository.EventTypes.OrderBy(m => m.TypeName).ToList(),
                Venues = repository.Venues.OrderBy(m => m.VenueName).ToList(),
                Facilitators = repository.Facilitators.OrderBy(m => m.FacilitatorType).ToList()


            };

            return View(viewModel);
        }

        public IActionResult DeleteEventsList()
        {
            var eventDay = repository.EventDays
                .Include(e => e.Event)
                .Include(e => e.Event.Venue)
                .Include(e => e.EventStatus)
                .ToList();
            return View(eventDay);
        }


        public EventFormViewModel CheckEventDuplicates(EventFormViewModel v)
        {
            var eventDay = repository.EventDays
                .Include(e => e.Event)
                .ToList();



            foreach (var item in eventDay)
            {
                if (v.Event.EventName.ToLower() == item.Event.EventName.ToLower()
                    && v.EventDay.EventDate == item.EventDate
                    && v.Event.VenueId == item.Event.VenueId
                    )
                    return null;
            }
            return v;

        }


        private void LogActivity(string activity)
        {
            var userActivity = new UserActivity()
            {
                UserName = _httpContextAccessor.HttpContext.User.GetLoggedInUserName(),
                Description = activity,
                Date = DateTime.Now
            };

            repository.SaveUserActivity(userActivity);
            repository.CreateUserActivity(userActivity);



        }

        public void CheckPendingEvents()
        {
            var events = repository.EventDays
               .Include(e => e.Event)
               .Where(e => e.EventStatus.EventStatusName == "Upcoming" && e.EndDate <= DateTime.Now)
               .ToList();
            var statusId = repository.EventStatuses.Where(e => e.EventStatusName == "Pending").Select(e => e.Id).Single();

            if (events.Count != 0)
            {
                foreach (var item in events)
                {
                    item.EventStatusId = statusId;

                    repository.UpdateEventDay(item);
                    repository.CreateEventDay(item);
                }

            }
        }

        public void CheckUpcomingEvents()
        {
            var events = repository.EventDays
               .Include(e => e.Event)
               .Where(e => e.EventStatus.EventStatusName == "Pending" && e.EndDate > DateTime.Now)
               .ToList();

            try
            {
                if (events.Count != 0)
                {
                    foreach (var item in events)
                    {
                        item.EventStatusId = repository.EventStatuses.Where(e => e.EventStatusName == "Upcoming").Select(e => e.Id).SingleOrDefault();

                        repository.UpdateEventDay(item);
                        repository.CreateEventDay(item);
                    }

                }
            }
            catch (Exception e)
            {
                RedirectToAction("CustomError", "Shared", e.Message);
            }

        }
    }
}
