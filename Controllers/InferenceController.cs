using CrashBoard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrashBoard.Controllers
{
    public class InferenceController : Controller
    {
        private InferenceSession _session;
        private ICrashRepository repo { get; set; }

        public InferenceController(InferenceSession session, ICrashRepository temp)
        {
            _session = session;
            repo = temp;
        }

        [HttpGet]
        public IActionResult SeverityPredictor()
        {
            ViewBag.Cities = repo.Cities.OrderBy(x => x.CITY_NAME).ToList();
            ViewBag.Counties = repo.Counties.OrderBy(x => x.COUNTY_NAME).ToList();
            ViewBag.Severities = repo.Severities.ToList();
            ViewBag.SeverityPrediction = "None";
            return View(new Crash());
        }

        [HttpPost]
        public ActionResult SeverityPredictor(Crash crash)
        {
            // Function receives normal Crash data ^

            // Translating Crash data into dummy-coded variables for the severity regression model:

            var pedestrian_involved = crash.PEDESTRIAN_INVOLVED ? 1 : 0;
            var bicyclist_involved = crash.BICYCLIST_INVOLVED ? 1: 0;
            var motorcycle_involved = crash.MOTORCYCLE_INVOLVED ? 1 : 0;
            var improper_restraint = crash.IMPROPER_RESTRAINT ? 1 : 0;
            var unrestrained = crash.UNRESTRAINED ? 1 : 0;
            var dui = crash.DUI ? 1 : 0;
            var intersection_related = crash.INTERSECTION_RELATED ? 1 : 0;
            var overturn_rollover = crash.OVERTURN_ROLLOVER ? 1 : 0;
            var older_driver_involved = crash.OLDER_DRIVER_INVOLVED ? 1 : 0;
            var single_vehicle = crash.SINGLE_VEHICLE ? 1 : 0;
            var distracted_driving = crash.DISTRACTED_DRIVING ? 1 : 0;
            var drowsy_driving = crash.DROWSY_DRIVING ? 1 : 0;
            var roadway_departure = crash.ROADWAY_DEPARTURE ? 1 : 0;

            var city_SALT_LAKE_CITY = 0;
            var city_name = "none";
            if ((crash.CityId != 0))
            {
                city_name = repo.Cities.FirstOrDefault(x => x.CITY_ID == crash.CityId).CITY_NAME;
            }
            if (city_name == "Salt Lake City")
            {
                city_SALT_LAKE_CITY = 1;
            }
            var county_name_WEBER = 0;
            var county_name = "none";
            if ((crash.CountyId != 0))
            {
                county_name = repo.Counties.FirstOrDefault(x => x.COUNTY_ID == crash.CountyId).COUNTY_NAME;
            }
            if (county_name == "Weber")
            {
                county_name_WEBER = 1;
            }

            var month_10 = 0;
            var month_4 = 0;
            var month_5 = 0;
            var month_6 = 0;
            var month_7 = 0;
            var month_8 = 0;
            var month_9 = 0;
            if (crash.CRASH_DATETIME.Month == 10){ month_10 = 1; }
            else if (crash.CRASH_DATETIME.Month == 4) { month_4 = 1; }
            else if (crash.CRASH_DATETIME.Month == 5) { month_5 = 1; }
            else if (crash.CRASH_DATETIME.Month == 6) { month_6 = 1; }
            else if (crash.CRASH_DATETIME.Month == 7) { month_7 = 1; }
            else if (crash.CRASH_DATETIME.Month == 8) { month_8 = 1; }
            else if (crash.CRASH_DATETIME.Month == 9) { month_9 = 1; }

            var day_of_week_5 = 0;
            var day_of_week_6 = 0;
            if (crash.CRASH_DATETIME.DayOfWeek == DayOfWeek.Saturday)
            {
                day_of_week_5 = 1;

            } else if (crash.CRASH_DATETIME.DayOfWeek == DayOfWeek.Sunday)
            { 
                day_of_week_6 = 1;
            }

            var hour_14 = 0;
            if (crash.CRASH_DATETIME.Hour == 14)
            {
                hour_14 = 1;
            }


            // Creating new Hypothetical Crash object with this transformed crash data
            var data = new HypotheticalCrash(pedestrian_involved, bicyclist_involved, motorcycle_involved, improper_restraint, unrestrained, dui, intersection_related, overturn_rollover, older_driver_involved, single_vehicle, distracted_driving, drowsy_driving, roadway_departure, city_SALT_LAKE_CITY, county_name_WEBER, month_10, month_4, month_5, month_6, month_7, month_8, month_9, day_of_week_5, day_of_week_6, hour_14);

            // Getting Prediction using the hypothetical crash data
            var result = _session.Run(new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("float_input", data.AsTensor())
            });
            Tensor<float> score = result.First().AsTensor<float>();
            var prediction = new SeverityPrediction { PredictedValue = score.First()};
            result.Dispose();

            // Sending Prediction back to same page, to be displayed next to inputs

            var severity_prediction = 1;
            if (prediction.PredictedValue != 0)
            {
                severity_prediction = (int)Math.Round(prediction.PredictedValue);
            }

            Severity severity = repo.Severities.FirstOrDefault(x => x.SEVERITY_ID == severity_prediction);
            ViewBag.SeverityPrediction = severity.SEVERITY_DESCRIPTION;

            ViewBag.Cities = repo.Cities.OrderBy(x => x.CITY_NAME).ToList();
            ViewBag.Counties = repo.Counties.OrderBy(x => x.COUNTY_NAME).ToList();
            ViewBag.Severities = repo.Severities.ToList();

            return View("SeverityPredictor", crash);
        }
       
    }
}
