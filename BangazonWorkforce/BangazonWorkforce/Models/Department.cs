using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BangazonWorkforce.Models
{
    public class Department
    {
        [Display(Name = "Department")]
        public int Id { get; set; }

        [Display(Name = "Department")]
        public string Name { get; set; }
        public int
         Budget { get; set; }
        [Display(Name = "Employee Count")]
        public int EmployeeCount { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}