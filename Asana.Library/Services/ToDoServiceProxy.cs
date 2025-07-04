using Asana.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asana.Library.Services
{
    public class ToDoServiceProxy
    {
        private List<ToDo> _toDoList; // List of ToDos
        private List<Project> _projectList; // List of Projects

        // Copy of ToDos and Projects
        public List<ToDo> ToDos
        {
            get
            {
                return _toDoList.ToList();
            }
            private set
            {
                if (value != _toDoList)
                {
                    _toDoList = value;
                }
            }
        }

        public List<Project> Projects
        {
            get
            {
                return _projectList.ToList();
            }
            private set
            {
                if (value != _projectList)
                {
                    _projectList = value;
                }
            }
        }

        // Singleton instance
        private ToDoServiceProxy()
        {
            // Start with empty lists
            _toDoList = new List<ToDo>();
            _projectList = new List<Project>();
        }

        private static ToDoServiceProxy? instance;

        // Singleton access point
        public static ToDoServiceProxy Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new ToDoServiceProxy();
                }
                return instance;
            }
        }

        // ToDo methods
        // Returns the next available ToDo ID
        private int nextToDoId
        {
            get
            {
                return _toDoList.Any() ? _toDoList.Max(t => t.Id) + 1 : 1;
            }
        }

        // Adds or updates a ToDo item
        public ToDo? AddOrUpdate(ToDo? toDo)
        {
            if (toDo == null) return toDo;  // Return null if toDo is null

            var isNew = toDo.Id == 0;       // Check if it's a new ToDo

            // If it's a new ToDo, assign the next available ID
            if (isNew)
            {
                toDo.Id = nextToDoId;
                _toDoList.Add(toDo);
            }
            // If it's an existing ToDo, find it in the list and update it
            else
            {
                var existingToDo = _toDoList.FirstOrDefault(t => t.Id == toDo.Id);
                if (existingToDo != null)
                {
                    var index = _toDoList.IndexOf(existingToDo);
                    _toDoList.RemoveAt(index);
                    _toDoList.Insert(index, toDo);
                }
            }

            return toDo;
        }

        public ToDo? AddOrUpdateToDo(ToDo? toDo)
        {
            return AddOrUpdate(toDo);
        }

        // Retrieves a ToDo by its ID
        public ToDo? GetById(int id)
        {
            return _toDoList.FirstOrDefault(t => t.Id == id);
        }

        public ToDo? GetToDoById(int id)
        {
            return GetById(id);
        }

        // Retrieves all ToDos 
        public void DeleteToDo(int id)
        {
            var toDoToDelete = _toDoList.FirstOrDefault(t => t.Id == id);
            if (toDoToDelete != null)
            {
                _toDoList.Remove(toDoToDelete);
            }
        }

        // Deletes a ToDo by its ID
        public void DeleteToDo(ToDo? toDo)
        {
            if (toDo != null)
            {
                DeleteToDo(toDo.Id);
            }
        }

        // Project methods
        // Returns the next available Project ID
        private int nextProjectId
        {
            get
            {
                return _projectList.Any() ? _projectList.Max(p => p.Id) + 1 : 1;
            }
        }

        // Adds or updates a Project
        public Project? AddOrUpdateProject(Project? project)
        {
            if (project == null) return project; // Return null if project is null

            var isNew = project.Id == 0;    // Check if it's a new Project

            // If it's a new Project, assign the next available ID
            if (isNew)
            {
                project.Id = nextProjectId;
                _projectList.Add(project);
            }
            // If it's an existing Project, find it in the list and update it
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

        // Retrieves a Project by its ID
        public Project? GetProjectById(int? id)
        {
            if (!id.HasValue) return null;
            return _projectList.FirstOrDefault(p => p.Id == id);
        }

        // Deletes a Project by its ID and unassigns all ToDos from it
        public void DeleteProject(Project? project)
        {
            if (project == null) return;

            // Unassign all todos from this project
            var assignedToDos = _toDoList.Where(t => t.ProjectId == project.Id).ToList();
            foreach (var todo in assignedToDos)
            {
                todo.ProjectId = null;
            }

            _projectList.Remove(project);   // Remove the project from the list
        }

        // Retrieves all ToDos associated with a specific Project
        public List<ToDo> GetToDosByProject(int projectId)
        {
            return _toDoList.Where(t => t.ProjectId == projectId).ToList();
        }

        // Project assignment methods
        public bool AssignToDoToProject(int toDoId, int? projectId)
        {
            var toDo = GetToDoById(toDoId);
            if (toDo == null) return false;

            if (projectId == null || projectId == 0)
            {
                toDo.ProjectId = null;
                return true;
            }

            var project = GetProjectById(projectId);
            if (project == null) return false;

            toDo.ProjectId = projectId;
            return true;
        }
    }
}