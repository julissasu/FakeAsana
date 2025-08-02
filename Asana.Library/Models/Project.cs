using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Asana.Library.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CompletePercent { get; set; }

        [JsonIgnore]
        public double CompleteProgress => CompletePercent / 100.0;

        [JsonIgnore]
        public List<ToDo> ToDos { get; set; } = new List<ToDo>();
    }
}