using System.ComponentModel.DataAnnotations;
using System;

namespace BangazonWorkforce.Models
{
    public class Computer
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate {get; set;}
        [Display(Name = "Decommission Date")]
        public DateTime? DecommissionDate { get; set; }
        [Required]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }
        [Required]
        [Display(Name = "Make")]
        public string Make { get; set; }
    }
}