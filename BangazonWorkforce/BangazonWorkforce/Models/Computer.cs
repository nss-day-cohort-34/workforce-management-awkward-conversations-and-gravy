using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Computer
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
        [Display(Name = "Decommission Date")]
        public DateTime DecomissionDate { get; set; }
        [Required]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }
        [Required]
        [Display(Name = "Make")]
        public string Make { get; set; }
    }
}
