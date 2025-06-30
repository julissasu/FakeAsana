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
                            ToDoServiceProxy.CreateToDo(toDos, projects, ref nextToDoId);
                            break;
                        case 2:
                            ToDoServiceProxy.DeleteToDo(toDos, projects);
                            break;
                        case 3:
                            ToDoServiceProxy.UpdateToDo(toDos, projects);
                            break;
                        case 4:
                            ToDoServiceProxy.DisplayAllToDos(toDos);
                            break;
                        case 5:
                            ToDoServiceProxy.CreateProject(projects, ref nextProjectId);
                            break;
                        case 6:
                            ToDoServiceProxy.DeleteProject(projects);
                            break;
                        case 7:
                            ToDoServiceProxy.UpdateProject(projects);
                            break;
                        case 8:
                            ToDoServiceProxy.DisplayAllProjects(projects);
                            break;
                        case 9:
                            ToDoServiceProxy.DisplayToDosByProject(projects);
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
    }
}