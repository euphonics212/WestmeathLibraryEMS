using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using WestmeathLibraryEMS.Models;
using Microsoft.EntityFrameworkCore;
using WestmeathLibraryEMS.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;


namespace WestmeathLibraryEMS.Controllers
{
    public class HomeController : Controller
    {
        private IRepository repository;
        private IHttpContextAccessor _httpContextAccessor;

        public HomeController(IRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            repository = repo;
            _httpContextAccessor = httpContextAccessor;

        }



        public IActionResult Index()
        {

            CheckPendingEvents(); // check if any events are pending closure
            CheckUpcomingEvents();
            var eventDay = repository.EventDays
                            .Include(e => e.Event)
                            .Include(e => e.Event.Venue)
                            .Include(e => e.EventStatus)
                            .ToList();
            return View(eventDay);
        }

        public IActionResult NewEvent()
        {
            return RedirectToAction("New", "Events");

        }
        public IActionResult ViewEvents()
        {
            return RedirectToAction("Index", "Events");

        }

        public IActionResult Upcoming()
        {

            CheckPendingEvents();

            return RedirectToAction("UpcomingEvents", "Events");

        }

        public IActionResult Pending()
        {

            return RedirectToAction("PendingEventsStatus", "Events");

        }
        public IActionResult Canceled()
        {
            return RedirectToAction("CanceledEvents", "Events");

        }

        public IActionResult Closed()
        {
            return RedirectToAction("ClosedEvents", "Events");

        }

        public IActionResult AddEventMarketing()
        {
            return RedirectToAction("New", "EventMarketing");

        }
        public IActionResult ViewEventMarketing()
        {
            return RedirectToAction("Index", "EventMarketing");

        }


        [Authorize]
        public IActionResult SystemSetup()
        {
            return View();
        }

        [Authorize]
        public IActionResult GenerateReports()
        {
            var viewModel = new VenueEventTypeViewModel
            {
                Venues = repository.Venues.ToList(),
                EventTypes = repository.EventTypes.ToList(),
                EventStatuses = repository.EventStatuses.ToList(),
                Events = repository.Events.ToList(),
                EventDays = repository.EventDays.ToList()
            };


            return View(viewModel);
        }

        [Authorize]
        public IActionResult ViewActivityLog()
        {
            var activityLog = repository.UserActivities.ToList();
            return View(activityLog);
        }

        [HttpGet]
        public JsonResult GetEventDates()
        {

            var eventDay = repository.EventDays
                 .Include(e => e.Event)
                 .Include(e => e.Event.Venue)
                 .Include(e => e.EventStatus)
                 .ToList();

            var eventJsonList = Enumerable.Empty<object>().Select(e => new { id = Convert.ToInt64(0), title = "title", start = "start", end = "end", venue = "venue", guid = "guid", status = "status" }).ToList();

            eventJsonList.Clear();

            foreach (var item in eventDay)
            {
                eventJsonList.Add(new
                {
                    id = Convert.ToInt64(item.Event.Id),
                    title = item.Event.EventName,
                    start = item.EventDate.Date.ToString("yyyy,MM,dd"),
                    end = item.EndDate.Date.AddDays(1).ToString("yyyy,MM,dd"),// add 1 day to end for calendar to show inclusive days on the event
                    venue = item.Event.Venue.VenueName,
                    guid = item.Event.Guid,
                    status = item.EventStatus.EventStatusName
                });
            };

            string result = JsonConvert.SerializeObject(eventDay);

            Console.WriteLine(eventJsonList);

            return Json(eventJsonList);
        }

        //ensures that all closed and canceled events do not store the contact details (GDPR)

        public void CheckPendingEvents()
        {
            var events = repository.EventDays
               .Include(e => e.Event)
               .Where(e => e.EventStatus.EventStatusName == "Upcoming" && e.EndDate <= DateTime.Now)
               .ToList();

            if (events.Count != 0)
            {
                foreach (var item in events)
                {
                    item.EventStatusId = repository.EventStatuses.Where(e => e.EventStatusName == "Pending").Select(e => e.Id).SingleOrDefault();

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
        public ActionResult Help()
        {
            return View();
        }


    }
}
