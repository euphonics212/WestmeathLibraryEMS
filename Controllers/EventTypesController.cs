using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Models;
namespace WestmeathLibraryEMS.Controllers
{
    [Authorize]
    public class EventTypesController : Controller
    {
        private IRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventTypesController(IRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            repository = repo;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index() => View(repository.EventTypes);


        public IActionResult New()
        {
            var eventTypes = new EventType();
            return View(eventTypes);
        }

        public IActionResult Edit(int id)
        {
            var eventTypes = repository.EventTypes.SingleOrDefault(t => t.Id == id);
            if (eventTypes == null)
                return RedirectToAction("Error", "Shared");

            return View(eventTypes);
        }
        public ActionResult Details(int id)
        {
            var eventTypes = repository.EventTypes.SingleOrDefault(t => t.Id == id);
            if (eventTypes == null)
                return RedirectToAction("Error", "Shared");

            return View(eventTypes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EventType eventType)
        {
            if (!ModelState.IsValid)
            {
                eventType = new EventType();
                return RedirectToAction("New", "EventTypes", eventType);
            }
            if (eventType.Id == 0)
            {
                var checkedEvent = CheckEventDuplicates(eventType);

                if (checkedEvent != null)
                {
                    repository.SaveEventType(eventType);
                    repository.CreateEventType(eventType);

                    LogActivity("New Event Type added: " + " | " + eventType.TypeName);
                }
                else
                {
                    ModelState.AddModelError("Error", "An Event Type already exists with that name.");
                    string msg = ModelState.ToString();
                    return View("CustomError", msg);
                }

            }
            else
            {
                var eventTypeInDb = repository.EventTypes.Single(t => t.Id == eventType.Id);
                eventTypeInDb.TypeName = eventType.TypeName;

                repository.UpdateEventType(eventType);
                repository.CreateEventType(eventType);

                LogActivity("Event Type edited: " + " | " + eventType.TypeName);
            }
           ;
            return RedirectToAction("Index", "EventTypes");
        }

        public ActionResult Delete(int? id)
        {
            var eventType = repository.EventTypes.SingleOrDefault(t => t.Id == id);
            if (eventType == null)
                return RedirectToAction("Error", "Shared");
            if (id == null)
            {
                if (eventType == null)
                    return RedirectToAction("Error", "Shared");
            }
            else
                repository.RemoveEventType(eventType);
            return View(eventType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventType eventType = repository.EventTypes.Single(t => t.Id == id);
            repository.DeleteEventType(eventType);

            LogActivity("Event Type deleted: " + " | " + eventType.TypeName);
            return RedirectToAction("Index");
        }

        public EventType CheckEventDuplicates(EventType v)
        {
            var eventType = repository.EventTypes
                .ToList();



            foreach (var item in eventType)
            {
                if (v.TypeName.ToLower().Trim() == item.TypeName.ToLower().Trim()

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
    }
}
