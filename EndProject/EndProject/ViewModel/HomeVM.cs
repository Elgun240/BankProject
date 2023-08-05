using EndProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.ViewModel
{
    public class HomeVM
    {
        public AboutUs AboutUS { get; set; }
        public Slider Sliders { get; set; }
        public List<Workers> Workers { get; set; }
        public List<Position> Positions { get; set; }
        public List<News> News { get; set; }
        public List<Compaign> Compaigns { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Credit> Credits { get; set; }
        public List<AppUser> Users { get; set; }

    }
}
