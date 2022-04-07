using CrashBoard.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models
{
    //Makes repository of the data
    public class EFCrashRepository : ICrashRepository
    {
        private CrashDbContext context { get; set; }

        public EFCrashRepository(CrashDbContext cc)
        {
            context = cc;
        }
        public IQueryable<Crash> Crashes => context.Crashes;

        public IQueryable<City> Cities => context.Cities;

        public IQueryable<County> Counties => context.Counties;

        public IQueryable<Severity> Severities => context.Severities;

        public void CreateCrash(Crash c)
        {
            context.Add(c);
            context.SaveChanges();
        }

        public void DeleteCrash(Crash c)
        {
            context.Remove(c);
            context.SaveChanges();
        }

        public void SaveCrash(Crash c)
        {
            context.Update(c);
            context.SaveChanges();
        }
    }
}
