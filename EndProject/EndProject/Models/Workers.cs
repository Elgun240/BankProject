using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Models
{
    public class Workers
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public int Age { get; set; } 
        public string Images { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public WorkerBio WorkerBio { get; set; }
        public Position Position { get; set; }
        public int PositionId { get; set; }
        public bool IsDeactive { get; set; }

    }
}
