<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
=======
﻿using System.ComponentModel.DataAnnotations;
using System;
>>>>>>> master

namespace BangazonWorkforce.Models
{
    public class Computer
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Purchase Date")]
<<<<<<< HEAD
        public DateTime PurchaseDate { get; set; }
        [Display(Name = "Decommission Date")]
        public DateTime DecomissionDate { get; set; }
        [Required]
        [Display(Name = "Manufacturer")]
=======
        public DateTime PurchaseDate {get; set;}
        [Display(Name = "Decommission Date")]
        public DateTime? DecommissionDate { get; set; }
        [Required]
        [Display(Name = "Computer")]
>>>>>>> master
        public string Manufacturer { get; set; }
        [Required]
        [Display(Name = "Make")]
        public string Make { get; set; }
    }
}