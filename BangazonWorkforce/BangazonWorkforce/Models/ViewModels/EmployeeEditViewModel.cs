using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public Employee Employee { get; set; }
        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Computers { get; set; } = new List<SelectListItem>();
        public int ComputerId { get; set; }
    }
}
