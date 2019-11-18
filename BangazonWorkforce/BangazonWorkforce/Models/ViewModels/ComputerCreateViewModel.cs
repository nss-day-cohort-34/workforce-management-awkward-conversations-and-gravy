using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class ComputerCreateViewModel
    {
        public List<SelectListItem> Employees { get; set; }
        public Computer Computer { get; set; }
    }
}
