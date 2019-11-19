using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{
    public class EmployeeEditViewModel
    {
        public Employee Employee { get; set; }
        public List<SelectListItem> Departments { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Computers { get; set; } = new List<SelectListItem>();
        [Display(Name = "Computer")]
        public int ComputerId { get; set; }

        public List<SelectListItem> TrainingProgramOptions
        {
            get
            {
                if (AllTrainingPrograms == null) return null;

                return AllTrainingPrograms
                    .Select(tp => new SelectListItem(tp.Name, tp.Id.ToString()))
                    .ToList();
            }
        }
    }
}
