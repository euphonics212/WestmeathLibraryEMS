using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Models;

namespace WestmeathLibraryEMS.Controllers
{
    public class MarketingTypesController : Controller
    {
        private IRepository repository;

        public MarketingTypesController(IRepository repo)
        {
            repository = repo;
        }
        public IActionResult Index() => View(repository.MarketingTypes);

        public IActionResult New()
        {
            var marketingType = new MarketingType();
            return View(marketingType);
        }
        public IActionResult Edit(int id)
        {
            var marketingType = repository.MarketingTypes.SingleOrDefault(t => t.Id == id);
            if (marketingType == null)
                return RedirectToAction("Error", "Shared");

            return View(marketingType);
        }
        public ActionResult Details(int id)
        {
            var marketingType = repository.MarketingTypes.SingleOrDefault(t => t.Id == id);
            if (marketingType == null)
                return RedirectToAction("Error", "Shared");

            return View(marketingType);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MarketingType marketingType)
        {
            if (!ModelState.IsValid)
            {
                marketingType = new MarketingType();
                return RedirectToAction("New", "EventTypes", marketingType);
            }
            if (marketingType.Id == 0)
                repository.SaveMarketingType(marketingType);
            else
            {
                var marketingTypeInDb = repository.MarketingTypes.Single(t => t.Id == marketingType.Id);
                marketingTypeInDb.MarketingTypeName = marketingType.MarketingTypeName;

            }

            repository.CreateMarketingType(marketingType);
            return RedirectToAction("Index", "MarketingTypes");
        }

        public ActionResult Delete(int? id)
        {
            var marketingType = repository.MarketingTypes.SingleOrDefault(t => t.Id == id);
            if (marketingType == null)
                return RedirectToAction("Error", "Shared");
            if (id == null)
            {
                if (marketingType == null)
                    return RedirectToAction("Error", "Shared");
            }
            else
                repository.RemoveMarketingType(marketingType);
            return View(marketingType);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MarketingType marketingType = repository.MarketingTypes.Single(t => t.Id == id);
            repository.DeleteMarketingType(marketingType);
            return RedirectToAction("Index");
        }
    }
}
