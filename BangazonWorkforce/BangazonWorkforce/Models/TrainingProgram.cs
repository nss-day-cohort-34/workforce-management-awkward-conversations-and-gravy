using System;
using System.ComponentModel.DataAnnotations;

namespace BangazonWorkforce.Models
{ 
    public class TrainingProgram
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Training Program")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        public int MaxAttendees { get; set; }
    }
}