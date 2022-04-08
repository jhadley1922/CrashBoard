using CrashBoard.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models
{
    //Crash repository interface
    public interface ICrashRepository
    {
        IQueryable<Crash> Crashes { get; }
        IQueryable<City> Cities { get; }
        IQueryable<County> Counties { get; }
        IQueryable<Severity> Severities { get; }
        public List<DataPoint> CountyCounts { get; }

        public void GetCountyCounts();
        public void SaveCrash(Crash c);
        public void CreateCrash(Crash c);
        public void DeleteCrash(Crash c);
    }
}
