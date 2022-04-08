using CrashBoard.Models;
using CrashBoard.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CrashBoard.Controllers
{
    public class HomeController : Controller
    {
        private ICrashRepository repo { get; set; }

        private IWebHostEnvironment _env { get; set; }

        public HomeController(ICrashRepository temp, IWebHostEnvironment env)
        {
            repo = temp;
            _env = env;
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

        public IActionResult Recommendations() 
        {
            return View();
        }

        public IActionResult Analysis()
        {
            string wwwPath = this._env.WebRootPath;
            string contentPath = this._env.ContentRootPath;

            // Crash Count per countyy
            List<DataPoint> CountyCounts = new List<DataPoint>();
            //Read the contents of CSV file.
            string[] CountyPath = { wwwPath, "csv", "20_county_counts.csv" };
            string csvData = System.IO.File.ReadAllText(Path.Combine(CountyPath));
            //Execute a loop over the rows.
            int i = 1;
            foreach (string row in csvData.Split("\n"))
            {
                if (i == 1)
                {
                    i++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        int count = Int32.Parse(cells[1]);
                        CountyCounts.Add(new DataPoint(cells[0], count));
                    }
                }

            }
            ViewBag.CountyCounts = JsonConvert.SerializeObject(CountyCounts);


            // Fatal Crash Count per countyy
            List<DataPoint> FatalCountyCounts = new List<DataPoint>();
            string[] FatalCountyPath = { wwwPath, "csv", "20_county_counts.csv" };
            //Read the contents of CSV file.
            csvData = System.IO.File.ReadAllText(Path.Combine(FatalCountyPath));
            //Execute a loop over the rows.
            i = 1;
            foreach (string row in csvData.Split("\n"))
            {
                if (i == 1)
                {
                    i++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        int count = Int32.Parse(cells[1]);
                        FatalCountyCounts.Add(new DataPoint(cells[0], count));
                    }
                }

            }
            ViewBag.FatalCountyCounts = JsonConvert.SerializeObject(FatalCountyCounts);

            // Crash Count by severity
            List<DataPoint> SeverityCounts = new List<DataPoint>();
            string[] SeverityPath = { wwwPath, "csv", "severity_counts_df.csv" };
            //Read the contents of CSV file.
            csvData = System.IO.File.ReadAllText(Path.Combine(SeverityPath));
            //Execute a loop over the rows.
            i = 1;
            foreach (string row in csvData.Split("\n"))
            {
                if (i == 1)
                {
                    i++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        int count = Int32.Parse(cells[1]);
                        SeverityCounts.Add(new DataPoint(cells[0], count));
                    }
                }

            }
            ViewBag.SeverityCounts = JsonConvert.SerializeObject(SeverityCounts);

            // Crash Count by City
            List<DataPoint> CityCounts = new List<DataPoint>();
            string[] CityPath = { wwwPath, "csv", "20_city_counts_df.csv" };
            //Read the contents of CSV file.
            csvData = System.IO.File.ReadAllText(Path.Combine(CityPath));
            //Execute a loop over the rows.
            i = 1;
            foreach (string row in csvData.Split("\n"))
            {
                if (i == 1)
                {
                    i++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        int count = Int32.Parse(cells[1]);
                        CityCounts.Add(new DataPoint(cells[0], count));
                    }
                }

            }
            ViewBag.CityCounts = JsonConvert.SerializeObject(CityCounts);

            // Fatal Crash Count by City
            List<DataPoint> FatalCityCounts = new List<DataPoint>();
            string[] FatalCityPath = { wwwPath, "csv", "20_fatal_city_counts_df.csv" };
            //Read the contents of CSV file.
            csvData = System.IO.File.ReadAllText(Path.Combine(FatalCityPath));
            //Execute a loop over the rows.
            i = 1;
            foreach (string row in csvData.Split("\n"))
            {
                if (i == 1)
                {
                    i++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        int count = Int32.Parse(cells[1]);
                        FatalCityCounts.Add(new DataPoint(cells[0], count));
                    }
                }

            }
            ViewBag.FatalCityCounts = JsonConvert.SerializeObject(FatalCityCounts);

            // Crash Count by Feature
            List<DataPoint> FeatureCounts = new List<DataPoint>();
            string[] FeaturePath = { wwwPath, "csv", "10_feature_counts.csv" };
            //Read the contents of CSV file.
            csvData = System.IO.File.ReadAllText(Path.Combine(FeaturePath));
            //Execute a loop over the rows.
            i = 1;
            foreach (string row in csvData.Split("\n"))
            {
                if (i == 1)
                {
                    i++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        int count = Int32.Parse(cells[2]);
                        FeatureCounts.Add(new DataPoint(cells[1], count));
                    }
                }

            }
            ViewBag.FeatureCounts = JsonConvert.SerializeObject(FeatureCounts);

            // Fatal Crash Count by Feature
            List<DataPoint> FatalFeatureCounts = new List<DataPoint>();
            string[] FatalFeaturePath = { wwwPath, "csv", "10_fatal_feature_counts.csv" };
            //Read the contents of CSV file.
            csvData = System.IO.File.ReadAllText(Path.Combine(FatalFeaturePath));
            //Execute a loop over the rows.
            i = 1;
            foreach (string row in csvData.Split("\n"))
            {
                if (i == 1)
                {
                    i++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        int count = Int32.Parse(cells[2]);
                        FatalFeatureCounts.Add(new DataPoint(cells[1], count));
                    }
                }

            }
            ViewBag.FatalFeatureCounts = JsonConvert.SerializeObject(FatalFeatureCounts);

            return View();
        }

        public IActionResult Counties()
        {
            ViewBag.Counties = repo.Counties.ToList();
            return View();
        }

        public IActionResult County(int countyId)
        {

            var County = repo.Counties.FirstOrDefault(x => x.COUNTY_ID == countyId);

            if (County is null)
            {
                County = new County();
                County.COUNTY_ID = 0;
                County.COUNTY_NAME = "NOT FOUND";
            }
            
            var year = 2019;

            // In case it can't be found, won't be null
            ViewBag.Population = "Unknown"; 
            ViewBag.PopDensity = "Unknown";

            //Utah Population chart
            List<DataPoint> PopulationCounts = new List<DataPoint>();
            string wwwPath = this._env.WebRootPath;
            string[] PopulationPath = { wwwPath, "csv", "county_populations.csv" };
            //Read the contents of CSV file.
            var csvData = System.IO.File.ReadAllText(Path.Combine(PopulationPath));
            //Loop to find correct county
            int i = 1;
            foreach (string row in csvData.Split("\n"))
            {
                if (i == 1)
                {
                    i++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        if (cells[0].ToString().ToUpper() == County.COUNTY_NAME + " COUNTY")
                        {
                            ViewBag.Population = cells[1].ToString();
                            ViewBag.PopDensity = cells[3].ToString();
                        }

                        PopulationCounts.Add(new DataPoint(cells[0].ToString(), Int32.Parse(cells[1].ToString())));
                    }
                }
            }


            ViewBag.PopulationCounts = JsonConvert.SerializeObject(PopulationCounts);

            ViewBag.NumCrashes = repo.Crashes.Where(x => x.CRASH_DATETIME.Year == year).Count(x => x.CountyId == countyId);
            ViewBag.NumFatal = repo.Crashes.Where(x => x.CountyId == countyId).Where(x => x.CRASH_DATETIME.Year == year).Count(x => x.SeverityId == 5);

            if(ViewBag.Population == "Unknown")
            {
                ViewBag.CrashesCapita = "Unknown";
                ViewBag.FatalCapita = "Unknown";
            } else
            {
                ViewBag.CrashesCapita = Math.Round(ViewBag.NumCrashes * 100 / Double.Parse(ViewBag.Population), 4);
                ViewBag.FatalCapita = Math.Round(ViewBag.NumFatal * 100 / Double.Parse(ViewBag.Population), 4);
            }

            // Crash Count per countyy
            List<DataPoint> CountyCounts = new List<DataPoint>();
            //Read the contents of CSV file.
            string[] CountyPath = { wwwPath, "csv", "20_county_counts.csv" };
            csvData = System.IO.File.ReadAllText(Path.Combine(CountyPath));
            //Execute a loop over the rows.
            i = 1;
            foreach (string row in csvData.Split("\n"))
            {
                if (i == 1)
                {
                    i++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        int count = Int32.Parse(cells[1]);
                        CountyCounts.Add(new DataPoint(cells[0], count));
                    }
                }

            }
            ViewBag.CountyCounts = JsonConvert.SerializeObject(CountyCounts);


            // Fatal Crash Count per countyy
            List<DataPoint> FatalCountyCounts = new List<DataPoint>();
            string[] FatalCountyPath = { wwwPath, "csv", "20_county_counts.csv" };
            //Read the contents of CSV file.
            csvData = System.IO.File.ReadAllText(Path.Combine(FatalCountyPath));
            //Execute a loop over the rows.
            i = 1;
            foreach (string row in csvData.Split("\n"))
            {
                if (i == 1)
                {
                    i++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        var cells = row.Split(',');
                        int count = Int32.Parse(cells[1]);
                        FatalCountyCounts.Add(new DataPoint(cells[0], count));
                    }
                }

            }
            ViewBag.FatalCountyCounts = JsonConvert.SerializeObject(FatalCountyCounts);


            return View(County);
        }
    }
}