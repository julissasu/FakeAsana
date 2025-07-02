using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asana.Library.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CompletePercent { get; set; }
        public double CompleteProgress => CompletePercent / 100.0;
        
        public List<ToDo> ToDos { get; set; } = new List<ToDo>();
    }
}

