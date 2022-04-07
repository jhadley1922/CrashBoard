using CrashBoard.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Components
{
    // Class that builds the severities filter on CrashData page
    public class SeveritiesViewComponent : ViewComponent
    {
        private ICrashRepository repo { get; set; }

        public SeveritiesViewComponent(ICrashRepository temp)
        {
            repo = temp;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedSeverity = RouteData?.Values["severity"];

            var severities = repo.Severities.ToList();

            return View(severities);

        }
    }
}
