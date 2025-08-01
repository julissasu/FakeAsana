using Asana.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Asana.Library.Data
{
    /// <summary>
    /// A fake in-memory database used for project management logic
    /// This is separate from the main ToDo functionality which uses Filebase
    /// </summary>
    public class FakeDatabase
    {
        private readonly List<Project> _projects;
        private readonly Dictionary<string, object> _metadata;

        public FakeDatabase()
        {
            _projects = new List<Project>();
            _metadata = new Dictionary<string, object>();
            
            // Initialize with some sample projects for demonstration
            InitializeSampleData();
        }

        /// <summary>
        /// Get all projects from the fake database
        /// </summary>
        public List<Project> GetProjects()
        {
            return _projects.ToList();
        }

        /// <summary>
        /// Add or update a project in the fake database
        /// </summary>
        public Project AddOrUpdateProject(Project project)
        {
            if (project == null) throw new ArgumentNullException(nameof(project));

            var existingProject = _projects.FirstOrDefault(p => p.Id == project.Id);
            if (existingProject != null)
            {
                // Update existing project
                var index = _projects.IndexOf(existingProject);
                _projects[index] = project;
            }
            else
            {
                // Add new project with auto-generated ID
                if (project.Id == 0)
                {
                    project.Id = GetNextProjectId();
                }
                _projects.Add(project);
            }

            return project;
        }

        /// <summary>
        /// Get a project by ID
        /// </summary>
        public Project? GetProjectById(int id)
        {
            return _projects.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Delete a project from the fake database
        /// </summary>
        public bool DeleteProject(int id)
        {
            var project = _projects.FirstOrDefault(p => p.Id == id);
            if (project != null)
            {
                _projects.Remove(project);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get projects by completion status
        /// </summary>
        public List<Project> GetProjectsByCompletion(bool isComplete)
        {
            return _projects.Where(p => (p.CompletePercent == 100) == isComplete).ToList();
        }

        /// <summary>
        /// Calculate total completion percentage across all projects
        /// </summary>
        public double CalculateOverallProgress()
        {
            if (!_projects.Any()) return 0;
            return _projects.Average(p => p.CompletePercent);
        }

        /// <summary>
        /// Get project statistics
        /// </summary>
        public ProjectStatistics GetStatistics()
        {
            return new ProjectStatistics
            {
                TotalProjects = _projects.Count,
                CompletedProjects = _projects.Count(p => p.CompletePercent == 100),
                InProgressProjects = _projects.Count(p => p.CompletePercent > 0 && p.CompletePercent < 100),
                NotStartedProjects = _projects.Count(p => p.CompletePercent == 0),
                AverageCompletion = CalculateOverallProgress()
            };
        }

        /// <summary>
        /// Store metadata in the fake database
        /// </summary>
        public void SetMetadata(string key, object value)
        {
            _metadata[key] = value;
        }

        /// <summary>
        /// Retrieve metadata from the fake database
        /// </summary>
        public T? GetMetadata<T>(string key)
        {
            if (_metadata.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            return default(T);
        }

        /// <summary>
        /// Clear all data from the fake database
        /// </summary>
        public void Clear()
        {
            _projects.Clear();
            _metadata.Clear();
        }

        private int GetNextProjectId()
        {
            return _projects.Any() ? _projects.Max(p => p.Id) + 1 : 1;
        }

        private void InitializeSampleData()
        {
            // Add some sample projects for demonstration
            _projects.AddRange(new[]
            {
                new Project
                {
                    Id = 1,
                    Name = "Website Redesign",
                    Description = "Complete redesign of company website with modern UI/UX",
                    CompletePercent = 75
                },
                new Project
                {
                    Id = 2,
                    Name = "Mobile App Development",
                    Description = "Develop cross-platform mobile application",
                    CompletePercent = 30
                },
                new Project
                {
                    Id = 3,
                    Name = "Database Migration",
                    Description = "Migrate legacy database to new cloud infrastructure",
                    CompletePercent = 100
                }
            });

            // Set some metadata
            SetMetadata("created_date", DateTime.Now);
            SetMetadata("version", "1.0.0");
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
