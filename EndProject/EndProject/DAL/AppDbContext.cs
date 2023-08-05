using EndProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.DAL
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<AboutUs> AboutUs { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Workers> Workers { get; set; }
        public DbSet<SliderImage> SliderImages { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<WorkerBio> WorkerBio { get; set; }
        public DbSet<Compaign> Compaign { get; set; }
        public DbSet<CurrencyConverter> CurrencyConverters { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Credit> Credits { get; set; }
        
    }
}
