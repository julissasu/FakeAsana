using System.ComponentModel.DataAnnotations;

namespace Asana.API.DTOs
{
    // DTOs for ToDo items
    public class ToDoDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; } = 1;
        public bool IsComplete { get; set; }
        public int? ProjectId { get; set; }
        public DateTime? DueDate { get; set; }
    }
    public class CreateToDoDto
    {
        [Required(ErrorMessage = "Todo name is required")]
        [StringLength(100, ErrorMessage = "Todo name cannot exceed 100 characters")]

        public string Name { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]

        public string Description { get; set; } = string.Empty;
        [Range(1, 3, ErrorMessage = "Priority must be between 1 and 3")]

        public int Priority { get; set; } = 1;

        public int? ProjectId { get; set; }

        public DateTime? DueDate { get; set; }
    }
    public class UpdateToDoDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Todo name is required")]
        [StringLength(100, ErrorMessage = "Todo name cannot exceed 100 characters")]

        public string Name { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]

        public string Description { get; set; } = string.Empty;
        [Range(1, 3, ErrorMessage = "Priority must be between 1 and 3")]

        public int Priority { get; set; } = 1;

        public bool IsComplete { get; set; }

        public int? ProjectId { get; set; }

        public DateTime? DueDate { get; set; }
    }
}
