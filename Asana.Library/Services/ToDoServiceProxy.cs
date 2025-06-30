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
        // static data storage
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

        private int nextToDoKey
        {
            get
            {
                if (_toDoList.Any())
                {
                    return _toDoList.Select(t => t.Id).Max() + 1; // get next ID based on max existing ID
                }
                return 1; // start from 1 if no ToDos exist
            }
        }

        private int nextProjectKey
        {
            get
            {
                if (_projectList.Any())
                {
                    return _projectList.Select(p => p.Id).Max() + 1; // get next ID based on max existing ID
                }
                return 1; // start from 1 if no Projects exist
            }
        }

        // ToDo methods
        public ToDo? AddOrUpdateToDo(ToDo? toDo)
        {
            if (toDo != null && toDo.Id == 0)
            {
                toDo.Id = nextToDoKey;
                _toDoList.Add(toDo);
            }
            else if (toDo != null)
            {
                var existing = _toDoList.FirstOrDefault(t => t.Id == toDo.Id);
                if (existing != null)
                {
                    existing.Name = toDo.Name;
                    existing.Description = toDo.Description;
                    existing.Priority = toDo.Priority;
                    existing.IsComplete = toDo.IsComplete;
                    existing.ProjectId = toDo.ProjectId;
                }
            }
            return toDo;
        }

        public ToDo? GetToDoById(int id)
        {
            return _toDoList.FirstOrDefault(t => t.Id == id);
        }

        public void DisplayToDos()
        {
            Console.WriteLine("All ToDos:");
            if (_toDoList.Count == 0)
            {
                Console.WriteLine("No ToDos found.");
            }
            else
            {
                foreach (var t in _toDoList)
                {
                    var projectInfo = t.ProjectId.HasValue
                        ? $" (Project ID: {t.ProjectId})"
                        : " (No Project)";

                    Console.WriteLine($"[{t.Id}] {t.Name} - {t.Description} (Priority {t.Priority}) - Complete: {t.IsComplete}{projectInfo}");
                }
            }
        }

        public void DeleteToDo(ToDo? toDo)
        {
            if (toDo == null) return;

            // Remove from project if assigned
            var project = _projectList.FirstOrDefault(p => p.Id == toDo.ProjectId);
            project?.ToDos.Remove(toDo);

            _toDoList.Remove(toDo);
        }

        public bool AssignToDoToProject(int toDoId, int? projectId)
        {
            var toDo = GetToDoById(toDoId);
            if (toDo == null) return false;

            // Remove from current project
            var currentProject = _projectList.FirstOrDefault(p => p.Id == toDo.ProjectId);
            currentProject?.ToDos.Remove(toDo);

            if (projectId.HasValue && projectId > 0)
            {
                var targetProject = GetProjectById(projectId);
                if (targetProject != null)
                {
                    toDo.ProjectId = projectId;
                    targetProject.ToDos.Add(toDo);
                    return true;
                }
                return false;
            }
            else
            {
                toDo.ProjectId = null;
                return true;
            }
        }

        // Project methods
        public Project? AddOrUpdateProject(Project? project)
        {
            if (project != null && project.Id == 0)
            {
                project.Id = nextProjectKey;
                project.ToDos = new List<ToDo>();
                _projectList.Add(project);
            }
            else if (project != null)
            {
                var existing = _projectList.FirstOrDefault(p => p.Id == project.Id);
                if (existing != null)
                {
                    existing.Name = project.Name;
                    existing.Description = project.Description;
                }
            }
            return project;
        }

        public Project? GetProjectById(int? id)
        {
            if (!id.HasValue) return null;
            return _projectList.FirstOrDefault(p => p.Id == id);
        }

        public void DisplayProjects()
        {
            Console.WriteLine("All Projects:");
            if (_projectList.Count == 0)
            {
                Console.WriteLine("No projects found.");
            }
            else
            {
                foreach (var p in _projectList)
                {
                    double completed = p.ToDos.Count > 0 ? p.ToDos.Count(td => td.IsComplete) * 100.0 / p.ToDos.Count : 0;
                    Console.WriteLine($"[{p.Id}] {p.Name} - {p.Description} - Complete: {completed:F1}% ({p.ToDos.Count(td => td.IsComplete)}/{p.ToDos.Count} tasks)");
                }
            }
        }

        public void DeleteProject(Project? project)
        {
            if (project == null) return;

            // Unassign all ToDos from this project
            foreach (var todo in project.ToDos)
            {
                todo.ProjectId = null;
            }
            _projectList.Remove(project);
        }

        public void DisplayToDosByProject(int projectId)
        {
            var project = GetProjectById(projectId);
            if (project != null)
            {
                Console.WriteLine($"ToDos for project '{project.Name}':");
                var projectToDos = GetToDosByProject(projectId);
                if (projectToDos.Count == 0)
                {
                    Console.WriteLine("No ToDos assigned to this project.");
                }
                else
                {
                    foreach (var t in projectToDos)
                    {
                        Console.WriteLine($"[{t.Id}] {t.Name} - {t.Description} (Priority {t.Priority}) - Complete: {t.IsComplete}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Project not found.");
            }
        }

        public List<ToDo> GetToDosByProject(int projectId)
        {
            var project = GetProjectById(projectId);
            return project?.ToDos.ToList() ?? new List<ToDo>();
        }
    }
}
