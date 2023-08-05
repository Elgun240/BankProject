using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EndProject.Models
{
    public class News
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
       
        public string Image { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public string Subtitle { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDeactive { get; set; }
    }
}
