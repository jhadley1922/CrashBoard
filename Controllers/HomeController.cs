using CrashBoard.Models;
using CrashBoard.Models.ViewModels;
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

        public IActionResult CrashData(int county, int pageNum)
        {
            int pageSize = 50;

            ViewBag.PageNum = pageNum;
            ViewBag.SelectedCounty = county;
            ViewBag.TotalPages = (int)Math.Ceiling((double)repo.Crashes.Count() / pageSize);

            var cvm = new CrashesViewModel
            {
                Crashes = repo.Crashes
                    .Include(x => x.CITY)
                    .Include(x => x.COUNTY)
                    .Include(x => x.SEVERITY)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize)
            };
            if (county != 0)
            {
                cvm = new CrashesViewModel
                {
                    Crashes = repo.Crashes
                        .Include(x => x.CITY)
                        .Include(x => x.COUNTY)
                        .Include(x => x.SEVERITY)
                        .Where(x => x.CountyId == county)
                        .Skip((pageNum - 1) * pageSize)
                        .Take(pageSize)
                };
                ViewBag.TotalPages = (int)Math.Ceiling((double)repo.Crashes.Where(x => x.CountyId == county).Count() / pageSize);
            }
            return View(cvm);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
