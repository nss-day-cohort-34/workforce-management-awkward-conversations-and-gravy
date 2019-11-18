using BangazonWorkforce.Models;
using System.Collections.Generic;

namespace BangazonWorkforce.Models.ViewModels
{
    public class DepartmentEditViewModel
    {
        public Department Department { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
