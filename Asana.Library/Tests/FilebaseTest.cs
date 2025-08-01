using Asana.Library.Data;
using Asana.Library.Models;
using Asana.Library.Services;
using System;
using System.Collections.Generic;

namespace Asana.Library.Tests
{
    public class FilebaseTest
    {
        public static void TestFilebaseOperations()
        {
            Console.WriteLine("=== Filebase Implementation Test ===");
            
            try
            {
                // Test 1: Direct Filebase operations
                Console.WriteLine("\n1. Testing direct Filebase operations...");
                var filebase = new Filebase();
                
                var testData = new List<string> { "Test1", "Test2", "Test3" };
                filebase.Save("test_table", testData);
                Console.WriteLine("✓ Data saved to Filebase");
                
                var loadedData = filebase.Load<List<string>>("test_table");
                Console.WriteLine($"✓ Data loaded from Filebase: {loadedData?.Count ?? 0} items");
                
                // Test 2: ToDoServiceProxy with Filebase integration
                Console.WriteLine("\n2. Testing ToDoServiceProxy with Filebase...");
                var todoService = ToDoServiceProxy.Current;
                
                // Add some test todos
                var todo1 = new ToDo
                {
                    Name = "Test Filebase Integration",
                    Description = "This todo should be saved to Filebase",
                    Priority = 2,
                    DueDate = DateTime.Now.AddDays(1)
                };
                
                var savedTodo = todoService.AddOrUpdateToDo(todo1);
                Console.WriteLine($"✓ ToDo saved: ID={savedTodo?.Id}, Name='{savedTodo?.Name}'");
                
                var allTodos = todoService.ToDos;
                Console.WriteLine($"✓ Total ToDos in storage: {allTodos.Count}");
                
                // Test 3: ProjectServiceProxy with FakeDatabase
                Console.WriteLine("\n3. Testing ProjectServiceProxy with FakeDatabase...");
                var projectService = ProjectServiceProxy.Current;
                
                var projects = projectService.Projects;
                Console.WriteLine($"✓ Projects loaded from FakeDatabase: {projects.Count}");
                
                var stats = projectService.GetProjectStatistics();
                Console.WriteLine($"✓ Project Statistics - Total: {stats.TotalProjects}, Completed: {stats.CompletedProjects}");
                
                Console.WriteLine("\n=== All tests completed successfully! ===");
                
                // Show directory info
                var platform = Environment.OSVersion.Platform == PlatformID.Win32NT ? "Windows (C:\\temp)" : $"Unix-like ({System.IO.Path.GetTempPath()}Filebase)";
                Console.WriteLine($"\nFilebase storage location: {platform}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed: {ex.Message}");
                Console.WriteLine($"Details: {ex}");
            }
        }
    }
}
