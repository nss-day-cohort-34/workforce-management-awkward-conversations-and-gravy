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
        [DataType(DataType.Date)]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Decommission Date")]
        public DateTime? DecomissionDate { get; set; }

        [Required]
        [Display(Name = "Manufacturer")]
        public string Manufacturer { get; set; }

        [Required]
        [Display(Name = "Make")]
        public string Make { get; set; }

        [Display(Name = "Computer")]
        public string ComputerInfo
        {
            get
            {
                return $"{Manufacturer} {Make}";
             }
        }
        public int AssignmentCount { get; set; }
    }
}
