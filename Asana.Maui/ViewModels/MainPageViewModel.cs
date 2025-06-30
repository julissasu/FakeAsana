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
            set => SetProperty(ref _newToDoName, value);
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

        // Properties for new Project creation
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
            if (!CanAddToDo()) return;

            var newToDo = new ToDo
            {
                Id = 0, // Will be set by service
                Name = NewToDoName,
                Description = NewToDoDescription,
                Priority = NewToDoPriority,
                IsComplete = false
            };

            var createdToDo = _toDoSvc.AddOrUpdateToDo(newToDo);
            if (createdToDo != null)
            {
                // Clear input fields
                NewToDoName = string.Empty;
                NewToDoDescription = string.Empty;
                NewToDoPriority = 1;

                // Refresh data to show new ToDo
                RefreshData();
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