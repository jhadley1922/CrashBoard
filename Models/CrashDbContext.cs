using CrashBoard.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashBoard.Models
{
    //Sets up DbSets from database
    public class CrashDbContext : DbContext
    {
        public CrashDbContext()
        {

        }
        public CrashDbContext(DbContextOptions<CrashDbContext> options) : base(options) { }

        public DbSet<Crash> Crashes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<Severity> Severities { get; set; }
    }
}
