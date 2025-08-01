namespace Asana.HttpClient.Models
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CompletePercent { get; set; }
    }

    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

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
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; } = 1;
        public int? ProjectId { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
