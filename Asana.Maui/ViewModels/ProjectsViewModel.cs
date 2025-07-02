using Asana.Library.Models;
using Asana.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Asana.Maui.ViewModels
{
    public class ProjectsViewModel : INotifyPropertyChanged
    {
        private readonly ToDoServiceProxy _toDoSvc;

        // ListView data source
        public ObservableCollection<Project> Projects { get; set; }

        // Commands
        public ICommand RefreshCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand EditProjectCommand { get; }
        public ICommand ViewProjectTasksCommand { get; }

        public ProjectsViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;

            // Initialize collections
            Projects = new ObservableCollection<Project>();

            // Initialize commands
            RefreshCommand = new Command(RefreshProjects);
            DeleteProjectCommand = new Command<Project>(DeleteProject);
            EditProjectCommand = new Command<Project>(EditProject);
            ViewProjectTasksCommand = new Command<Project>(ViewProjectTasks);

            // Load initial data
            RefreshProjects();
        }

        public void RefreshProjects()
        {
            Projects.Clear();

            foreach (var project in _toDoSvc.Projects)
            {
                // Calculate completion percentage
                var totalTasks = project.ToDos.Count;
                var completedTasks = project.ToDos.Count(t => t.IsComplete);

                if (totalTasks > 0)
                {
                    project.CompletePercent = (int)((double)completedTasks / totalTasks * 100);
                }
                else
                {
                    project.CompletePercent = 0;
                }

                Projects.Add(project);
            }
        }

        private async void DeleteProject(Project? project)
        {
            if (project == null) return;

            if (Application.Current?.MainPage != null)
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Delete",
                    $"Are you sure you want to delete '{project.Name}'?\n" +
                    $"This will unassign all tasks from this project.",
                    "Yes", "No");

                if (confirm)
                {
                    _toDoSvc.DeleteProject(project);
                    RefreshProjects();
                }
            }
        }

        private async void EditProject(Project? project)
        {
            if (project == null) return;

            if (Application.Current?.MainPage != null)
            {
                // Simple edit using prompts
                var newName = await Application.Current.MainPage.DisplayPromptAsync(
                    "Edit Project",
                    "Project Name:",
                    initialValue: project.Name,
                    maxLength: 50);

                if (!string.IsNullOrWhiteSpace(newName))
                {
                    var newDescription = await Application.Current.MainPage.DisplayPromptAsync(
                        "Edit Project",
                        "Project Description:",
                        initialValue: project.Description ?? "",
                        maxLength: 200);

                    project.Name = newName;
                    project.Description = newDescription;
                    _toDoSvc.AddOrUpdateProject(project);
                    RefreshProjects();
                }
            }
        }

        private async void ViewProjectTasks(Project? project)
        {
            if (project == null) return;

            var tasks = _toDoSvc.GetToDosByProject(project.Id);
            var taskList = "Tasks in this project:\n\n";

            if (tasks.Any())
            {
                foreach (var task in tasks)
                {
                    var status = task.IsComplete ? "✓" : "○";
                    var priority = $"P{task.Priority}";
                    taskList += $"{status} {task.Name} [{priority}] - Due: {task.DueDate:MMM dd}\n";
                }
            }
            else
            {
                taskList = "No tasks assigned to this project.";
            }

            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    $"{project.Name}",
                    taskList,
                    "OK");
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}