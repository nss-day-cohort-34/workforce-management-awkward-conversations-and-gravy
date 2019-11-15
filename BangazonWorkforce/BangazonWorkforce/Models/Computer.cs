using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Computer
    {
        public int Id { get; set; }
        public string Manufacturer { get; set; }
        public string Make { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime DecomissionDate { get; set; }
    }
}
