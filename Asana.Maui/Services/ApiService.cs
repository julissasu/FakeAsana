using System.Text;
using System.Text.Json;

namespace Asana.Maui.Services
{
    // DTOs for Projects and ToDos
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CompletePercent { get; set; }
    }

    public class CreateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class ToDoDto
    {
        // Properties for ToDo
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; } = 1;
        public bool IsComplete { get; set; }
        public int? ProjectId { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class CreateToDoDto
    {
        // Properties for creating a new ToDo
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; } = 1;
        public int? ProjectId { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(string baseUrl = "http://localhost:5000")
        {
            // Initialize HttpClient and base URL
            _httpClient = new HttpClient();
            _baseUrl = baseUrl;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Use camelCase for JSON serialization
            };
        }

        // Project methods
        public async Task<List<ProjectDto>> GetProjectsAsync()
        {
            try
            {
                // Make API call to get projects
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/projects");
                response.EnsureSuccessStatusCode();

                // Deserialize JSON response to list of ProjectDto
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ProjectDto>>(json, _jsonOptions) ?? new List<ProjectDto>();
            }
            catch
            {
                return new List<ProjectDto>(); // Return empty list on error
            }
        }

        public async Task<ProjectDto?> CreateProjectAsync(CreateProjectDto project)
        {
            try
            {
                // Serialize project to JSON and make POST request
                var json = JsonSerializer.Serialize(project, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Make API call to create project
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/projects", content);
                response.EnsureSuccessStatusCode();

                // Deserialize response to ProjectDto
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ProjectDto>(responseJson, _jsonOptions);
            }
            catch
            {
                return null; // Return null on error
            }
        }

        // ToDo methods
        public async Task<List<ToDoDto>> GetToDosAsync()
        {
            try
            {
                // Make API call to get ToDos
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/todos");
                response.EnsureSuccessStatusCode();

                // Deserialize JSON response to list of ToDoDto
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ToDoDto>>(json, _jsonOptions) ?? new List<ToDoDto>();
            }
            catch
            {
                return new List<ToDoDto>(); // Return empty list on error
            }
        }

        public async Task<ToDoDto?> CreateToDoAsync(CreateToDoDto todo)
        {
            try
            {
                // Serialize ToDo to JSON and make POST request
                var json = JsonSerializer.Serialize(todo, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Make API call to create ToDo
                var response = await _httpClient.PostAsync($"{_baseUrl}/api/todos", content);
                response.EnsureSuccessStatusCode();

                // Deserialize response to ToDoDto
                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ToDoDto>(responseJson, _jsonOptions);
            }
            catch
            {
                return null; // Return null on error
            }
        }

        public async Task<bool> DeleteToDoAsync(int id)
        {
            try
            {
                // Make API call to delete ToDo by ID
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/todos/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose(); // Dispose HttpClient when done
        }
    }
}
