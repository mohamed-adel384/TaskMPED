using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskMPED.Models
{
    public class Employee
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        [Required]
        public int Phone { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfBirth { get; set; }

        // Save in DataBase
        public byte[] Image { get; set; }
        public string Address { get; set; }

        [ForeignKey("City")]
        public int City_Id { get; set; }

        [ForeignKey("Department")]
        public int Dep_Id { get; set; }


        public City City { get; set; }
        public Department Department { get; set; }
    }
}
