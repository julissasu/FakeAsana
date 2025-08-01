using Asana.API.Database;
using Asana.Library.Models;
using Asana.Library.Services;

namespace Asana.API.Services
{
    public class FilebasePersistenceService
    {
        private readonly Filebase _filebase;
        private const string TODO_TABLE = "todos";
        private const string PROJECT_TABLE = "projects";

        public FilebasePersistenceService(Filebase filebase)
        {
            _filebase = filebase;
            LoadDataFromFilebase();
        }

        private void LoadDataFromFilebase()
        {
            LoadToDosFromFilebase();
            LoadProjectsFromFilebase();
        }

        private void LoadToDosFromFilebase()
        {
            try
            {
                var loadedTodos = _filebase.Load<List<ToDo>>(TODO_TABLE);
                if (loadedTodos != null && loadedTodos.Any())
                {
                    var todoService = ToDoServiceProxy.Current;

                    // Clear existing todos
                    var currentTodos = todoService.ToDos.ToList();
                    foreach (var currentTodo in currentTodos)
                    {
                        todoService.DeleteToDo(currentTodo);
                    }

                    // Add loaded todos
                    foreach (var todo in loadedTodos)
                    {
                        todoService.AddOrUpdateToDo(todo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load todos from Filebase: {ex.Message}");
            }
        }

        private void LoadProjectsFromFilebase()
        {
            try
            {
                var loadedProjects = _filebase.Load<List<Project>>(PROJECT_TABLE);
                
                if (loadedProjects != null && loadedProjects.Any())
                {
                    var projectService = ProjectServiceProxy.Current;

                    // Clear existing projects
                    var currentProjects = projectService.Projects.ToList();
                    foreach (var currentProject in currentProjects)
                    {
                        projectService.DeleteProject(currentProject);
                    }

                    // Add loaded projects
                    foreach (var project in loadedProjects)
                    {
                        projectService.AddOrUpdateProject(project);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load projects from Filebase: {ex.Message}");
            }
        }

        public void SaveToDos()
        {
            try
            {
                var todoService = ToDoServiceProxy.Current;
                _filebase.Save(TODO_TABLE, todoService.ToDos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not save todos to Filebase: {ex.Message}");
            }
        }

        public void SaveProjects()
        {
            try
            {
                var projectService = ProjectServiceProxy.Current;
                _filebase.Save(PROJECT_TABLE, projectService.Projects);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not save projects to Filebase: {ex.Message}");
            }
        }

        public void SaveAll()
        {
            SaveToDos();
            SaveProjects();
        }

        public async Task SaveToDosAsync()
        {
            try
            {
                var todoService = ToDoServiceProxy.Current;
                await _filebase.SaveAsync(TODO_TABLE, todoService.ToDos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not save todos to Filebase: {ex.Message}");
            }
        }

        public async Task SaveProjectsAsync()
        {
            try
            {
                var projectService = ProjectServiceProxy.Current;
                await _filebase.SaveAsync(PROJECT_TABLE, projectService.Projects);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not save projects to Filebase: {ex.Message}");
            }
        }

        public async Task SaveAllAsync()
        {
            await SaveToDosAsync();
            await SaveProjectsAsync();
        }
    }
}
