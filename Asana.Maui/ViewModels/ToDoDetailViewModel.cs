using Asana.Library.Models;
using Asana.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Asana.Maui.ViewModels
{
    [QueryProperty(nameof(ToDoId), "todoId")]
    public class ToDoDetailViewModel : INotifyPropertyChanged
    {
        private readonly ToDoServiceProxy _toDoSvc;
        private int _todoId = 0;
        private bool _isEditMode = false;

        // Query parameter for editing
        public int ToDoId
        {
            get => _todoId;
            set
            {
                _todoId = value;
                if (_todoId > 0)
                {
                    LoadToDoForEdit(_todoId);
                }
            }
        }

        // Page title
        private string _pageTitle = "Create Task";
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        // Entry properties
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        // Priority selection
        private int _priority = 1;
        public int Priority
        {
            get => _priority;
            set
            {
                SetProperty(ref _priority, value);
                OnPropertyChanged(nameof(Priority1Color));
                OnPropertyChanged(nameof(Priority2Color));
                OnPropertyChanged(nameof(Priority3Color));
            }
        }

        // Priority button colors
        public Color Priority1Color => Priority == 1 ? Color.FromArgb("#007AFF") : Color.FromArgb("#E0E0E0");
        public Color Priority2Color => Priority == 2 ? Color.FromArgb("#FF9500") : Color.FromArgb("#E0E0E0");
        public Color Priority3Color => Priority == 3 ? Color.FromArgb("#FF3B30") : Color.FromArgb("#E0E0E0");

        // DatePicker property
        private DateTime _dueDate = DateTime.Now;
        public DateTime DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        // Picker for project selection
        public ObservableCollection<Project> AvailableProjects { get; set; }

        private Project? _selectedProject;
        public Project? SelectedProject
        {
            get => _selectedProject;
            set => SetProperty(ref _selectedProject, value);
        }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand SetPriorityCommand { get; }
        public ICommand CancelCommand { get; }

        public ToDoDetailViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;

            // Initialize collections
            AvailableProjects = new ObservableCollection<Project>();

            // Initialize commands
            SaveCommand = new Command(SaveToDo);
            SetPriorityCommand = new Command<string>(SetPriority);
            CancelCommand = new Command(Cancel);

            // Load projects for picker
            LoadProjects();
        }

        private void LoadProjects()
        {
            AvailableProjects.Clear();

            // Add "No Project" option
            AvailableProjects.Add(new Project { Id = 0, Name = "No Project" });

            // Add existing projects
            foreach (var project in _toDoSvc.Projects)
            {
                AvailableProjects.Add(project);
            }

            // Set default selection
            SelectedProject = AvailableProjects.FirstOrDefault();
        }

        private void LoadToDoForEdit(int todoId)
        {
            var todo = _toDoSvc.GetToDoById(todoId);
            if (todo != null)
            {
                _isEditMode = true;
                PageTitle = "Edit Task";

                Name = todo.Name ?? string.Empty;
                Description = todo.Description ?? string.Empty;
                Priority = todo.Priority;
                DueDate = todo.DueDate;

                // Set selected project
                if (todo.ProjectId.HasValue)
                {
                    SelectedProject = AvailableProjects.FirstOrDefault(p => p.Id == todo.ProjectId.Value);
                }
                else
                {
                    SelectedProject = AvailableProjects.FirstOrDefault(); // "No Project"
                }
            }
        }

        private void SetPriority(string priorityStr)
        {
            if (int.TryParse(priorityStr, out int priority))
            {
                Priority = priority;
            }
        }

        private async void SaveToDo()
        {
            // Validation
            if (string.IsNullOrWhiteSpace(Name))
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Task name is required", "OK");
                }
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Saving ToDo: {Name}");

            if (_isEditMode)
            {
                // Update existing ToDo
                var existingToDo = _toDoSvc.GetToDoById(_todoId);
                if (existingToDo != null)
                {
                    existingToDo.Name = Name;
                    existingToDo.Description = Description;
                    existingToDo.Priority = Priority;
                    existingToDo.DueDate = DueDate;

                    _toDoSvc.AddOrUpdateToDo(existingToDo);

                    // Update project assignment
                    var newProjectId = SelectedProject?.Id > 0 ? SelectedProject.Id : (int?)null;
                    _toDoSvc.AssignToDoToProject(_todoId, newProjectId);

                    System.Diagnostics.Debug.WriteLine($"Updated ToDo ID: {_todoId}");
                }
            }
            else
            {
                // Create new ToDo
                var newToDo = new ToDo
                {
                    Id = 0,
                    Name = Name,
                    Description = Description,
                    Priority = Priority,
                    DueDate = DueDate,
                    IsComplete = false
                };

                var savedToDo = _toDoSvc.AddOrUpdateToDo(newToDo);

                if (savedToDo != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Created new ToDo with ID: {savedToDo.Id}");

                    // Assign to project if selected
                    if (SelectedProject != null && SelectedProject.Id > 0)
                    {
                        _toDoSvc.AssignToDoToProject(savedToDo.Id, SelectedProject.Id);
                        System.Diagnostics.Debug.WriteLine($"Assigned to Project ID: {SelectedProject.Id}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to create ToDo!");
                }
            }

            // Navigate back
            await Shell.Current.GoToAsync("..");
        }

        private async void Cancel()
        {
            await Shell.Current.GoToAsync("..");
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