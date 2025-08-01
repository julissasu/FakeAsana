using System.Text;
using System.Text.Json;

namespace Asana.Maui.Services
{
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
            _httpClient = new HttpClient();
            _baseUrl = baseUrl;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Project methods
        public async Task<List<ProjectDto>> GetProjectsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/projects");
                response.EnsureSuccessStatusCode();

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
                var json = JsonSerializer.Serialize(project, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/projects", content);
                response.EnsureSuccessStatusCode();

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
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/todos");
                response.EnsureSuccessStatusCode();

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
                var json = JsonSerializer.Serialize(todo, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/api/todos", content);
                response.EnsureSuccessStatusCode();

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
            _httpClient?.Dispose();
        }
    }
}
