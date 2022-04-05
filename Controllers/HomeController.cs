using CrashBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Controllers
{
    public class HomeController : Controller
    {
        private ICrashRepository repo { get; set; }

        public HomeController(ICrashRepository temp)
        {
            repo = temp;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CrashData()
        {
            var crashes = repo.Crashes
                .Include(x => x.CITY)
                .Include(x => x.COUNTY)
                .Include(x => x.SEVERITY)
                .Take(100)
                .ToList();
            return View(crashes);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
