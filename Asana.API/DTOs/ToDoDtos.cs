using System.ComponentModel.DataAnnotations;

namespace Asana.API.DTOs
{
    /// <summary>
    /// ToDo data transfer object
    /// </summary>
    public class ToDoDto
    {
        /// <summary>
        /// Unique identifier for the todo
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Name of the todo item
        /// </summary>
        /// <example>Update documentation</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the todo item
        /// </summary>
        /// <example>Update API documentation with latest changes</example>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Priority level (1=Low, 2=Medium, 3=High)
        /// </summary>
        /// <example>2</example>
        public int Priority { get; set; } = 1;

        /// <summary>
        /// Whether the todo is completed
        /// </summary>
        /// <example>false</example>
        public bool IsComplete { get; set; }

        /// <summary>
        /// ID of the associated project (optional)
        /// </summary>
        /// <example>1</example>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Due date for the todo (optional)
        /// </summary>
        /// <example>2025-08-15T09:00:00</example>
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a new todo
    /// </summary>
    public class CreateToDoDto
    {
        /// <summary>
        /// Name of the todo item
        /// </summary>
        /// <example>Update documentation</example>
        [Required(ErrorMessage = "Todo name is required")]
        [StringLength(100, ErrorMessage = "Todo name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the todo item
        /// </summary>
        /// <example>Update API documentation with latest changes</example>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Priority level (1=Low, 2=Medium, 3=High)
        /// </summary>
        /// <example>2</example>
        [Range(1, 3, ErrorMessage = "Priority must be between 1 and 3")]
        public int Priority { get; set; } = 1;

        /// <summary>
        /// ID of the associated project (optional)
        /// </summary>
        /// <example>1</example>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Due date for the todo (optional)
        /// </summary>
        /// <example>2025-08-15T09:00:00</example>
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// Data transfer object for updating a todo
    /// </summary>
    public class UpdateToDoDto
    {
        /// <summary>
        /// Unique identifier for the todo
        /// </summary>
        /// <example>1</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Name of the todo item
        /// </summary>
        /// <example>Update documentation - Completed</example>
        [Required(ErrorMessage = "Todo name is required")]
        [StringLength(100, ErrorMessage = "Todo name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the todo item
        /// </summary>
        /// <example>Updated API documentation with all latest changes and examples</example>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Priority level (1=Low, 2=Medium, 3=High)
        /// </summary>
        /// <example>3</example>
        [Range(1, 3, ErrorMessage = "Priority must be between 1 and 3")]
        public int Priority { get; set; } = 1;

        /// <summary>
        /// Whether the todo is completed
        /// </summary>
        /// <example>true</example>
        public bool IsComplete { get; set; }

        /// <summary>
        /// ID of the associated project (optional)
        /// </summary>
        /// <example>1</example>
        public int? ProjectId { get; set; }

        /// <summary>
        /// Due date for the todo (optional)
        /// </summary>
        /// <example>2025-08-15T09:00:00</example>
        public DateTime? DueDate { get; set; }
    }
}
