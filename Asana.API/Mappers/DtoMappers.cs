using Asana.API.DTOs;
using Asana.Library.Models;

namespace Asana.API.Mappers
{
    public static class ProjectMapper
    {
        public static ProjectDto ToDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name ?? string.Empty,
                Description = project.Description ?? string.Empty,
                CompletePercent = project.CompletePercent
            };
        }

        public static Project ToModel(CreateProjectDto dto)
        {
            return new Project
            {
                Id = 0, // Will be assigned by service
                Name = dto.Name,
                Description = dto.Description,
                CompletePercent = 0
            };
        }

        public static Project ToModel(UpdateProjectDto dto)
        {
            return new Project
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                CompletePercent = dto.CompletePercent
            };
        }
    }

    public static class ToDoMapper
    {
        public static ToDoDto ToDto(ToDo todo)
        {
            return new ToDoDto
            {
                Id = todo.Id,
                Name = todo.Name ?? string.Empty,
                Description = todo.Description ?? string.Empty,
                Priority = todo.Priority ?? 1,
                IsComplete = todo.IsComplete,
                ProjectId = todo.ProjectId,
                DueDate = todo.DueDate
            };
        }

        public static ToDo ToModel(CreateToDoDto dto)
        {
            return new ToDo
            {
                Id = 0, // Will be assigned by service
                Name = dto.Name,
                Description = dto.Description,
                Priority = dto.Priority,
                IsComplete = false,
                ProjectId = dto.ProjectId,
                DueDate = dto.DueDate ?? DateTime.Now
            };
        }

        public static ToDo ToModel(UpdateToDoDto dto)
        {
            return new ToDo
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Priority = dto.Priority,
                IsComplete = dto.IsComplete,
                ProjectId = dto.ProjectId,
                DueDate = dto.DueDate
            };
        }
    }
}
