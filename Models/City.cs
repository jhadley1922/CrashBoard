using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models
{
    //Model for City table
    public class City
    {
        [Key]
        [Required]
        public int CITY_ID { get; set; }
        public string CITY_NAME { get; set; }
    }
}
