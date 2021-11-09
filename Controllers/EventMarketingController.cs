using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static MoreLinq.Extensions.LagExtension;
using static MoreLinq.Extensions.LeadExtension;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Models;
using WestmeathLibraryEMS.ViewModels;

namespace WestmeathLibraryEMS.Controllers
{
    public class EventMarketingController : Controller
    {
        private IRepository repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventMarketingController(IRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            repository = repo;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            var eventMarketing = repository.EventMarketings
                .Include(e => e.Event)
                .Include(e => e.MarketingType)
                .ToList();
            return View(eventMarketing);
        }

        public IActionResult New()
        {
            var viewModel = new EventMarketingFormViewModel
            {
                EventMarketing = new EventMarketing(),

                Events = repository.Events.OrderBy(e => e.DateAdded).Include(e => e.Venue).ToList(),
                MarketingTypes = repository.MarketingTypes.ToList()
            };

            return View(viewModel);
        }

        public IActionResult Edit(int id)
        {
            var eventMarketing = repository.EventMarketings
                .Include(e => e.Event)
                .Include(e => e.MarketingType)
                .Include(e => e.Event.Venue)
                .SingleOrDefault(e => e.Id == id);

            if (eventMarketing == null)
                return RedirectToAction("Error", "Shared");

            var viewModel = new EventMarketingFormViewModel()
            {
                EventMarketing = eventMarketing,

                Events = repository.Events.OrderBy(e => e.DateAdded).Include(e => e.Venue).ToList(),
                MarketingTypes = repository.MarketingTypes.ToList()
            };

            return View(viewModel);
        }


        public IActionResult Details(int id)
        {
            var eventMarketing = repository.EventMarketings
                .Include(e => e.Event)
                .Include(e => e.MarketingType)
                .SingleOrDefault(e => e.Id == id);

            if (eventMarketing.Url == null)
            {
                eventMarketing.Url = string.Concat(this.Request.Scheme, "://", this.Request.Host, this.Request.Path, this.Request.QueryString);
            }

            if (eventMarketing == null)
                return RedirectToAction("Error", "Shared");

            var viewModel = new EventMarketingFormViewModel()
            {
                EventMarketing = eventMarketing,
                Events = repository.Events.ToList(),
                MarketingTypes = repository.MarketingTypes.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EventMarketingFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("New", "EventMarkting", viewModel);
            }

            if (viewModel.EventMarketing.Id == 0)
            {


                viewModel = CheckEventDuplicates(viewModel);

                if (viewModel != null)
                {
                    viewModel.EventMarketing.DateAdded = DateTime.Now;

                    repository.SaveEventMarketing(viewModel.EventMarketing);
                    repository.CreateEventMarketing(viewModel.EventMarketing);

                    var marketingTypes = repository.MarketingTypes.Single(e => e.Id == viewModel.EventMarketing.MarketingTypeId);
                    var events = repository.Events.Single(e => e.Id == viewModel.EventMarketing.EventId);

                    LogActivity("New Event Marketing added: " + marketingTypes.MarketingTypeName + events.EventName);
                    GC.Collect();
                }
                else
                {
                    ModelState.AddModelError("Error", "Event marketing type already exists for this event");

                    return View("CustomError", "Shared");
                }

            }
            else
            {


                if (viewModel == null)
                {
                    ModelState.AddModelError("Error", "Possible duplicate Event Marketing combination");
                    return RedirectToAction("Error", "Shared");
                }

                var EventMarketingInDb = repository.EventMarketings.Single(m => m.Id == viewModel.EventMarketing.Id);

                EventMarketingInDb.DateAdded = viewModel.EventMarketing.DateAdded;
                EventMarketingInDb.EventId = viewModel.EventMarketing.EventId;
                EventMarketingInDb.MarketingTypeId = viewModel.EventMarketing.MarketingTypeId;
                EventMarketingInDb.Url = viewModel.EventMarketing.Url;

                repository.UpdateEventMarketing(viewModel.EventMarketing);
                repository.CreateEventMarketing(viewModel.EventMarketing);

                var marketingTypes = repository.MarketingTypes.Single(e => e.Id == viewModel.EventMarketing.MarketingTypeId);
                var events = repository.Events.Single(e => e.Id == viewModel.EventMarketing.EventId);


                LogActivity("Event Marketing edited: " + marketingTypes.MarketingTypeName + " | " + events.EventName);
            }

            return RedirectToAction("Index", "EventMarketing");
        }

        public IActionResult Delete(int id)
        {
            var eventMarketing = repository.EventMarketings
                .Include(e => e.Event)
                .Include(e => e.MarketingType)
                .SingleOrDefault(e => e.Id == id);



            var viewModel = new EventMarketingFormViewModel()
            {
                EventMarketing = eventMarketing,

                Events = repository.Events.ToList(),
                MarketingTypes = repository.MarketingTypes.ToList()
            };

            if (eventMarketing == null)
                return RedirectToAction("Error", "Shared");
            else
            {
                repository.RemoveEventMarketing(viewModel.EventMarketing);
                return View(viewModel);
            }

        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            EventMarketing eventMarketing = repository.EventMarketings.Single(t => t.Id == id);

            LogActivity("Event Marketing deleted: ");

            repository.DeleteEventMarketing(eventMarketing);

            return RedirectToAction("DeleteEventMarketingList");
        }

        public IActionResult DeleteEventMarketingList()
        {
            var eventMarketing = repository.EventMarketings
                .Include(e => e.Event)
                .Include(e => e.MarketingType)
                .ToList();
            return View(eventMarketing);
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

        public EventMarketingFormViewModel CheckEventDuplicates(EventMarketingFormViewModel v)
        {

            //var eventId = repository.EventMarketings.Single(e => e.MarketingTypeId == v.EventMarketing.MarketingTypeId).MarketingTypeId;
            //var typeId = repository.MarketingTypes.Single(e => e.Id == v.EventMarketing.MarketingTypeId);


            var eventMarketing = repository.EventMarketings.ToList();
            if (eventMarketing != null)
            {
                foreach (var item in eventMarketing)
                {
                    if (item.MarketingTypeId == v.EventMarketing.MarketingTypeId && item.EventId == v.EventMarketing.EventId)
                    {
                        return null;
                    }

                }
                return v;
            }
            else
            {
                return null;
            }


        }


    }
}
