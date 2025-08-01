using Asana.Library.Models;
using Asana.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Asana
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var toDoSvc = ToDoServiceProxy.Current; // Singleton instance of todo service proxy
            var projectSvc = ProjectServiceProxy.Current; // Singleton instance of project service proxy
            int choiceInt; // User menu choice

            do
            {
                // Display main menu options to user
                Console.WriteLine("\nChoose a menu option:");
                Console.WriteLine("1. Create a ToDo");
                Console.WriteLine("2. Delete a ToDo");
                Console.WriteLine("3. Update a ToDo");
                Console.WriteLine("4. List all ToDos");
                Console.WriteLine("5. Create a Project");
                Console.WriteLine("6. Delete a Project");
                Console.WriteLine("7. Update a Project");
                Console.WriteLine("8. List all Projects");
                Console.WriteLine("9. List all ToDos in a Given Project");
                Console.WriteLine("10. Test Filebase Database");
                Console.WriteLine("0. Exit");

                var choice = Console.ReadLine() ?? "0"; // Default to "0" if input is null

                // Validate user input is a valid integer
                if (int.TryParse(choice, out choiceInt))
                {
                    switch (choiceInt)
                    {
                        case 0:
                            // Exit the program
                            Console.WriteLine("Exiting...");
                            break;

                        case 1:
                            // Create a new ToDo with user input
                            Console.Write("Name: ");
                            var name = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                Console.WriteLine("Name cannot be empty. ToDo not created.");
                                break; // Skip creation if name is empty
                            }

                            Console.Write("Description: ");
                            var description = Console.ReadLine();

                            // Get priority with validation
                            Console.Write("Priority (1-3): ");
                            int.TryParse(Console.ReadLine(), out int priority);
                            if (priority < 1 || priority > 3)
                            {
                                priority = 1; // Default to 1 if invalid
                            }

                            // Ask for due date with validation
                            DateTime dueDate = DateTime.Now;
                            Console.Write("Due Date (MM/dd/yyyy) or press Enter for today: ");
                            var dueDateInput = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(dueDateInput))
                            {
                                if (!DateTime.TryParse(dueDateInput, out dueDate))
                                {
                                    Console.WriteLine("Invalid date format. Using today's date.");
                                    dueDate = DateTime.Now; // default to today if parsing fails
                                }
                            }

                            // Create the new ToDo object
                            var newToDo = toDoSvc.AddOrUpdateToDo(new ToDo
                            {
                                Name = name,
                                Description = description,
                                Priority = priority,
                                IsComplete = false,
                                DueDate = dueDate,
                                Id = 0 // Will be assigned by service
                            });

                            // Optionally assign to a project if projects exist
                            if (newToDo != null)
                            {
                                if (projectSvc.Projects.Count > 0)
                                {
                                    Console.Write("Assign to Project ID (or leave empty): ");
                                    var projInput = Console.ReadLine();
                                    if (int.TryParse(projInput, out int projId))    // search for project by ID
                                    {
                                        if (toDoSvc.AssignToDoToProject(newToDo.Id, projId))
                                        {
                                            // assign ToDo to project
                                            var project = projectSvc.GetProjectById(projId);
                                            Console.WriteLine($"ToDo assigned to project '{project?.Name}'.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Project not found. ToDo created without project.");
                                        }
                                    }
                                }
                                Console.WriteLine($"ToDo created with ID {newToDo.Id}.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to create ToDo.");
                            }
                            break;

                        case 2:
                            // Delete an existing ToDo
                            DisplayToDos(toDoSvc, true); // Show all todos for selection
                            Console.Write("ToDo ID to Delete: ");
                            var deleteChoice = int.Parse(Console.ReadLine() ?? "0");
                            var toDoToDelete = toDoSvc.GetToDoById(deleteChoice);   // search for ToDo by ID
                            if (toDoToDelete != null)
                            {
                                toDoSvc.DeleteToDo(toDoToDelete);
                                Console.WriteLine("ToDo deleted successfully.");
                            }
                            else
                            {
                                Console.WriteLine("ToDo not found.");
                            }
                            break;

                        case 3:
                            // Update an existing ToDo 
                            DisplayToDos(toDoSvc, true); // Show all todos for selection
                            Console.Write("ToDo ID to Update: ");
                            var updateChoice = int.Parse(Console.ReadLine() ?? "0");
                            var toDoToUpdate = toDoSvc.GetToDoById(updateChoice);

                            if (toDoToUpdate != null)
                            {
                                // Display current ToDo details and prompt for updates
                                Console.WriteLine($"Updating ToDo: {toDoToUpdate.Name}");
                                Console.WriteLine("What would you like to update?");
                                Console.WriteLine("1. Toggle completion status");
                                Console.WriteLine("2. Change project assignment");
                                Console.WriteLine("3. Edit name");
                                Console.WriteLine("4. Edit description");
                                Console.WriteLine("5. Change priority");
                                Console.WriteLine("6. Change due date");
                                Console.Write("Enter choice (1-6): ");
                                var updateType = Console.ReadLine();

                                switch (updateType)
                                {
                                    case "1":
                                        // Toggle completion status 
                                        toDoToUpdate.IsComplete = !toDoToUpdate.IsComplete;
                                        toDoSvc.AddOrUpdateToDo(toDoToUpdate);
                                        Console.WriteLine($"Completion status changed to: {(toDoToUpdate.IsComplete ? "Complete" : "Incomplete")}");
                                        break;

                                    case "2":
                                        // Change project assignment
                                        DisplayProjects(projectSvc);
                                        Console.Write("Enter Project ID to assign (or 0 to unassign): ");
                                        if (int.TryParse(Console.ReadLine(), out int newProjectId))
                                        {
                                            if (toDoSvc.AssignToDoToProject(updateChoice, newProjectId == 0 ? null : newProjectId))
                                            {
                                                Console.WriteLine("Project assignment updated.");
                                            }
                                            else
                                            {
                                                Console.WriteLine("Failed to update project assignment.");
                                            }
                                        }
                                        break;

                                    case "3":
                                        // Edit name 
                                        Console.Write($"Current name: {toDoToUpdate.Name}\nNew name: ");
                                        var newName = Console.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(newName))
                                        {
                                            toDoToUpdate.Name = newName;
                                            toDoSvc.AddOrUpdateToDo(toDoToUpdate);
                                            Console.WriteLine("Name updated.");
                                        }
                                        break;

                                    case "4":
                                        // Edit description
                                        Console.Write($"Current description: {toDoToUpdate.Description}\nNew description: ");
                                        toDoToUpdate.Description = Console.ReadLine();
                                        toDoSvc.AddOrUpdateToDo(toDoToUpdate);
                                        Console.WriteLine("Description updated.");
                                        break;

                                    case "5":
                                        // Change priority
                                        Console.Write($"Current priority: {toDoToUpdate.Priority}\nNew priority (1-3): ");
                                        if (int.TryParse(Console.ReadLine(), out int newPriority) && newPriority >= 1 && newPriority <= 3)
                                        {
                                            toDoToUpdate.Priority = newPriority;
                                            toDoSvc.AddOrUpdateToDo(toDoToUpdate);
                                            Console.WriteLine("Priority updated.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid priority. No changes made.");
                                        }
                                        break;

                                    case "6":
                                        // Change due date
                                        Console.Write($"Current due date: {toDoToUpdate.DueDate:MM/dd/yyyy}\nNew due date (MM/dd/yyyy): ");
                                        var newDueDateInput = Console.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(newDueDateInput) && DateTime.TryParse(newDueDateInput, out DateTime newDueDate))
                                        {
                                            toDoToUpdate.DueDate = newDueDate;
                                            toDoSvc.AddOrUpdateToDo(toDoToUpdate);
                                            Console.WriteLine("Due date updated.");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid date format. No changes made.");
                                        }
                                        break;
                                    default:
                                        Console.WriteLine("Invalid choice.");
                                        break;
                                }
                            }
                            else
                            {
                                Console.WriteLine("ToDo not found.");
                            }
                            break;

                        case 4:
                            // Display all todos
                            DisplayToDos(toDoSvc, true);
                            break;

                        case 5:
                            // Create a new Project
                            Console.Write("Project Name: ");
                            var projName = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(projName))
                            {
                                Console.WriteLine("Name cannot be empty. Project not created.");
                                break; // Skip creation if name is empty
                            }

                            Console.Write("Project Description: ");
                            var projDescription = Console.ReadLine();

                            // Create the new Project object
                            var newProject = projectSvc.AddOrUpdateProject(new Project
                            {
                                Name = projName,
                                Description = projDescription,
                                CompletePercent = 0,
                                Id = 0 // Will be assigned by service
                            });
                            if (newProject != null)
                            {
                                Console.WriteLine($"Project created with ID {newProject.Id}.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to create Project.");
                            }
                            break;

                        case 6:
                            // Delete an existing Project
                            DisplayProjects(projectSvc);
                            Console.Write("Project ID to Delete: ");
                            var deleteProjChoice = int.Parse(Console.ReadLine() ?? "0");
                            var projectToDelete = projectSvc.GetProjectById(deleteProjChoice); // search for Project by ID
                            if (projectToDelete != null)
                            {
                                toDoSvc.DeleteProject(projectToDelete); // This is an important detail. ToDoService still needs to know to unassign todos.
                                Console.WriteLine("Project deleted successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Project not found.");
                            }
                            break;

                        case 7:
                            // Update an existing Project
                            DisplayProjects(projectSvc);
                            Console.Write("Project ID to Update: ");
                            var updateProjChoice = int.Parse(Console.ReadLine() ?? "0");
                            var projectToUpdate = projectSvc.GetProjectById(updateProjChoice); // search for Project by ID

                            // If project exists, prompt for updates
                            if (projectToUpdate != null)
                            {
                                Console.Write($"Current Name: {projectToUpdate.Name}\nNew Name (or press Enter to keep current): ");
                                var newProjName = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(newProjName))
                                    projectToUpdate.Name = newProjName;

                                Console.Write($"Current Description: {projectToUpdate.Description}\nNew Description (or press Enter to keep current): ");
                                var newProjDesc = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(newProjDesc))
                                    projectToUpdate.Description = newProjDesc;

                                projectSvc.AddOrUpdateProject(projectToUpdate);
                                Console.WriteLine("Project updated successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Project not found.");
                            }
                            break;

                        case 8:
                            // Display all Projects
                            DisplayProjects(projectSvc);
                            break;

                        case 9:
                            // Display ToDos filtered by a specific Project
                            DisplayProjects(projectSvc);
                            Console.Write("Enter Project ID to view its ToDos: ");
                            var viewProjChoice = int.Parse(Console.ReadLine() ?? "0");  // search for Project by ID
                            DisplayToDosByProject(toDoSvc, viewProjChoice);
                            break;

                        case 10:
                            Console.WriteLine("\nDatabase System: Using Filebase for persistent storage.");
                            Console.WriteLine("All TODO and Project data is automatically saved to JSON files.");
                            break;

                        default:
                            // Handle invalid menu selection
                            Console.WriteLine("ERROR: Unknown menu selection.");
                            break;
                    }
                }
                else // Handle non-integer input
                {
                    Console.WriteLine("ERROR: Invalid menu selection.");
                }
            } while (choiceInt != 0);
        }

        // Function to display all ToDos, optionally filtered by completion status
        private static void DisplayToDos(ToDoServiceProxy service, bool isShowCompleted = false)
        {
            var todos = service.ToDos;
            // Filter out completed tasks if not requested
            if (!isShowCompleted)
            {
                todos = todos.Where(t => !t.IsComplete).ToList();
            }

            // Handle empty results
            if (todos.Count == 0)
            {
                Console.WriteLine(isShowCompleted ? "No tasks found." : "No incomplete tasks found.");
                return;
            }

            // Display header based on filter
            Console.WriteLine(isShowCompleted ? "All Tasks:" : "Incomplete Tasks:");
            todos.ForEach(Console.WriteLine);
        }

        // Function to display all Projects
        private static void DisplayProjects(ProjectServiceProxy service)
        {
            if (service.Projects.Count == 0)
            {
                Console.WriteLine("No projects found.");
                return;
            }

            Console.WriteLine("Projects:");
            foreach (var project in service.Projects)
            {
                // Calculate completion statistics for each project
                var projectToDos = ToDoServiceProxy.Current.GetToDosByProject(project.Id);
                var completedCount = projectToDos.Count(t => t.IsComplete);
                var totalCount = projectToDos.Count;
                var percentage = totalCount > 0 ? (completedCount * 100.0 / totalCount) : 0;

                // Display project info with completion stats
                Console.WriteLine($"[{project.Id}] {project.Name} - {project.Description}");
                Console.WriteLine($"    Tasks: {completedCount}/{totalCount} completed ({percentage:F0}%)");
            }
        }

        // Function to display ToDos associated with a specific Project
        private static void DisplayToDosByProject(ToDoServiceProxy toDoSvc, int projectId)
        {
            var projectSvc = ProjectServiceProxy.Current;
            // Validate project exists
            var project = projectSvc.GetProjectById(projectId);
            if (project == null)
            {
                Console.WriteLine("Project not found.");
                return;
            }

            // Get all todos assigned to this project
            var projectToDos = toDoSvc.GetToDosByProject(projectId);
            if (projectToDos.Count == 0)
            {
                Console.WriteLine($"No tasks found in project '{project.Name}'.");
                return;
            }

            // Display project header and associated tasks
            Console.WriteLine($"Tasks in project '{project.Name}':");
            projectToDos.ForEach(Console.WriteLine);
        }
    }
}