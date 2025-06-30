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

        // Properties for new ToDo creation
        private string _newToDoName = string.Empty;
        public string NewToDoName
        {
            get => _newToDoName;
            set
            {
                SetProperty(ref _newToDoName, value);
                ((Command)AddToDoCommand).ChangeCanExecute();
            }
        }

        private string _newToDoDescription = string.Empty;
        public string NewToDoDescription
        {
            get => _newToDoDescription;
            set => SetProperty(ref _newToDoDescription, value);
        }

        private int _newToDoPriority = 1;
        public int NewToDoPriority
        {
            get => _newToDoPriority;
            set => SetProperty(ref _newToDoPriority, value);
        }

        private DateTime _newToDoDueDate = DateTime.Now.AddDays(7);
        public DateTime NewToDoDueDate
        {
            get => _newToDoDueDate;
            set => SetProperty(ref _newToDoDueDate, value);
        }

        // Properties for new Project creation
        private string _newProjectName = string.Empty;
        public string NewProjectName
        {
            get => _newProjectName;
            set
            {
                SetProperty(ref _newProjectName, value);
                ((Command)AddProjectCommand).ChangeCanExecute();
            }
        }

        private string _newProjectDescription = string.Empty;
        public string NewProjectDescription
        {
            get => _newProjectDescription;
            set => SetProperty(ref _newProjectDescription, value);
        }

        // Selected items
        private ToDo? _selectedToDo;
        public ToDo? SelectedToDo
        {
            get => _selectedToDo;
            set => SetProperty(ref _selectedToDo, value);
        }

        private Project? _selectedProject;
        public Project? SelectedProject
        {
            get => _selectedProject;
            set => SetProperty(ref _selectedProject, value);
        }

        private Project? _selectedProjectForNewToDo;
        public Project? SelectedProjectForNewToDo
        {
            get => _selectedProjectForNewToDo;
            set => SetProperty(ref _selectedProjectForNewToDo, value);
        }

        // For Picker - Priority options
        public List<int> PriorityOptions { get; } = new List<int> { 1, 2, 3 };

        // Commands
        public ICommand AddToDoCommand { get; }
        public ICommand DeleteToDoCommand { get; }
        public ICommand ToggleToDoCompleteCommand { get; }
        public ICommand AddProjectCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand RefreshCommand { get; }

        public MainPageViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;

            // Initialize collections
            ToDos = new ObservableCollection<ToDo>();
            Projects = new ObservableCollection<Project>();

            // Initialize commands
            AddToDoCommand = new Command(AddToDo, CanAddToDo);
            DeleteToDoCommand = new Command<ToDo>(DeleteToDo);
            ToggleToDoCompleteCommand = new Command<ToDo>(ToggleToDoComplete);
            AddProjectCommand = new Command(AddProject, CanAddProject);
            DeleteProjectCommand = new Command<Project>(DeleteProject);
            RefreshCommand = new Command(RefreshData);

            // Load initial data
            RefreshData();
        }

        private bool CanAddToDo()
        {
            return !string.IsNullOrWhiteSpace(NewToDoName);
        }

        private void AddToDo()
        {
            try
            {
                if (!CanAddToDo()) return;

                var newToDo = new ToDo
                {
                    Id = 0, // Will be set by service
                    Name = NewToDoName,
                    Description = NewToDoDescription ?? string.Empty,
                    Priority = NewToDoPriority,
                    DueDate = NewToDoDueDate,
                    IsComplete = false,
                    ProjectId = null
                };

                var createdToDo = _toDoSvc.AddOrUpdateToDo(newToDo);
                if (createdToDo != null)
                {
                    // Assign to project if one is selected
                    if (SelectedProjectForNewToDo != null)
                    {
                        _toDoSvc.AssignToDoToProject(createdToDo.Id, SelectedProjectForNewToDo.Id);
                    }

                    // Clear input fields
                    NewToDoName = string.Empty;
                    NewToDoDescription = string.Empty;
                    NewToDoPriority = 1;
                    NewToDoDueDate = DateTime.Now.AddDays(7);
                    SelectedProjectForNewToDo = null;

                    // Refresh data to show new ToDo
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                // This will help us see what's causing the crash
                System.Diagnostics.Debug.WriteLine($"Error adding ToDo: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw to see the error
            }
        }

        private void DeleteToDo(ToDo? toDo)
        {
            if (toDo == null) return;

            _toDoSvc.DeleteToDo(toDo);
            RefreshData();
        }

        private void ToggleToDoComplete(ToDo? toDo)
        {
            if (toDo == null) return;

            toDo.IsComplete = !toDo.IsComplete;
            _toDoSvc.AddOrUpdateToDo(toDo);
            RefreshData();
        }

        private bool CanAddProject()
        {
            return !string.IsNullOrWhiteSpace(NewProjectName);
        }

        private void AddProject()
        {
            if (!CanAddProject()) return;

            var newProject = new Project
            {
                Id = 0, // Will be set by service
                Name = NewProjectName,
                Description = NewProjectDescription,
                CompletePercent = 0
            };

            var createdProject = _toDoSvc.AddOrUpdateProject(newProject);
            if (createdProject != null)
            {
                // Clear input fields
                NewProjectName = string.Empty;
                NewProjectDescription = string.Empty;

                // Refresh data to show new Project
                RefreshData();
            }
        }

        private void DeleteProject(Project? project)
        {
            if (project == null) return;

            _toDoSvc.DeleteProject(project);
            RefreshData();
        }

        public void AssignToDoToProject(ToDo toDo, Project? project)
        {
            if (toDo == null) return;

            int? projectId = project?.Id;
            _toDoSvc.AssignToDoToProject(toDo.Id, projectId);
            RefreshData();
        }

        private void RefreshData()
        {
            // Clear and reload ToDos
            ToDos.Clear();
            foreach (var todo in _toDoSvc.ToDos)
            {
                ToDos.Add(todo);
            }

            // Clear and reload Projects
            Projects.Clear();
            foreach (var project in _toDoSvc.Projects)
            {
                Projects.Add(project);
            }
        }

        // Helper method to get ToDos for a specific project
        public ObservableCollection<ToDo> GetToDosByProject(int projectId)
        {
            var projectToDos = _toDoSvc.GetToDosByProject(projectId);
            return new ObservableCollection<ToDo>(projectToDos);
        }

        // Helper method to get project name for a ToDo
        public string GetProjectNameForToDo(int? projectId)
        {
            if (!projectId.HasValue) return "No Project";
            var project = _toDoSvc.GetProjectById(projectId);
            return project?.Name ?? "Unknown Project";
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