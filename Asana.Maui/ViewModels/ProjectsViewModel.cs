using Asana.Library.Models;
using Asana.Library.Services;

namespace Asana.Maui.ViewModels
{
    public class ProjectViewModel : IEquatable<ProjectViewModel>
    {
        public Project? Model { get; set; }

        public string DisplayName
        {
            get
            {
                if (Model == null) return "No Project";
                return $"{Model.Id}. {Model.Name}";
            }
        }

        public string TaskSummary
        {
            get
            {
                if (Model == null) return string.Empty;

                var projectToDos = ToDoServiceProxy.Current.GetToDosByProject(Model.Id);
                var completedCount = projectToDos.Count(t => t.IsComplete);
                var totalCount = projectToDos.Count;

                if (totalCount == 0) return "No tasks assigned";

                var percentage = totalCount > 0 ? (completedCount * 100.0 / totalCount) : 0;
                return $"{completedCount}/{totalCount} tasks completed ({percentage:F0}%)";
            }
        }

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

        public override bool Equals(object? obj)
        {
            return Equals(obj as ProjectViewModel);
        }

        public override int GetHashCode()
        {
            return Model?.Id.GetHashCode() ?? 0;
        }
    }
}