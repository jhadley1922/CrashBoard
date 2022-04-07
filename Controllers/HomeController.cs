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


        // Gets index page
        public IActionResult Index()
        {
            return View();
        }

        //Gets CrashData page, displays crashes, can filter and search page
        public IActionResult CrashData(int severity, int pageNum, string searchString)
        {
            int pageSize = 50;

            ViewBag.PageNum = pageNum;
            ViewBag.SelectedSeverity = severity;
            ViewBag.TotalPages = (int)Math.Ceiling((double)repo.Crashes.Count() / pageSize);
            ViewBag.SearchString = searchString;

            var cvm = new CrashesViewModel
            {
                Crashes = repo.Crashes
                    .Include(x => x.CITY)
                    .Include(x => x.COUNTY)
                    .Include(x => x.SEVERITY)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize)
            };

            if (severity != 0 && !String.IsNullOrEmpty(searchString))
            {
                cvm = new CrashesViewModel
                {
                    Crashes = repo.Crashes
                        .Include(x => x.CITY)
                        .Include(x => x.COUNTY)
                        .Include(x => x.SEVERITY)
                        .Where(x => x.SeverityId == severity)
                        .Where(x => x.CRASH_ID.Contains(searchString)
                               || x.CITY.CITY_NAME.Contains(searchString)
                               || x.COUNTY.COUNTY_NAME.Contains(searchString)
                               || x.MAIN_ROAD_NAME.Contains(searchString))
                        .Skip((pageNum - 1) * pageSize)
                        .Take(pageSize)
                };
                ViewBag.TotalPages = (int)Math.Ceiling((double)repo.Crashes
                        .Where(x => x.SeverityId == severity)
                        .Where(x => x.CRASH_ID.Contains(searchString)
                               || x.CITY.CITY_NAME.Contains(searchString)
                               || x.COUNTY.COUNTY_NAME.Contains(searchString)
                               || x.MAIN_ROAD_NAME.Contains(searchString))
                        .Count() / pageSize);
            }
            else if (!String.IsNullOrEmpty(searchString))
            {
                cvm = new CrashesViewModel
                {
                    Crashes = repo.Crashes
                        .Include(x => x.CITY)
                        .Include(x => x.COUNTY)
                        .Include(x => x.SEVERITY)
                        .Where(x => x.CRASH_ID.Contains(searchString)
                               || x.CITY.CITY_NAME.Contains(searchString)
                               || x.COUNTY.COUNTY_NAME.Contains(searchString)
                               || x.MAIN_ROAD_NAME.Contains(searchString))
                        .Skip((pageNum - 1) * pageSize)
                        .Take(pageSize)
                };
                ViewBag.TotalPages = (int)Math.Ceiling((double)repo.Crashes
                         .Where(x => x.CRASH_ID.Contains(searchString)
                               || x.CITY.CITY_NAME.Contains(searchString)
                               || x.COUNTY.COUNTY_NAME.Contains(searchString)
                               || x.MAIN_ROAD_NAME.Contains(searchString))
                         .Count() / pageSize);
            }
            else if(severity != 0)
            {
                cvm = new CrashesViewModel
                {
                    Crashes = repo.Crashes
                        .Include(x => x.CITY)
                        .Include(x => x.COUNTY)
                        .Include(x => x.SEVERITY)
                        .Where(x => x.SeverityId == severity)
                        .Skip((pageNum - 1) * pageSize)
                        .Take(pageSize)
                };
                ViewBag.TotalPages = (int)Math.Ceiling((double)repo.Crashes
                    .Where(x => x.SeverityId == severity)
                    .Count() / pageSize);
            }
            return View(cvm);
        }

        // Gets ccrash details
        public IActionResult CrashDetails(int crashpk)
        {
            var crash = repo.Crashes
                .Include(x => x.CITY)
                .Include(x => x.COUNTY)
                .Include(x => x.SEVERITY)
                .FirstOrDefault(x => x.CRASH_PK == crashpk);
            return View(crash);
        }

        //Gets Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        //Gets solution page
        public IActionResult Solution()
        {
            return View();
        }
    }
}
