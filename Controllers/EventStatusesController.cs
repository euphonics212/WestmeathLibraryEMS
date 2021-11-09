using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Models;

namespace WestmeathLibraryEMS.Controllers
{
    public class EventStatusesController : Controller
    {
        private IRepository repository;

        public EventStatusesController(IRepository repo)
        {
            repository = repo;
        }
        public IActionResult Index() => View(repository.EventStatuses);


        public IActionResult New()
        {
            var eventStatus = new EventStatus();
            return View(eventStatus);
        }

        public IActionResult Edit(int id)
        {
            var eventStatus = repository.EventStatuses.SingleOrDefault(t => t.Id == id);
            if (eventStatus == null)
                return RedirectToAction("Error", "Shared");

            return View(eventStatus);
        }

        public ActionResult Details(int id)
        {
            var eventStatus = repository.EventStatuses.SingleOrDefault(t => t.Id == id);
            if (eventStatus == null)
                return RedirectToAction("Error", "Shared");

            return View(eventStatus);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EventStatus eventStatus)
        {
            if (!ModelState.IsValid)
            {
                eventStatus = new EventStatus();
                return RedirectToAction("New", "EventStatuses", eventStatus);
            }
            if (eventStatus.Id == 0)
                repository.SaveEventStatus(eventStatus);
            else
            {
                var eventStatusInDb = repository.EventStatuses.Single(t => t.Id == eventStatus.Id);
                eventStatusInDb.EventStatusName = eventStatus.EventStatusName;

            }

            repository.CreateEventStatus(eventStatus);
            return RedirectToAction("Index", "EventStatuses");
        }

        public ActionResult Delete(int? id)
        {
            var eventStatus = repository.EventStatuses.SingleOrDefault(t => t.Id == id);
            if (eventStatus == null)
                return RedirectToAction("Error", "Shared");
            if (id == null)
            {
                if (eventStatus == null)
                    return RedirectToAction("Error", "Shared");
            }
            else
                repository.RemoveEventStatus(eventStatus);
            return View(eventStatus);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventStatus eventStatus = repository.EventStatuses.Single(t => t.Id == id);
            repository.DeleteEventStatus(eventStatus);
            return RedirectToAction("Index");
        }
    }
}
