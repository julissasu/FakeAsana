using System.ComponentModel.DataAnnotations;

namespace Asana.API.DTOs
{
    // DTOs for Project
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CompletePercent { get; set; }
    }

    public class CreateProjectDto
    {
        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateProjectDto
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Project name is required")]
        [StringLength(100, ErrorMessage = "Project name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "Complete percent must be between 0 and 100")]
        public int CompletePercent { get; set; }
    }
}
