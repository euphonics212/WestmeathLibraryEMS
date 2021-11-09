using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WestmeathLibraryEMS.Models;

namespace WestmeathLibraryEMS.Controllers
{
    public class FacilitatorsController : Controller
    {
        private IRepository repository;

        public FacilitatorsController(IRepository repo)
        {
            repository = repo;
        }
        public IActionResult Index() => View(repository.Facilitators);

        public IActionResult New()
        {
            var facilitator = new Facilitator();
            return View(facilitator);
        }

        public IActionResult Edit(int id)
        {
            var facilitators = repository.Facilitators.SingleOrDefault(t => t.Id == id);
            if (facilitators == null)
                return RedirectToAction("Error", "Shared");

            return View(facilitators);
        }

        public ActionResult Details(int id)
        {
            var facilitators = repository.Facilitators.SingleOrDefault(t => t.Id == id);
            if (facilitators == null)
                return RedirectToAction("Error", "Shared");

            return View(facilitators);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Facilitator facilitator)
        {
            if (!ModelState.IsValid)
            {
                facilitator = new Facilitator();
                return RedirectToAction("New", "Facilitators", facilitator);
            }
            if (facilitator.Id == 0)
                repository.SaveFacilitatorType(facilitator);
            else
            {
                var FacilitatorInDb = repository.Facilitators.Single(t => t.Id == facilitator.Id);
                FacilitatorInDb.FacilitatorType = facilitator.FacilitatorType;

            }

            repository.CreateFacilitatorType(facilitator);
            return RedirectToAction("Index", "Facilitators");
        }


        public ActionResult Delete(int? id)
        {
            var facilitator = repository.Facilitators.SingleOrDefault(t => t.Id == id);
            if (facilitator == null)
                return RedirectToAction("Error", "Shared");
            if (id == null)
            {
                if (facilitator == null)
                    return RedirectToAction("Error", "Shared");
            }
            else
                repository.RemoveFacilitatorType(facilitator);
            return View(facilitator);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Facilitator facilitator = repository.Facilitators.Single(t => t.Id == id);
            repository.DeleteFacilitatorType(facilitator);
            return RedirectToAction("Index");
        }

    }
}
