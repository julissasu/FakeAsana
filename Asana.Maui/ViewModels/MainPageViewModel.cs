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

        // Collections for REQUIRED ListView controls
        public ObservableCollection<ToDo> ToDos { get; set; }
        public ObservableCollection<Project> Projects { get; set; }

        // Task creation properties
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
            set
            {
                SetProperty(ref _newToDoPriority, value);
                OnPropertyChanged(nameof(Priority1Color));
                OnPropertyChanged(nameof(Priority2Color));
                OnPropertyChanged(nameof(Priority3Color));
            }
        }

        // Priority colors
        public Color Priority1Color => NewToDoPriority == 1 ? Colors.Blue : Colors.LightGray;
        public Color Priority2Color => NewToDoPriority == 2 ? Colors.Blue : Colors.LightGray;
        public Color Priority3Color => NewToDoPriority == 3 ? Colors.Blue : Colors.LightGray;

        private DateTime _newToDoDueDate = DateTime.Now;
        public DateTime NewToDoDueDate
        {
            get => _newToDoDueDate;
            set => SetProperty(ref _newToDoDueDate, value);
        }

        // ORIGINAL APPROACH: Simple project selection
        private Project? _selectedProjectForNewToDo;
        public Project? SelectedProjectForNewToDo
        {
            get => _selectedProjectForNewToDo;
            set => SetProperty(ref _selectedProjectForNewToDo, value);
        }

        // Project creation properties
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

        // SIMPLE BUTTON APPROACH: Display selected project name
        public string SelectedProjectDisplay
        {
            get
            {
                if (SelectedProjectForNewToDo == null)
                    return "Select Project (Optional)";
                return SelectedProjectForNewToDo.Name ?? "Unnamed Project";
            }
        }

        // Commands
        public ICommand AddToDoCommand { get; }
        public ICommand AddProjectCommand { get; }
        public ICommand DeleteToDoCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand SetPriorityCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ShowProjectSelectorCommand { get; }

        public MainPageViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;

            // Initialize collections
            ToDos = new ObservableCollection<ToDo>();
            Projects = new ObservableCollection<Project>();

            // Initialize commands
            AddToDoCommand = new Command(AddToDo);
            AddProjectCommand = new Command(AddProject);
            DeleteToDoCommand = new Command<ToDo>(DeleteToDo);
            DeleteProjectCommand = new Command<Project>(DeleteProject);
            SetPriorityCommand = new Command<string>(SetPriority);
            RefreshCommand = new Command(RefreshData);
            ShowProjectSelectorCommand = new Command(ShowProjectSelector);

            // Load initial data
            RefreshData();
        }

        private void SetPriority(string priorityStr)
        {
            if (int.TryParse(priorityStr, out int priority))
            {
                NewToDoPriority = priority;
            }
        }

        // ORIGINAL WORKING LOGIC: Simple add todo
        private void AddToDo()
        {
            if (string.IsNullOrWhiteSpace(NewToDoName)) return;

            var newToDo = new ToDo
            {
                Id = 0,
                Name = NewToDoName,
                Description = NewToDoDescription,
                Priority = NewToDoPriority,
                DueDate = NewToDoDueDate,
                IsComplete = false
            };

            var createdToDo = _toDoSvc.AddOrUpdateToDo(newToDo);

            // ORIGINAL LOGIC: Assign to project if one is selected
            if (createdToDo != null && SelectedProjectForNewToDo != null)
            {
                _toDoSvc.AssignToDoToProject(createdToDo.Id, SelectedProjectForNewToDo.Id);
            }

            // Clear form and refresh
            ClearTaskForm();
            RefreshData();
        }

        private void AddProject()
        {
            if (string.IsNullOrWhiteSpace(NewProjectName)) return;

            var newProject = new Project
            {
                Id = 0,
                Name = NewProjectName,
                Description = NewProjectDescription,
                CompletePercent = 0
            };

            _toDoSvc.AddOrUpdateProject(newProject);
            ClearProjectForm();
            RefreshData();
        }

        private void DeleteToDo(ToDo? toDo)
        {
            if (toDo == null) return;
            _toDoSvc.DeleteToDo(toDo);
            RefreshData();
        }

        private void DeleteProject(Project? project)
        {
            if (project == null) return;
            _toDoSvc.DeleteProject(project);
            RefreshData();
        }

        private void ClearTaskForm()
        {
            NewToDoName = string.Empty;
            NewToDoDescription = string.Empty;
            NewToDoPriority = 1;
            NewToDoDueDate = DateTime.Now;
            SelectedProjectForNewToDo = null; // Clear selection
            OnPropertyChanged(nameof(SelectedProjectDisplay));
        }

        // SIMPLE PROJECT SELECTOR using DisplayActionSheet
        private async void ShowProjectSelector()
        {
            if (Projects.Count == 0)
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("No Projects", "Create a project first.", "OK");
                }
                return;
            }

            var projectNames = new List<string> { "No Project" };
            projectNames.AddRange(Projects.Select(p => p.Name ?? "Unnamed Project"));

            if (Application.Current?.MainPage != null)
            {
                var selectedProjectName = await Application.Current.MainPage.DisplayActionSheet(
                    "Select Project", "Cancel", null, projectNames.ToArray());

                if (selectedProjectName != null && selectedProjectName != "Cancel")
                {
                    if (selectedProjectName == "No Project")
                    {
                        SelectedProjectForNewToDo = null;
                    }
                    else
                    {
                        SelectedProjectForNewToDo = Projects.FirstOrDefault(p => p.Name == selectedProjectName);
                    }
                    OnPropertyChanged(nameof(SelectedProjectDisplay));
                }
            }
        }

        private void ClearProjectForm()
        {
            NewProjectName = string.Empty;
            NewProjectDescription = string.Empty;
        }

        // ORIGINAL APPROACH: Simple refresh
        public void RefreshData()
        {
            // Update ToDos
            ToDos.Clear();
            foreach (var todo in _toDoSvc.ToDos)
            {
                if (todo.ProjectId.HasValue)
                {
                    var project = _toDoSvc.GetProjectById(todo.ProjectId);
                    todo.ProjectDisplayName = $"Project: {project?.Name}";
                }
                else
                {
                    todo.ProjectDisplayName = "";
                }
                ToDos.Add(todo);
            }

            // Update Projects - simple approach
            Projects.Clear();
            foreach (var project in _toDoSvc.Projects)
            {
                Projects.Add(project);
            }

            // Update project display
            OnPropertyChanged(nameof(SelectedProjectDisplay));
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