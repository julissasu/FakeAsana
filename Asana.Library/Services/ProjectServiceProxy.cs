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
            }

            return project;
        }

        public Project? GetProjectById(int? id)
        {
            if (!id.HasValue || id.Value == 0) return null;
            return _projectList.FirstOrDefault(p => p.Id == id);
        }

        public void DeleteProject(Project? project)
        {
            if (project == null) return;
            _projectList.Remove(project);
        }
    }
}