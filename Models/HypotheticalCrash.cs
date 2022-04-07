using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models
{
    public class HypotheticalCrash
    {
        public float work_zone_related { get; set; }
        public float route_89 { get; set; }
        public float route_Other { get; set; }
        public float main_road_name_Other { get; set; }
        public float city_Other { get; set; }
        public float city_SALT_LAKE_CITY { get; set; }
        public float city_WEST_VALLEY_CITY { get; set; }
        public float county_name_Other { get; set; }
        public float county_name_SALT_LAKE { get; set; }
        public float county_name_UTAH { get; set; }
        public float county_name_WEBER { get; set; }
        public float pedestrian_involved_Other { get; set; }
        public float bicyclist_involved_Other { get; set; }
        public float improper_restraint_Other { get; set; }
        public float unrestrained_Other { get; set; }
        public float dui_Other { get; set; }
        public float intersection_related_True { get; set; }
        public float wild_animal_related_Other { get; set; }
        public float domestic_animal_related_Other { get; set; }
        public float overturn_rollover_Other { get; set; }
        public float commercial_motor_veh_involved_True { get; set; }
        public float teenage_driver_involved_True { get; set; }
        public float older_driver_involved_True { get; set; }
        public float night_dark_condition_True { get; set; }
        public float single_vehicle_True { get; set; }
        public float distracted_driving_True { get; set; }
        public float drowsy_driving_Other { get; set; }
        public float roadway_departure_True { get; set; }
        public float year_2017 { get; set; }
        public float year_2018 { get; set; }
        public float year_2019 { get; set; }
        public float month_10 { get; set; }
        public float month_11 { get; set; }
        public float month_12 { get; set; }
        public float month_3 { get; set; }
        public float month_4 { get; set; }
        public float month_5 { get; set; }
        public float month_6 { get; set; }
        public float month_7 { get; set; }
        public float month_8 { get; set; }
        public float month_9 { get; set; }
        public float day_of_week_1 { get; set; }
        public float day_of_week_2 { get; set; }
        public float day_of_week_3 { get; set; }
        public float day_of_week_4 { get; set; }
        public float day_of_week_5 { get; set; }
        public float day_of_week_6 { get; set; }
        public float hour_13 { get; set; }
        public float hour_14 { get; set; }
        public float hour_15 { get; set; }
        public float hour_16 { get; set; }
        public float hour_17 { get; set; }
        public float hour_18 { get; set; }
        public float hour_7 { get; set; }
        public float hour_8 { get; set; }
        public float hour_Other { get; set; }
        public float crash_severity_id { get; set; }


    public Tensor<float> AsTensor()
        {
            float[] data = new float[]
            {
                work_zone_related, route_89, route_Other, main_road_name_Other,
                city_Other, city_SALT_LAKE_CITY, city_WEST_VALLEY_CITY,
                county_name_Other, county_name_SALT_LAKE, county_name_UTAH,
                county_name_WEBER, pedestrian_involved_Other,
                bicyclist_involved_Other, improper_restraint_Other,
                unrestrained_Other, dui_Other, intersection_related_True,
                wild_animal_related_Other, domestic_animal_related_Other,
                overturn_rollover_Other, commercial_motor_veh_involved_True,
                teenage_driver_involved_True, older_driver_involved_True,
                night_dark_condition_True, single_vehicle_True,
                distracted_driving_True, drowsy_driving_Other,
                roadway_departure_True, year_2017, year_2018, year_2019,
                month_10, month_11, month_12, month_3, month_4, month_5,
                month_6, month_7, month_8, month_9, day_of_week_1,
                day_of_week_2, day_of_week_3, day_of_week_4, day_of_week_5,
                day_of_week_6, hour_13, hour_14, hour_15, hour_16, hour_17,
                hour_18, hour_7, hour_8, hour_Other, crash_severity_id
            };
            int[] dimensions = new int[] { 1, 8 };
            return new DenseTensor<float>(data, dimensions);
        }
    }
}
