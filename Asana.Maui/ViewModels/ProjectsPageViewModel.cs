using Asana.Library.Models;
using Asana.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Asana.Maui.ViewModels
{
    public class ProjectsPageViewModel : INotifyPropertyChanged
    {
        private readonly ToDoServiceProxy _toDoSvc;

        // REQUIRED: ListView data source
        public ObservableCollection<Project> Projects { get; set; }

        // Selected project for operations
        private Project? _selectedProject;
        public Project? SelectedProject
        {
            get => _selectedProject;
            set => SetProperty(ref _selectedProject, value);
        }

        // Commands
        public ICommand RefreshCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand ViewProjectDetailsCommand { get; }

        public ProjectsPageViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;

            // Initialize collections
            Projects = new ObservableCollection<Project>();

            // Initialize commands
            RefreshCommand = new Command(RefreshProjects);
            DeleteProjectCommand = new Command<Project>(DeleteProject);
            ViewProjectDetailsCommand = new Command<Project>(ViewProjectDetails);

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

            // Confirm deletion
            if (Application.Current?.MainPage != null)
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Delete",
                    $"Are you sure you want to delete '{project.Name}'?",
                    "Yes", "No");

                if (confirm)
                {
                    _toDoSvc.DeleteProject(project);
                    RefreshProjects();
                }
            }
        }

        private async void ViewProjectDetails(Project? project)
        {
            if (project == null) return;

            var tasks = _toDoSvc.GetToDosByProject(project.Id);
            var taskCount = tasks.Count;
            var completedCount = tasks.Count(t => t.IsComplete);

            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    project.Name ?? "Project Details",
                    $"Description: {project.Description ?? "No description"}\n" +
                    $"Total Tasks: {taskCount}\n" +
                    $"Completed: {completedCount}\n" +
                    $"Progress: {project.CompletePercent}%",
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