using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TaskMPED.Models;

namespace TaskMPED.ViewModels
{
    public class EmployeeFormViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [EmailAddress()]
        public string Email { get; set; }
        public int Age { get; set; }
        [Required]
        public int Phone { get; set; }
        [Display(Name = "Date of birth")]
        [Required ]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfBirth { get; set; }

        // Save in DataBase
        [Display(Name = "Select profile picture...")]
        public byte[] Image { get; set; }
        [Required, StringLength(2500)]
        public string Address { get; set; }

        [ForeignKey("City")]
        [Display(Name = "City")]
        public int City_Id { get; set; }

        [ForeignKey("Department")]
        [Display(Name = "Department")]
        public int Dep_Id { get; set; }


        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}
