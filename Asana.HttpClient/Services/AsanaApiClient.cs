using System.Text.Json;
using System.Text;
using Asana.HttpClient.Models;

namespace Asana.HttpClient.Services
{
    public class AsanaApiClient
    {
        private readonly System.Net.Http.HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public AsanaApiClient(string baseUrl = "http://localhost:5001")
        {
            _httpClient = new System.Net.Http.HttpClient();
            _baseUrl = baseUrl;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Project methods
        public async Task<List<ProjectDto>> GetProjectsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/projects");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProjectDto>>(json, _jsonOptions) ?? new List<ProjectDto>();
        }

        public async Task<ProjectDto?> GetProjectAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/projects/{id}");
            if (!response.IsSuccessStatusCode) return null;
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProjectDto>(json, _jsonOptions);
        }

        public async Task<ProjectDto?> CreateProjectAsync(CreateProjectDto project)
        {
            var json = JsonSerializer.Serialize(project, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/projects", content);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProjectDto>(responseJson, _jsonOptions);
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/projects/{id}");
            return response.IsSuccessStatusCode;
        }

        // ToDo methods
        public async Task<List<ToDoDto>> GetToDosAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/todos");
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ToDoDto>>(json, _jsonOptions) ?? new List<ToDoDto>();
        }

        public async Task<ToDoDto?> GetToDoAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/todos/{id}");
            if (!response.IsSuccessStatusCode) return null;
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ToDoDto>(json, _jsonOptions);
        }

        public async Task<ToDoDto?> CreateToDoAsync(CreateToDoDto todo)
        {
            var json = JsonSerializer.Serialize(todo, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/todos", content);
            response.EnsureSuccessStatusCode();
            
            var responseJson = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ToDoDto>(responseJson, _jsonOptions);
        }

        public async Task<bool> DeleteToDoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/todos/{id}");
            return response.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
