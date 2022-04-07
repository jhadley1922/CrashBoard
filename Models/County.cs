using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models
{
    //Model for County table
    public class County
    {
        [Key]
        [Required]
        public int COUNTY_ID { get; set; }
        public string COUNTY_NAME { get; set; }
    }
}
