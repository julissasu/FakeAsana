using Asana.API.Database;
using Asana.Library.Models;
using Asana.Library.Services;

namespace Asana.API.Services
{
    public class FilebasePersistenceService
    {
        private readonly Filebase _filebase;
        private const string TODO_TABLE = "todos";

        public FilebasePersistenceService(Filebase filebase)
        {
            _filebase = filebase;
            LoadToDosFromFilebase();
        }

        private void LoadToDosFromFilebase()
        {
            try
            {
                var loadedTodos = _filebase.Load<List<ToDo>>(TODO_TABLE);
                if (loadedTodos != null && loadedTodos.Any())
                {
                    // Replace the in-memory todos with the loaded ones
                    var todoService = ToDoServiceProxy.Current;
                    foreach (var todo in loadedTodos)
                    {
                        todoService.AddOrUpdateToDo(todo);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but continue with empty list
                Console.WriteLine($"Warning: Could not load todos from Filebase: {ex.Message}");
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
    }
}
