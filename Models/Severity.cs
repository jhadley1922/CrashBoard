using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models
{
    //Model for Severity table
    public class Severity
    {
        [Key]
        [Required]
        public int SEVERITY_ID { get; set; }
        public string SEVERITY_DESCRIPTION { get; set; }
    }
}
