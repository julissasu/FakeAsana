using Asana.Library.Models;
using Asana.Library.Data;
using System.Collections.Generic;
using System.Linq;

namespace Asana.Library.Services
{
    public class ProjectServiceProxy
    {
        private readonly FakeDatabase _fakeDatabase;

        private ProjectServiceProxy()
        {
            _fakeDatabase = new FakeDatabase();
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
                return _fakeDatabase.GetProjects();
            }
        }

        public Project? AddOrUpdateProject(Project? project)
        {
            if (project == null) return null;

            return _fakeDatabase.AddOrUpdateProject(project);
        }

        public Project? GetProjectById(int? id)
        {
            if (!id.HasValue || id.Value == 0) return null;
            return _fakeDatabase.GetProjectById(id.Value);
        }

        public void DeleteProject(Project? project)
        {
            if (project == null) return;
            _fakeDatabase.DeleteProject(project.Id);
        }

        /// <summary>
        /// Get projects by completion status
        /// </summary>
        public List<Project> GetProjectsByCompletion(bool isComplete)
        {
            return _fakeDatabase.GetProjectsByCompletion(isComplete);
        }

        /// <summary>
        /// Get project statistics from the fake database
        /// </summary>
        public ProjectStatistics GetProjectStatistics()
        {
            return _fakeDatabase.GetStatistics();
        }

        /// <summary>
        /// Calculate overall progress across all projects
        /// </summary>
        public double CalculateOverallProgress()
        {
            return _fakeDatabase.CalculateOverallProgress();
        }
    }
}