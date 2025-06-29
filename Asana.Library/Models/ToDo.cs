using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asana.Library.Models
{
    public class ToDo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Priority { get; set; }
        public bool IsComplete { get; set; }
        public int? ProjectId { get; set; }
    }
}