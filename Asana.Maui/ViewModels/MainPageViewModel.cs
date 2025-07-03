using Asana.Library.Models;
using Asana.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Asana.Maui.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly ToDoServiceProxy _toDoSvc;

        // Observable collections for data binding
        public ObservableCollection<ToDo> ToDos { get; set; }
        public ObservableCollection<Project> Projects { get; set; }

        // Show/Hide completed tasks
        private bool _showCompletedTasks = true;
        public bool ShowCompletedTasks
        {
            get => _showCompletedTasks;
            set
            {
                SetProperty(ref _showCompletedTasks, value);
                RefreshData();
            }
        }

        // Properties for new Project creation (keep it simple in main page)
        private string _newProjectName = string.Empty;
        public string NewProjectName
        {
            get => _newProjectName;
            set => SetProperty(ref _newProjectName, value);
        }

        private string _newProjectDescription = string.Empty;
        public string NewProjectDescription
        {
            get => _newProjectDescription;
            set => SetProperty(ref _newProjectDescription, value);
        }

        // Commands
        public ICommand RefreshCommand { get; }
        public ICommand DeleteToDoCommand { get; }
        public ICommand ToggleToDoCompleteCommand { get; }
        public ICommand NavigateToCreateToDoCommand { get; }
        public ICommand NavigateToEditToDoCommand { get; }
        public ICommand NavigateToProjectsCommand { get; }
        public ICommand AddProjectCommand { get; }

        public MainPageViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;

            // Initialize collections
            ToDos = new ObservableCollection<ToDo>();
            Projects = new ObservableCollection<Project>();

            // Initialize commands
            RefreshCommand = new Command(RefreshData);
            DeleteToDoCommand = new Command<ToDo>(DeleteToDo);
            ToggleToDoCompleteCommand = new Command<ToDo>(ToggleToDoComplete);
            NavigateToCreateToDoCommand = new Command(NavigateToCreateToDo);
            NavigateToEditToDoCommand = new Command<ToDo>(NavigateToEditToDo);
            NavigateToProjectsCommand = new Command(NavigateToProjects);
            AddProjectCommand = new Command(AddProject);

            // Load initial data
            RefreshData();
        }

        public void RefreshData()
        {
            // Update ToDos based on ShowCompletedTasks setting
            ToDos.Clear();
            
            var todosToShow = _showCompletedTasks 
                ? _toDoSvc.ToDos 
                : _toDoSvc.ToDos.Where(t => !t.IsComplete);

            foreach (var todo in todosToShow)
            {
                // Set project display name
                if (todo.ProjectId.HasValue)
                {
                    var project = _toDoSvc.GetProjectById(todo.ProjectId);
                    todo.ProjectDisplayName = project?.Name ?? "Unknown Project";
                }
                else
                {
                    todo.ProjectDisplayName = "";
                }
                ToDos.Add(todo);
            }

            // Update Projects
            Projects.Clear();
            foreach (var project in _toDoSvc.Projects)
            {
                Projects.Add(project);
            }
        }

        private async void DeleteToDo(ToDo? toDo)
        {
            if (toDo == null) return;

            if (Application.Current?.MainPage != null)
            {
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirm Delete",
                    $"Are you sure you want to delete '{toDo.Name}'?",
                    "Yes", "No");

                if (confirm)
                {
                    _toDoSvc.DeleteToDo(toDo);
                    RefreshData();
                }
            }
        }

        private void ToggleToDoComplete(ToDo? toDo)
        {
            if (toDo == null) return;
            
            toDo.IsComplete = !toDo.IsComplete;
            _toDoSvc.AddOrUpdateToDo(toDo);
            
            // If we're hiding completed tasks and this task is now complete, refresh
            if (!ShowCompletedTasks && toDo.IsComplete)
            {
                RefreshData();
            }
        }

        // Simple project creation right in main page
        private async void AddProject()
        {
            if (string.IsNullOrWhiteSpace(NewProjectName))
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Project name is required", "OK");
                }
                return;
            }

            var newProject = new Project
            {
                Id = 0,
                Name = NewProjectName,
                Description = NewProjectDescription,
                CompletePercent = 0
            };

            _toDoSvc.AddOrUpdateProject(newProject);

            // Clear form
            NewProjectName = string.Empty;
            NewProjectDescription = string.Empty;
            
            RefreshData();
        }

        // Navigation methods
        private async void NavigateToCreateToDo()
        {
            await Shell.Current.GoToAsync("ToDoDetailView");
        }

        private async void NavigateToEditToDo(ToDo? toDo)
        {
            if (toDo == null) return;
            await Shell.Current.GoToAsync($"ToDoDetailView?todoId={toDo.Id}");
        }

        private async void NavigateToProjects()
        {
            await Shell.Current.GoToAsync("ProjectsView");
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