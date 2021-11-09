using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Models;


namespace WestmeathLibraryEMS.Controllers.AdminFunction
{
    [Authorize]
    public class VenuesController : Controller
    {
        private IRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VenuesController(IRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            repository = repo;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index() => View(repository.Venues);


        public IActionResult New()
        {
            var venue = new Venue();
            return View(venue);
        }

        public IActionResult Edit(int id)
        {
            var venue = repository.Venues.SingleOrDefault(v => v.Id == id);
            if (venue == null)
                return RedirectToAction("Error", "Shared");

            return View(venue);
        }
        public ActionResult Details(int id)
        {
            var venue = repository.Venues.SingleOrDefault(v => v.Id == id);
            if (venue == null)
                return RedirectToAction("Error", "Shared");

            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Venue venue)
        {
            if (!ModelState.IsValid)
            {
                venue = new Venue();
                return RedirectToAction("New", "Venues", venue);
            }
            if (venue.Id == 0)
            {
                venue = CheckVenueDuplicates(venue);


                if (venue != null)
                {
                    LogActivity("New Venue added: " + " | " + venue.VenueName);
                    repository.SaveVenue(venue);
                }
                else
                {

                    ModelState.AddModelError("Error", "A venue already exists with that name.");
                    string msg = ModelState.ToString();
                    return View("CustomError", msg);
                }



            }

            else
            {
                var venueIdInDb = repository.Venues.Single(v => v.Id == venue.Id);
                venueIdInDb.Address = venue.Address;
                venueIdInDb.Coordinates = venue.Coordinates;
                venueIdInDb.Eircode = venue.Eircode;
                venueIdInDb.VenueName = venue.VenueName;
            }

            LogActivity("New Venue edited: " + " | " + venue.VenueName);
            repository.UpdateVenue(venue);
            repository.CreateVenue(venue);



            return RedirectToAction("Index", "Venues");
        }


        [Authorize]
        public ActionResult Delete(int? id)
        {
            var venue = repository.Venues.SingleOrDefault(v => v.Id == id);
            if (venue == null)
                return RedirectToAction("Error", "Shared");
            if (id == null)
            {
                if (venue == null)
                    return RedirectToAction("Error", "Shared");
            }
            else
                repository.RemoveVenue(venue);
            return View(venue);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Venue venue = repository.Venues.Single(v => v.Id == id);

            LogActivity("Venue deleted: " + " | " + venue.VenueName);

            repository.DeleteVenue(venue);

            return RedirectToAction("Index");
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

        private Venue CheckVenueDuplicates(Venue venue)
        {
            var v = repository.Venues.ToList();
            foreach (var item in v)
            {
                if (item.VenueName == venue.VenueName || item.Eircode == venue.Eircode)
                    return null;
            };

            return venue;
        }
    }
}
