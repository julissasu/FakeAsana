using Asana.Library.Models;
using Asana.Library.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Asana.Maui.ViewModels
{
    public class ToDoDetailViewModel : INotifyPropertyChanged
    {
        private ToDoServiceProxy _toDoSvc;

        // Default constructor initializes a new ToDo
        public ToDoDetailViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;
            Model = new ToDo();
            DeleteCommand = new Command(DoDelete);
        }

        // Constructor for loading an existing ToDo by ID
        public ToDoDetailViewModel(int id)
        {
            _toDoSvc = ToDoServiceProxy.Current;
            Model = _toDoSvc.GetToDoById(id) ?? new ToDo();
            DeleteCommand = new Command(DoDelete);
        }

        // Constructor for loading an existing ToDo model directly
        public ToDoDetailViewModel(ToDo? model)
        {
            _toDoSvc = ToDoServiceProxy.Current;
            Model = model ?? new ToDo();
            DeleteCommand = new Command(DoDelete);
        }

        // Command to delete the current ToDo
        public void DoDelete()
        {
            ToDoServiceProxy.Current.DeleteToDo(Model);
        }

        public ToDo? Model { get; set; }    // The underlying ToDo model
        public ICommand DeleteCommand { get; set; } // Delete command for the ToDo

        // Wrapper property for ID to handle updates
        public string ModelId
        {
            get
            {
                return Model?.Id.ToString() ?? "0";
            }
        }

        // Wrapper for IsComplete to trigger immediate updates
        public bool IsComplete
        {
            get
            {
                return Model?.IsComplete ?? false;
            }
            set
            {
                if (Model != null && Model.IsComplete != value)
                {
                    Model.IsComplete = value;
                    _toDoSvc.AddOrUpdateToDo(Model); // Save immediately
                    NotifyPropertyChanged();

                    // Trigger a static event that MainPageViewModel can listen to
                    TaskCompletionChanged?.Invoke();
                }
            }
        }

        // Static event for notifying other ViewModels when a task's completion status changes
        public static event Action? TaskCompletionChanged;

        // Project display
        public string ProjectDisplayName
        {
            get
            {
                if (Model?.ProjectId == null) return string.Empty;
                var project = _toDoSvc.GetProjectById(Model.ProjectId);
                return $"Project: {project?.Name ?? "Unknown"}";
            }
        }

        // Check if the ToDo has an associated project
        public bool HasProject => Model?.ProjectId != null;

        // Priority management (dropdown options)
        public List<int> Priorities
        {
            get
            {
                return new List<int> { 1, 2, 3 };
            }
        }

        // Currently selected priority
        public int SelectedPriority
        {
            get
            {
                return Model == null ? 1 : Model.Priority;
            }
            set
            {
                if (Model != null && Model.Priority != value)
                {
                    Model.Priority = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private List<ProjectViewModel>? _availableProjects;

        // Project selection
        public List<ProjectViewModel> AvailableProjects
        {
            get
            {
                if (_availableProjects == null)
                {
                    _availableProjects = new List<ProjectViewModel>
                    {
                        new ProjectViewModel { Model = null } // No Project option
                    };
                    _availableProjects.AddRange(_toDoSvc.Projects.Select(p => new ProjectViewModel { Model = p }));
                }
                return _availableProjects;
            }
        }

        // Currently selected project
        public ProjectViewModel? SelectedProject
        {
            get
            {
                if (Model?.ProjectId == null)
                {
                    // Return the "No Project" option
                    return AvailableProjects.FirstOrDefault(p => p.Model == null);
                }

                // Match selected project by ID
                return AvailableProjects.FirstOrDefault(p => p.Model?.Id == Model.ProjectId);
            }
            set
            {
                if (Model != null)
                {
                    Model.ProjectId = value?.Model?.Id;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged(nameof(ProjectDisplayName));
                    NotifyPropertyChanged(nameof(HasProject));
                }
            }
        }

        // Save or update the ToDo
        public void AddOrUpdateToDo()
        {
            _toDoSvc.AddOrUpdateToDo(Model);

            // Notify that the ID might have changed
            NotifyPropertyChanged(nameof(ModelId));
        }

        // Priority display for Entry binding
        public string PriorityDisplay
        {
            get
            {
                return Model == null ? string.Empty : Model.Priority.ToString();
            }
            set
            {
                if (Model == null) return;

                if (!int.TryParse(value, out int p))
                {
                    Model.Priority = 1; // Default to 1 if invalid
                }
                else
                {
                    Model.Priority = Math.Max(1, Math.Min(3, p)); // Clamp between 1 and 3
                }
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        // Notify property change for data binding
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}