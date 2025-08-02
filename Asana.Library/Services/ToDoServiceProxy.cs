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
        private List<ToDo> _toDoList;
        private ProjectServiceProxy _projectSvc;

        // ToDo methods
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

        private ToDoServiceProxy()
        {
            _projectSvc = ProjectServiceProxy.Current;
            _toDoList = new List<ToDo>();
        }

        private static ToDoServiceProxy? instance;
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

        // Get ToDo by ID
        private int nextToDoId
        {
            get
            {
                return _toDoList.Any() ? _toDoList.Max(t => t.Id) + 1 : 1;
            }
        }

        // Add or update a ToDo
        public ToDo? AddOrUpdate(ToDo? toDo)
        {
            if (toDo == null) return toDo;

            var isNew = toDo.Id == 0;

            if (isNew)
            {
                toDo.Id = nextToDoId;
                _toDoList.Add(toDo);
            }
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

        public ToDo? GetToDoById(int id)
        {
            return _toDoList.FirstOrDefault(t => t.Id == id);
        }

        public void DeleteToDo(int id)
        {
            var toDoToDelete = _toDoList.FirstOrDefault(t => t.Id == id);
            if (toDoToDelete != null)
            {
                _toDoList.Remove(toDoToDelete);
            }
        }

        public void DeleteToDo(ToDo? toDo)
        {
            if (toDo != null)
            {
                DeleteToDo(toDo.Id);
            }
        }

        // Project-related methods
        public List<Project> Projects
        {
            get { return _projectSvc.Projects; }
        }

        public Project? GetProjectById(int? id)
        {
            return _projectSvc.GetProjectById(id);
        }

        public void DeleteProject(Project? project)
        {
            if (project == null) return;
            // Unassign all todos from this project
            var assignedToDos = _toDoList.Where(t => t.ProjectId == project.Id).ToList();
            foreach (var todo in assignedToDos)
            {
                todo.ProjectId = null;
            }
            _projectSvc.DeleteProject(project);
        }

        public Project? AddOrUpdateProject(Project? project)
        {
            return _projectSvc.AddOrUpdateProject(project);
        }

        public List<ToDo> GetToDosByProject(int projectId)
        {
            return _toDoList.Where(t => t.ProjectId == projectId).ToList();
        }

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