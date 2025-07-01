using Asana.Library.Models;
using Asana.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Asana.Maui.ViewModels
{
    public class ToDoDetailViewModel : INotifyPropertyChanged
    {
        private readonly ToDoServiceProxy _toDoSvc;

        // REQUIRED: Properties for Entry controls
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

        // Priority button colors (simple visual feedback)
        public Color Priority1Color => Priority == 1 ? Colors.Blue : Colors.LightGray;
        public Color Priority2Color => Priority == 2 ? Colors.Blue : Colors.LightGray;
        public Color Priority3Color => Priority == 3 ? Colors.Blue : Colors.LightGray;

        // REQUIRED: DatePicker property
        private DateTime _dueDate = DateTime.Now;
        public DateTime DueDate
        {
            get => _dueDate;
            set => SetProperty(ref _dueDate, value);
        }

        // REQUIRED: Picker for project selection
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

        public ToDoDetailViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;

            // Initialize collections
            AvailableProjects = new ObservableCollection<Project>();

            // Initialize commands
            SaveCommand = new Command(SaveToDo);
            SetPriorityCommand = new Command<string>(SetPriority);

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
            // Basic validation
            if (string.IsNullOrWhiteSpace(Name))
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Task name is required", "OK");
                }
                return;
            }

            // Create new ToDo
            var newToDo = new ToDo
            {
                Id = 0, // Service will assign ID
                Name = Name,
                Description = Description,
                Priority = Priority,
                DueDate = DueDate,
                IsComplete = false
            };

            // Save the ToDo
            var savedToDo = _toDoSvc.AddOrUpdateToDo(newToDo);

            // Assign to project if selected (and not "No Project")
            if (savedToDo != null && SelectedProject != null && SelectedProject.Id > 0)
            {
                _toDoSvc.AssignToDoToProject(savedToDo.Id, SelectedProject.Id);
            }

            // Clear form
            ClearForm();

            // Show success message
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Task created successfully!", "OK");
            }
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Description = string.Empty;
            Priority = 1;
            DueDate = DateTime.Now;
            SelectedProject = AvailableProjects.FirstOrDefault(); // Reset to "No Project"
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