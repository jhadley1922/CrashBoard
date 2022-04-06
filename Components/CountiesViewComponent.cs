using CrashBoard.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Components
{
    public class CountiesViewComponent : ViewComponent
    {
        private ICrashRepository repo { get; set; }

        public CountiesViewComponent(ICrashRepository temp)
        {
            repo = temp;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCounty = RouteData?.Values["county"];

            var counties = repo.Counties.ToList();

            return View(counties);

        }
    }
}
