using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models
{
    public class HypotheticalCrash
    {
        public float pedestrian_involved { get; set; }
        public float bicyclist_involved { get; set; }
        public float motorcycle_involved { get; set; }
        public float improper_restraint { get; set; }
        public float unrestrained { get; set; }
        public float dui { get; set; }
        public float intersection_related { get; set; }
        public float overturn_rollover { get; set; }
        public float older_driver_involved { get; set; }
        public float single_vehicle { get; set; }
        public float distracted_driving { get; set; }
        public float drowsy_driving { get; set; }
        public float roadway_departure { get; set; }
        public float city_SALT_LAKE_CITY { get; set; }
        public float county_name_WEBER { get; set; }
        public float month_10 { get; set; }
        public float month_4 { get; set; }
        public float month_5 { get; set; }
        public float month_6 { get; set; }
        public float month_7 { get; set; }
        public float month_8 { get; set; }
        public float month_9 { get; set; }
        public float day_of_week_5 { get; set; }
        public float day_of_week_6 { get; set; }
        public float hour_14 { get; set; }

        public HypotheticalCrash(float pedestrian, float bicyclist, float motorcycle, float bad_restraint, float not_restrained, float dui_involved, float intersection, float rollover, float older_driver, float one_vehicle, float distracted, float drowsy, float road_departure, float SALT_LAKE_CITY, float WEBER, float month10, float month4, float month5, float month6, float month7, float month8, float month9, float day_of_week5, float day_of_week6, float hour14)
        {
            pedestrian_involved = pedestrian;
            bicyclist_involved = bicyclist;
            motorcycle_involved = motorcycle;
            improper_restraint = bad_restraint;
            unrestrained = not_restrained;
            dui = dui_involved;
            intersection_related = intersection;
            overturn_rollover = rollover;
            older_driver_involved = older_driver;
            single_vehicle = one_vehicle;
            distracted_driving = distracted;
            drowsy_driving = drowsy;
            roadway_departure = road_departure;
            city_SALT_LAKE_CITY = SALT_LAKE_CITY;
            county_name_WEBER = WEBER;
            month_10 = month10;
            month_4 = month4;
            month_5 = month5;
            month_6 = month6;
            month_7 = month7;
            month_8 = month8;
            month_9 = month9;
            day_of_week_5 = day_of_week5;
            day_of_week_6 = day_of_week6;
            hour_14 = hour14;

        }

        public Tensor<float> AsTensor()
        {
            float[] data = new float[]
            {
                pedestrian_involved, bicyclist_involved, motorcycle_involved, improper_restraint, unrestrained, dui, intersection_related, overturn_rollover, older_driver_involved, single_vehicle, distracted_driving, drowsy_driving, roadway_departure, city_SALT_LAKE_CITY, county_name_WEBER, month_10, month_4, month_5, month_6, month_7, month_8, month_9, day_of_week_5, day_of_week_6, hour_14
            };
            int[] dimensions = new int[] { 1, 25 };
            return new DenseTensor<float>(data, dimensions);
        }
    }
}
