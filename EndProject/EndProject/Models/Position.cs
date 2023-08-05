using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Models
{
    public class Position
    {
        public int Id { get; set; }
        [Required]
        public string  Name { get; set; }
        [Required]
        public string Obligations { get; set; }
        public bool IsDeactive { get; set; }
        public List<Workers> Workers { get; set; }
    }
}
