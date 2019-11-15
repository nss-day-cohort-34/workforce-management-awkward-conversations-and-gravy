using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeCreateViewModel
    {
        public List<SelectListItem> Departments { get; set; }
        public Employee Employee { get; set; }
    }
}
