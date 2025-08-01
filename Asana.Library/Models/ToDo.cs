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
        public int? Priority { get; set; } = 1;
        public bool IsComplete { get; set; }
        public int? ProjectId { get; set; }
        public DateTime? DueDate { get; set; } = DateTime.Now;

        public override string ToString()
        {
            var status = IsComplete ? "✓" : "○";
            var projectInfo = ProjectId.HasValue ? $" [Project: {ProjectId}]" : "";
            return $"{status} [{Id}] {Name} - {Description} (Priority {Priority}) - Due: {DueDate:MMM dd}{projectInfo}";
        }
    }
}