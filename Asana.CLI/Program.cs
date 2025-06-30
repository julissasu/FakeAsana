using Asana.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Asana
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // initialize lists for ToDos and Projects
            var toDos = new List<ToDo>();
            var projects = new List<Project>();

            // initialize IDs for ToDos and Projects
            int nextToDoId = 1;
            int nextProjectId = 1;

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
                            CreateToDo(toDos, projects, ref nextToDoId);
                            break;
                        case 2:
                            DeleteToDo(toDos, projects);
                            break;
                        case 3:
                            UpdateToDo(toDos, projects);
                            break;
                        case 4:
                            DisplayAllToDos(toDos);
                            break;
                        case 5:
                            CreateProject(projects, ref nextProjectId);
                            break;
                        case 6:
                            DeleteProject(projects);
                            break;
                        case 7:
                            UpdateProject(projects);
                            break;
                        case 8:
                            DisplayAllProjects(projects);
                            break;
                        case 9:
                            DisplayToDosByProject(projects);
                            break;

                        default:
                            // handle invalid choice
                            Console.WriteLine("Invalid choice, please try again.");
                            break;
                    }
                }
                else // handle non-integer input
                {
                    Console.WriteLine("Invalid input, please enter a number.");
                }
            } while (choiceInt != 0);
        }

        // function to delete a ToDo
        private static void DeleteToDo(List<ToDo> toDos, List<Project> projects)
        {
            Console.Write("Enter ToDo ID to delete: ");
            int.TryParse(Console.ReadLine(), out int deleteId);
            var toDoDelete = toDos.FirstOrDefault(t => t.Id == deleteId);   // find ToDo by ID
            if (toDoDelete != null)
            {
                toDos.Remove(toDoDelete);   // remove ToDo from the list
                var proj = projects.FirstOrDefault(p => p.Id == toDoDelete.ProjectId);  // find the project by ToDo's ProjectId
                proj?.ToDos.Remove(toDoDelete); // remove ToDo from the project if it exists

                Console.WriteLine($"ToDo with ID {deleteId} deleted.");
            }
            else
            {
                Console.WriteLine($"ToDo with ID {deleteId} not found");
            }
        }

        // function to create a new ToDo
        private static void CreateToDo(List<ToDo> toDos, List<Project> projects, ref int nextToDoId)
        {
            // prompt user for ToDo details
            Console.Write("Name: ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty. ToDo not created.");
                return; // skip creation if name is empty
            }

            Console.Write("Description: ");
            var description = Console.ReadLine();

            Console.Write("Priority (1-3): ");
            int.TryParse(Console.ReadLine(), out int priority);
            if (priority < 1 || priority > 3)
            {
                Console.WriteLine("Priority must be between 1-3. Setting to default 1.");
                priority = 1;   // default priority if invalid
            }

            var newToDo = new ToDo
            {
                Id = nextToDoId++,
                Name = name,
                Description = description,
                Priority = priority,
                IsComplete = false
            };

            toDos.Add(newToDo); // add new ToDo to the list

            if (projects.Count > 0)
            {
                // add ToDo to a project if it exists
                Console.Write("Assign to Project ID (or leave empty): ");
                var projInput = Console.ReadLine();
                if (int.TryParse(projInput, out int projId))
                {
                    // find the project by ID
                    var proj = projects.FirstOrDefault(p => p.Id == projId);
                    if (proj != null)
                    {
                        newToDo.ProjectId = proj.Id;
                        proj.ToDos.Add(newToDo);
                        Console.WriteLine($"ToDo assigned to project '{proj.Name}'.");
                    }
                    else
                    {
                        Console.WriteLine("Project not found. ToDo created without project.");
                    }
                }
            }
            Console.WriteLine($"ToDo created with ID {newToDo.Id}.");
        }

        // function to update a ToDo (completion or project assignment)
        private static void UpdateToDo(List<ToDo> toDos, List<Project> projects)
        {
            Console.Write("Enter ToDo Id to update: ");
            int.TryParse(Console.ReadLine(), out int updateId);
            var toDoUpdate = toDos.FirstOrDefault(t => t.Id == updateId);   // find ToDo by ID
            if (toDoUpdate != null)
            {
                Console.WriteLine($"Updating ToDo: '{toDoUpdate.Name}'");
                Console.WriteLine("What would you like to update?");
                Console.WriteLine("1. Toggle completion status");
                Console.WriteLine("2. Change project assignment");
                Console.Write("Enter choice (1 or 2): ");

                var updateChoice = Console.ReadLine();

                if (updateChoice == "1")
                {
                    // toggle completion status
                    toDoUpdate.IsComplete = !toDoUpdate.IsComplete;
                    string status = toDoUpdate.IsComplete ? "completed" : "incomplete";
                    Console.WriteLine($"ToDo '{toDoUpdate.Name}' marked as {status}.");
                }
                else if (updateChoice == "2")
                {
                    // change project assignment
                    var currentProject = projects.FirstOrDefault(p => p.Id == toDoUpdate.ProjectId);
                    Console.WriteLine($"Current project: {(currentProject?.Name ?? "None")}");

                    if (projects.Count == 0)
                    {
                        Console.WriteLine("No projects available. Create a project first.");
                    }
                    else
                    {
                        Console.WriteLine("Available projects:");
                        foreach (var p in projects)
                        {
                            Console.WriteLine($"  [{p.Id}] {p.Name}");
                        }
                        Console.Write("Enter Project ID to assign (or 0 to unassign): ");

                        if (int.TryParse(Console.ReadLine(), out int newProjectId))
                        {
                            if (newProjectId == 0)
                            {
                                // unassign from project
                                currentProject?.ToDos.Remove(toDoUpdate);
                                toDoUpdate.ProjectId = null;
                                Console.WriteLine($"ToDo '{toDoUpdate.Name}' unassigned from all projects.");
                            }
                            else
                            {
                                var targetProject = projects.FirstOrDefault(p => p.Id == newProjectId);
                                if (targetProject != null)
                                {
                                    // remove from current project if assigned
                                    currentProject?.ToDos.Remove(toDoUpdate);
                                    // add to new project
                                    targetProject.ToDos.Add(toDoUpdate);
                                    toDoUpdate.ProjectId = newProjectId;
                                    Console.WriteLine($"ToDo '{toDoUpdate.Name}' assigned to project '{targetProject.Name}'.");
                                }
                                else
                                {
                                    Console.WriteLine("Project not found. Assignment unchanged.");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Assignment unchanged.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. No changes made.");
                }
            }
            else
            {
                Console.WriteLine($"ToDo with ID {updateId} not found.");
            }
        }

        // function to display all ToDos
        private static void DisplayAllToDos(List<ToDo> toDos)
        {
            Console.WriteLine("All ToDos:");
            if (toDos.Count == 0)
            {
                Console.WriteLine("No ToDos found.");
            }
            else
            {
                foreach (var t in toDos)
                {
                    var projectInfo = t.ProjectId.HasValue
                        ? $" (Project ID: {t.ProjectId})"
                        : " (No Project)";

                    Console.WriteLine($"[{t.Id}] {t.Name} - {t.Description} (Priority {t.Priority}) - Complete: {t.IsComplete}{projectInfo}");
                }
            }
        }

        // function to create a new Project
        private static void CreateProject(List<Project> projects, ref int nextProjectId)
        {
            // prompt user for Project details
            Console.Write("Project Name: ");
            var projName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(projName))
            {
                Console.WriteLine("Project name cannot be empty. Project not created.");
                return; // skip creation if name is empty
            }

            Console.Write("Project Description: ");
            var projDescription = Console.ReadLine();

            var newProject = new Project
            {
                Id = nextProjectId++,
                Name = projName,
                Description = projDescription,
                CompletePercent = 0,
                ToDos = new List<ToDo>()
            };
            projects.Add(newProject);   // add new Project to the list

            Console.WriteLine($"Project created with ID {newProject.Id}.");
        }

        // function to delete an existing Project
        private static void DeleteProject(List<Project> projects)
        {
            Console.Write("Enter Project ID to delete: ");
            int.TryParse(Console.ReadLine(), out int deleteProjId);
            var projectDelete = projects.FirstOrDefault(p => p.Id == deleteProjId); // find Project by ID
            if (projectDelete != null)
            {
                // unassign all ToDos from this project
                foreach (var todo in projectDelete.ToDos)
                {
                    todo.ProjectId = null;
                }
                projects.Remove(projectDelete);
                Console.WriteLine($"Project with ID {deleteProjId} deleted. Associated ToDos unassigned.");
            }
            else
            {
                Console.WriteLine($"Project with ID {deleteProjId} not found.");
            }
        }

        // function to update an existing Project
        private static void UpdateProject(List<Project> projects)
        {
            Console.Write("Enter Project ID to update: ");
            int.TryParse(Console.ReadLine(), out int updateProjId);
            var projectUpdate = projects.FirstOrDefault(p => p.Id == updateProjId); // find Project by ID
            if (projectUpdate != null)
            {
                // prompt user for new values (allow keeping existing values)
                Console.Write($"New Name (current: '{projectUpdate.Name}', press Enter to keep): ");
                var newProjName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newProjName))
                    projectUpdate.Name = newProjName;

                Console.Write($"New Description (current: '{projectUpdate.Description}', press Enter to keep): ");
                var newProjDescription = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newProjDescription))
                    projectUpdate.Description = newProjDescription;

                Console.WriteLine($"Project with ID {updateProjId} updated.");
            }
            else
            {
                Console.WriteLine($"Project with ID {updateProjId} not found.");
            }
        }

        // function to list all Projects
        private static void DisplayAllProjects(List<Project> projects)
        {
            Console.WriteLine("All Projects:");
            if (projects.Count == 0)
            {
                Console.WriteLine("No projects found.");
            }
            else
            {
                foreach (var p in projects)
                {
                    // calculate completion percentage for each project
                    double completed = p.ToDos.Count > 0 ? p.ToDos.Count(td => td.IsComplete) * 100.0 / p.ToDos.Count : 0;
                    // display project details
                    Console.WriteLine($"[{p.Id}] {p.Name} - {p.Description} - Complete: {completed:F1}% ({p.ToDos.Count(td => td.IsComplete)}/{p.ToDos.Count} tasks)");
                }
            }
        }

        // function to list all ToDos in a given Project
        private static void DisplayToDosByProject(List<Project> projects)
        {
            Console.Write("Enter Project ID to view its ToDos: ");
            int.TryParse(Console.ReadLine(), out int pidToView);
            var projectToView = projects.FirstOrDefault(p => p.Id == pidToView);    // find Project by ID
            if (projectToView != null)
            {
                Console.WriteLine($"ToDos for project '{projectToView.Name}':");
                if (projectToView.ToDos.Count == 0)
                {
                    Console.WriteLine("No ToDos assigned to this project.");
                }
                else
                {
                    // display ToDos for the project
                    foreach (var t in projectToView.ToDos)
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
    }
}