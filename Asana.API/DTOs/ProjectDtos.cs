using System.ComponentModel.DataAnnotations;

namespace Asana.API.DTOs
{
    /// <summary>
    /// Project data transfer object
    /// </summary>
    public class ProjectDto
    {
        /// <summary>
        /// Unique identifier for the project
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Name of the project
        /// </summary>
        /// <example>Website Redesign</example>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the project
        /// </summary>
        /// <example>Complete redesign of the company website</example>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Completion percentage (0-100)
        /// </summary>
        /// <example>75</example>
        public int CompletePercent { get; set; }
    }

    /// <summary>
    /// Data transfer object for creating a new project
    /// </summary>
    public class CreateProjectDto
    {
        /// <summary>
        /// Name of the project
        /// </summary>
        /// <example>Website Redesign</example>
        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the project
        /// </summary>
        /// <example>Complete redesign of the company website</example>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data transfer object for updating a project
    /// </summary>
    public class UpdateProjectDto
    {
        /// <summary>
        /// Unique identifier for the project
        /// </summary>
        /// <example>1</example>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Name of the project
        /// </summary>
        /// <example>Website Redesign Updated</example>
        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Description of the project
        /// </summary>
        /// <example>Complete redesign of the company website with new features</example>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Completion percentage (0-100)
        /// </summary>
        /// <example>85</example>
        [Range(0, 100, ErrorMessage = "Complete percent must be between 0 and 100")]
        public int CompletePercent { get; set; }
    }
}
