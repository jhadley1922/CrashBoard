using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models.ViewModels
{
    public class AdminModel
    {
        public int severity { get; set; }
        public int pageNum { get; set; } 
        public string searchString { get; set; }
    }
}
