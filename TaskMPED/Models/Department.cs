using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskMPED.Models
{
    public class Department
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }


        // Not Use (old Version):-
        //public IEnumerable<Employee> Employees { get; set; }
        //public List<Employee> Employees { get; set; }
    }
}
