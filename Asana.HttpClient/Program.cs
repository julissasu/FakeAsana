using Asana.HttpClient.Models;
using Asana.HttpClient.Services;

namespace Asana.HttpClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Asana API HttpClient Demo ===\n");
            
            var client = new AsanaApiClient();

            try
            {
                // Test Projects API
                Console.WriteLine("📋 Testing Projects API:");
                
                // GET all projects
                Console.WriteLine("\n1. Getting all projects...");
                var projects = await client.GetProjectsAsync();
                Console.WriteLine($"Found {projects.Count} projects");

                // POST - Create a new project
                Console.WriteLine("\n2. Creating a new project...");
                var newProject = new CreateProjectDto
                {
                    Name = "HttpClient Test Project",
                    Description = "Created via HttpClient"
                };
                
                var createdProject = await client.CreateProjectAsync(newProject);
                if (createdProject != null)
                {
                    Console.WriteLine($"✅ Created project: {createdProject.Name} (ID: {createdProject.Id})");
                    
                    // GET single project
                    Console.WriteLine("\n3. Getting the created project...");
                    var retrievedProject = await client.GetProjectAsync(createdProject.Id);
                    if (retrievedProject != null)
                    {
                        Console.WriteLine($"✅ Retrieved: {retrievedProject.Name} - {retrievedProject.Description}");
                    }
                }

                // Test ToDos API
                Console.WriteLine("\n\n📝 Testing ToDos API:");
                
                // GET all todos
                Console.WriteLine("\n1. Getting all todos...");
                var todos = await client.GetToDosAsync();
                Console.WriteLine($"Found {todos.Count} todos");

                // POST - Create a new todo
                Console.WriteLine("\n2. Creating a new todo...");
                var newTodo = new CreateToDoDto
                {
                    Name = "HttpClient Test Task",
                    Description = "Created via HttpClient API call",
                    Priority = 2,
                    DueDate = DateTime.Now.AddDays(7),
                    ProjectId = createdProject?.Id
                };
                
                var createdTodo = await client.CreateToDoAsync(newTodo);
                if (createdTodo != null)
                {
                    Console.WriteLine($"✅ Created todo: {createdTodo.Name} (ID: {createdTodo.Id})");
                    
                    // GET single todo
                    Console.WriteLine("\n3. Getting the created todo...");
                    var retrievedTodo = await client.GetToDoAsync(createdTodo.Id);
                    if (retrievedTodo != null)
                    {
                        Console.WriteLine($"✅ Retrieved: {retrievedTodo.Name} - Priority: {retrievedTodo.Priority}");
                    }
                }

                // DELETE operations
                Console.WriteLine("\n\n🗑️ Testing DELETE operations:");
                
                if (createdTodo != null)
                {
                    Console.WriteLine($"\n1. Deleting todo (ID: {createdTodo.Id})...");
                    var todoDeleted = await client.DeleteToDoAsync(createdTodo.Id);
                    Console.WriteLine(todoDeleted ? "✅ Todo deleted successfully" : "❌ Failed to delete todo");
                }

                if (createdProject != null)
                {
                    Console.WriteLine($"\n2. Deleting project (ID: {createdProject.Id})...");
                    var projectDeleted = await client.DeleteProjectAsync(createdProject.Id);
                    Console.WriteLine(projectDeleted ? "✅ Project deleted successfully" : "❌ Failed to delete project");
                }

                Console.WriteLine("\n🎉 HttpClient demo completed successfully!");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ HTTP Error: {ex.Message}");
                Console.WriteLine("Make sure the API is running at http://localhost:5000");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
            finally
            {
                client.Dispose();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
