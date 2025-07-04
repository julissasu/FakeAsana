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
            var toDoSvc = ToDoServiceProxy.Current; // create an instance of the service proxy
            int choiceInt; // variable to hold user choice

            do
            {
                // display menu options
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
                Console.WriteLine("0. Exit");

                var choice = Console.ReadLine() ?? "0"; // default to "0" if null

                if (int.TryParse(choice, out choiceInt))
                {
                    switch (choiceInt)
                    {
                        case 0:
                            // exit the program
                            Console.WriteLine("Exiting...");
                            break;

                        case 1:
                            Console.Write("Name: ");
                            var name = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                Console.WriteLine("Name cannot be empty. ToDo not created.");
                                break; // skip creation if name is empty
                            }

                            Console.Write("Description: ");
                            var description = Console.ReadLine();

                            Console.Write("Priority (1-3): ");
                            int.TryParse(Console.ReadLine(), out int priority);
                            if (priority < 1 || priority > 3)
                            {
                                priority = 1; // default to 1 if invalid
                            }

                            var newToDo = toDoSvc.AddOrUpdateToDo(new ToDo
                            {
                                Name = name,
                                Description = description,
                                Priority = priority,
                                IsComplete = false,
                                Id = 0
                            });

                            if (newToDo != null)
                            {
                                if (toDoSvc.Projects.Count > 0)
                                {
                                    Console.Write("Assign to Project ID (or leave empty): ");
                                    var projInput = Console.ReadLine();
                                    if (int.TryParse(projInput, out int projId))
                                    {
                                        if (toDoSvc.AssignToDoToProject(newToDo.Id, projId))
                                        {
                                            var project = toDoSvc.GetProjectById(projId);
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
                                Console.WriteLine("ToDo created without project assignment.");
                            }
                            break;

                        case 2:
                            DisplayToDos(toDoSvc);
                            Console.Write("ToDo ID to Delete: ");
                            var deleteChoice = int.Parse(Console.ReadLine() ?? "0");
                            var toDoToDelete = toDoSvc.GetToDoById(deleteChoice);
                            toDoSvc.DeleteToDo(toDoToDelete);
                            break;

                        case 3:
                            DisplayToDos(toDoSvc);
                            Console.Write("ToDo ID to Update: ");
                            var updateChoice = int.Parse(Console.ReadLine() ?? "0");
                            var toDoToUpdate = toDoSvc.GetToDoById(updateChoice);

                            if (toDoToUpdate != null)
                            {
                                Console.WriteLine("What would you like to update?");
                                Console.WriteLine("1. Toggle completion status");
                                Console.WriteLine("2. Change project assignment");
                                Console.Write("Enter choice (1 or 2): ");
                                var updateType = Console.ReadLine();

                                if (updateType == "1")
                                {
                                    toDoToUpdate.IsComplete = !toDoToUpdate.IsComplete;
                                    toDoSvc.AddOrUpdateToDo(toDoToUpdate);
                                }
                                else if (updateType == "2")
                                {
                                    DisplayProjects(toDoSvc);
                                    Console.Write("Enter Project ID to assign (or 0 to unassign): ");
                                    if (int.TryParse(Console.ReadLine(), out int newProjectId))
                                    {
                                        toDoSvc.AssignToDoToProject(updateChoice, newProjectId == 0 ? null : newProjectId);
                                    }
                                }
                            }
                            break;

                        case 4:
                            DisplayToDos(toDoSvc);
                            break;

                        case 5:
                            Console.Write("Project Name: ");
                            var projName = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(projName))
                            {
                                Console.WriteLine("Name cannot be empty. Project not created.");
                                break; // skip creation if name is empty
                            }

                            Console.Write("Project Description: ");
                            var projDescription = Console.ReadLine();

                            var newProject = toDoSvc.AddOrUpdateProject(new Project
                            {
                                Name = projName,
                                Description = projDescription,
                                CompletePercent = 0,
                                Id = 0
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
                            DisplayProjects(toDoSvc);
                            Console.Write("Project ID to Delete: ");
                            var deleteProjChoice = int.Parse(Console.ReadLine() ?? "0");
                            var projectToDelete = toDoSvc.GetProjectById(deleteProjChoice);
                            toDoSvc.DeleteProject(projectToDelete);
                            break;

                        case 7:
                            DisplayProjects(toDoSvc);
                            Console.Write("Project ID to Update: ");
                            var updateProjChoice = int.Parse(Console.ReadLine() ?? "0");
                            var projectToUpdate = toDoSvc.GetProjectById(updateProjChoice);

                            if (projectToUpdate != null)
                            {
                                Console.Write("New Name: ");
                                projectToUpdate.Name = Console.ReadLine();
                                Console.Write("New Description: ");
                                projectToUpdate.Description = Console.ReadLine();
                                toDoSvc.AddOrUpdateProject(projectToUpdate);
                            }
                            break;

                        case 8:
                            DisplayProjects(toDoSvc);
                            break;

                        case 9:
                            DisplayProjects(toDoSvc);
                            Console.Write("Enter Project ID to view its ToDos: ");
                            var viewProjChoice = int.Parse(Console.ReadLine() ?? "0");
                            DisplayToDosByProject(toDoSvc, viewProjChoice);
                            break;

                        default:
                            // handle invalid choice
                            Console.WriteLine("ERROR: Unknown menu selection.");
                            break;
                    }
                }
                else // handle non-integer input
                {
                    Console.WriteLine("ERROR: Invalid menu selection.");
                }
            } while (choiceInt != 0);
        }

        // Helper methods for display (moved from service to CLI)
        private static void DisplayToDos(ToDoServiceProxy service, bool isShowCompleted = false)
        {
            var todos = service.ToDos;
            if (!isShowCompleted)
            {
                todos = todos.Where(t => !t.IsComplete).ToList();
            }

            if (todos.Count == 0)
            {
                Console.WriteLine("No tasks found.");
                return;
            }

            todos.ForEach(Console.WriteLine);
        }

        private static void DisplayProjects(ToDoServiceProxy service)
        {
            if (service.Projects.Count == 0)
            {
                Console.WriteLine("No projects found.");
                return;
            }

            Console.WriteLine("Projects:");
            service.Projects.ForEach(p => Console.WriteLine($"[{p.Id}] {p.Name} - {p.Description}"));
        }

        private static void DisplayToDosByProject(ToDoServiceProxy service, int projectId)
        {
            var project = service.GetProjectById(projectId);
            if (project == null)
            {
                Console.WriteLine("Project not found.");
                return;
            }

            var projectToDos = service.GetToDosByProject(projectId);
            if (projectToDos.Count == 0)
            {
                Console.WriteLine($"No tasks found in project '{project.Name}'.");
                return;
            }

            Console.WriteLine($"Tasks in project '{project.Name}':");
            projectToDos.ForEach(Console.WriteLine);
        }
    }
}