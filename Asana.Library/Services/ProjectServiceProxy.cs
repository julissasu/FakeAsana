using Asana.Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace Asana.Library.Services
{
    public class ProjectServiceProxy
    {
        private List<Project> _projectList;

        private ProjectServiceProxy()
        {
            _projectList = new List<Project>();
        }

        private static ProjectServiceProxy? _instance;
        public static ProjectServiceProxy Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProjectServiceProxy();
                }
                return _instance;
            }
        }

        public List<Project> Projects
        {
            get
            {
                return _projectList.ToList();
            }
        }

        private int nextProjectId
        {
            get
            {
                return _projectList.Any() ? _projectList.Max(p => p.Id) + 1 : 1;
            }
        }

        public Project? AddOrUpdateProject(Project? project)
        {
            if (project == null) return null;

            var isNew = project.Id == 0;

            if (isNew)
            {
                project.Id = nextProjectId;
                _projectList.Add(project);
            }
            else
            {
                var existingProject = _projectList.FirstOrDefault(p => p.Id == project.Id);
                if (existingProject != null)
                {
                    var index = _projectList.IndexOf(existingProject);
                    _projectList.RemoveAt(index);
                    _projectList.Insert(index, project);
                }
                else
                {
                    // Project has an ID but doesn't exist in list (e.g., loaded from persistence)
                    _projectList.Add(project);
                }
            }

            return project;
        }

        public Project? GetProjectById(int? id)
        {
            if (!id.HasValue || id.Value == 0) return null;
            return _projectList.FirstOrDefault(p => p.Id == id.Value);
        }

        public void DeleteProject(Project? project)
        {
            if (project == null) return;
            var projectToDelete = _projectList.FirstOrDefault(p => p.Id == project.Id);
            if (projectToDelete != null)
            {
                _projectList.Remove(projectToDelete);
            }
        }

        public void DeleteProject(int projectId)
        {
            var projectToDelete = _projectList.FirstOrDefault(p => p.Id == projectId);
            if (projectToDelete != null)
            {
                _projectList.Remove(projectToDelete);
            }
        }

        /// <summary>
        /// Get projects by completion status
        /// </summary>
        public List<Project> GetProjectsByCompletion(bool isComplete)
        {
            return _projectList.Where(p => (p.CompletePercent == 100) == isComplete).ToList();
        }

        /// <summary>
        /// Get project statistics
        /// </summary>
        public ProjectStatistics GetProjectStatistics()
        {
            var totalProjects = _projectList.Count;
            var completedProjects = _projectList.Count(p => p.CompletePercent == 100);
            var inProgressProjects = _projectList.Count(p => p.CompletePercent > 0 && p.CompletePercent < 100);
            var notStartedProjects = _projectList.Count(p => p.CompletePercent == 0);
            var averageCompletion = _projectList.Any() ? _projectList.Average(p => p.CompletePercent) : 0;

            return new ProjectStatistics
            {
                TotalProjects = totalProjects,
                CompletedProjects = completedProjects,
                InProgressProjects = inProgressProjects,
                NotStartedProjects = notStartedProjects,
                AverageCompletion = averageCompletion
            };
        }

        /// <summary>
        /// Calculate overall progress across all projects
        /// </summary>
        public double CalculateOverallProgress()
        {
            if (!_projectList.Any()) return 0;
            return _projectList.Average(p => p.CompletePercent);
        }
    }

    /// <summary>
    /// Project statistics data structure
    /// </summary>
    public class ProjectStatistics
    {
        public int TotalProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int InProgressProjects { get; set; }
        public int NotStartedProjects { get; set; }
        public double AverageCompletion { get; set; }
    }
}