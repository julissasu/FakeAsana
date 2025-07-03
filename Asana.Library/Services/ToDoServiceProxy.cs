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
        private List<Project> _projectList;

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

        private ToDoServiceProxy()
        {
            // Start with empty lists
            _toDoList = new List<ToDo>();
            _projectList = new List<Project>();
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

        // ToDo methods
        private int nextToDoId
        {
            get
            {
                return _toDoList.Any() ? _toDoList.Max(t => t.Id) + 1 : 1;
            }
        }

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

        public ToDo? GetById(int id)
        {
            return _toDoList.FirstOrDefault(t => t.Id == id);
        }

        public ToDo? GetToDoById(int id)
        {
            return GetById(id);
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

        // Project methods
        private int nextProjectId
        {
            get
            {
                return _projectList.Any() ? _projectList.Max(p => p.Id) + 1 : 1;
            }
        }

        public Project? AddOrUpdateProject(Project? project)
        {
            if (project == null) return project;

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
            if (!id.HasValue) return null;
            return _projectList.FirstOrDefault(p => p.Id == id);
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

            _projectList.Remove(project);
        }

        public List<ToDo> GetToDosByProject(int projectId)
        {
            return _toDoList.Where(t => t.ProjectId == projectId).ToList();
        }

        // Display methods (following professor's pattern)
        public void DisplayToDos(bool isShowCompleted = false)
        {
            if (isShowCompleted)
            {
                _toDoList.ForEach(Console.WriteLine);
            }
            else
            {
                _toDoList.Where(t => !t.IsComplete).ToList().ForEach(Console.WriteLine);
            }
        }
    }
}