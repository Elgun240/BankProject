using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Models
{
    public class WorkerBio
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        public Workers Worker { get; set; }
        [ForeignKey("Workers")]
        public int WorkerId { get; set; }
    }
}
