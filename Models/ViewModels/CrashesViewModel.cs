using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models.ViewModels
{
    // Bundles crashes info
    public class CrashesViewModel
    {
        public IQueryable<Crash> Crashes { get; set; }
    }
}
