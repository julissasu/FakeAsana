using Asana.Library.Models;
using Asana.Library.Services;

namespace Asana.Maui.ViewModels
{
    public class ProjectViewModel : IEquatable<ProjectViewModel>
    {
        public Project? Model { get; set; } // The underlying Project model

        // Display name for the project, used in UI
        public string DisplayName
        {
            get
            {
                if (Model == null) return "No Project";
                return $"{Model.Id}. {Model.Name}";
            }
        }

        // Summary of ToDos: how many tasks are completed out of total with percentage
        public string TaskSummary
        {
            get
            {
                if (Model == null) return string.Empty;

                // Get all ToDos for this project
                var projectToDos = ToDoServiceProxy.Current.GetToDosByProject(Model.Id);
                var completedCount = projectToDos.Count(t => t.IsComplete); // Count completed tasks
                var totalCount = projectToDos.Count;    // Count total tasks

                if (totalCount == 0) return "No tasks assigned";    // No tasks in this project

                var percentage = totalCount > 0 ? (completedCount * 100.0 / totalCount) : 0; // Calculate completion percentage
                return $"{completedCount}/{totalCount} tasks completed ({percentage:F0}%)";
            }
        }

        // Returns the display name for the project
        public override string ToString()
        {
            return DisplayName;
        }

        // Implement equality comparison for Picker binding
        public bool Equals(ProjectViewModel? other)
        {
            if (other == null) return false;

            // Both have null models (No Project option)
            if (Model == null && other.Model == null) return true;

            // One has null model, other doesn't
            if (Model == null || other.Model == null) return false;

            // Compare by ID
            return Model.Id == other.Model.Id;
        }

        // Override Equals to compare ProjectViewModel instances
        public override bool Equals(object? obj)
        {
            return Equals(obj as ProjectViewModel);
        }

        // Hash code based on the Project ID
        public override int GetHashCode()
        {
            return Model?.Id.GetHashCode() ?? 0;
        }
    }
}