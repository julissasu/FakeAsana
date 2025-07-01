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
            set
            {
                SetProperty(ref _newToDoPriority, value);
                // Update button colors when priority changes
                OnPropertyChanged(nameof(Priority1Color));
                OnPropertyChanged(nameof(Priority2Color));
                OnPropertyChanged(nameof(Priority3Color));
            }
        }

        // FIXED: Priority button colors
        public Color Priority1Color => NewToDoPriority == 1 ? Colors.DarkGray : Colors.LightGray;
        public Color Priority2Color => NewToDoPriority == 2 ? Colors.DarkGray : Colors.LightGray;
        public Color Priority3Color => NewToDoPriority == 3 ? Colors.DarkGray : Colors.LightGray;

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
            set
            {
                SetProperty(ref _selectedProjectForNewToDo, value);
                OnPropertyChanged(nameof(SelectedProjectDisplay)); // Update button text
            }
        }

        // Display property for the current project selection
        public string SelectedProjectDisplay
        {
            get
            {
                if (SelectedProjectForNewToDo == null)
                    return "Select Project (Optional)";
                return SelectedProjectForNewToDo.Name ?? "Unnamed Project";
            }
        }

        // For Picker - Priority options (keeping this in case you want to switch back)
        public List<int> PriorityOptions { get; } = new List<int> { 1, 2, 3 };

        // Commands
        public ICommand AddToDoCommand { get; }
        public ICommand DeleteToDoCommand { get; }
        public ICommand ToggleToDoCompleteCommand { get; }
        public ICommand AddProjectCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SetPriorityCommand { get; }
        public ICommand ShowProjectSelectorCommand { get; }
        public ICommand EditToDoCommand { get; }

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
            SetPriorityCommand = new Command<string>(SetPriority);
            ShowProjectSelectorCommand = new Command(async () => await ShowProjectSelector());
            EditToDoCommand = new Command<ToDo>(async (todo) => await EditToDo(todo)); // FIXED: Initialize edit command

            // Load initial data
            RefreshData();
        }

        // Priority button method
        private void SetPriority(string priorityStr)
        {
            if (int.TryParse(priorityStr, out int priority))
            {
                NewToDoPriority = priority;
            }
        }

        // NEW: Edit ToDo method
        private async Task EditToDo(ToDo? todo)
        {
            if (todo == null) return;

            try
            {
                var options = new List<string>
                {
                    "Change Project",
                    "Edit Name",
                    "Edit Description",
                    "Change Priority",
                    "Change Due Date"
                };

                if (Application.Current?.MainPage != null)
                {
                    var action = await Application.Current.MainPage.DisplayActionSheet(
                        $"Edit: {todo.Name}",
                        "Cancel",
                        null,
                        options.ToArray());

                    switch (action)
                    {
                        case "Change Project":
                            await EditToDoProject(todo);
                            break;
                        case "Edit Name":
                            await EditToDoName(todo);
                            break;
                        case "Edit Description":
                            await EditToDoDescription(todo);
                            break;
                        case "Change Priority":
                            await EditToDoPriority(todo);
                            break;
                        case "Change Due Date":
                            await EditToDoDueDate(todo);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error editing ToDo: {ex.Message}");
            }
        }

        // NEW: Project editing (reuses your existing project selector logic)
        private async Task EditToDoProject(ToDo todo)
        {
            var projectNames = new List<string> { "No Project" };

            foreach (var project in Projects)
            {
                projectNames.Add(project.Name ?? "Unnamed Project");
            }

            if (Application.Current?.MainPage != null)
            {
                var selectedProjectName = await Application.Current.MainPage.DisplayActionSheet(
                    "Select Project",
                    "Cancel",
                    null,
                    projectNames.ToArray());

                if (selectedProjectName != null && selectedProjectName != "Cancel")
                {
                    if (selectedProjectName == "No Project")
                    {
                        _toDoSvc.AssignToDoToProject(todo.Id, null);
                    }
                    else
                    {
                        var selectedProject = Projects.FirstOrDefault(p => p.Name == selectedProjectName);
                        if (selectedProject != null)
                        {
                            _toDoSvc.AssignToDoToProject(todo.Id, selectedProject.Id);
                        }
                    }
                    RefreshData();
                }
            }
        }

        // NEW: Simple text editing
        private async Task EditToDoName(ToDo todo)
        {
            if (Application.Current?.MainPage != null)
            {
                var result = await Application.Current.MainPage.DisplayPromptAsync(
                    "Edit Name",
                    "Enter new name:",
                    initialValue: todo.Name);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    todo.Name = result;
                    _toDoSvc.AddOrUpdateToDo(todo);
                    RefreshData();
                }
            }
        }

        private async Task EditToDoDescription(ToDo todo)
        {
            if (Application.Current?.MainPage != null)
            {
                var result = await Application.Current.MainPage.DisplayPromptAsync(
                    "Edit Description",
                    "Enter new description:",
                    initialValue: todo.Description);

                if (result != null) // Allow empty descriptions
                {
                    todo.Description = result;
                    _toDoSvc.AddOrUpdateToDo(todo);
                    RefreshData();
                }
            }
        }

        // NEW: Priority editing
        private async Task EditToDoPriority(ToDo todo)
        {
            if (Application.Current?.MainPage != null)
            {
                var selectedPriority = await Application.Current.MainPage.DisplayActionSheet(
                    "Select Priority",
                    "Cancel",
                    null,
                    "Priority 1", "Priority 2", "Priority 3");

                if (selectedPriority != null && selectedPriority != "Cancel")
                {
                    var priorityNumber = selectedPriority.Replace("Priority ", "");
                    if (int.TryParse(priorityNumber, out int priority))
                    {
                        todo.Priority = priority;
                        _toDoSvc.AddOrUpdateToDo(todo);
                        RefreshData();
                    }
                }
            }
        }

        // NEW: Due date editing  
        private async Task EditToDoDueDate(ToDo todo)
        {
            if (Application.Current?.MainPage != null)
            {
                var result = await Application.Current.MainPage.DisplayPromptAsync(
                    "Edit Due Date",
                    "Enter new due date (yyyy-mm-dd):",
                    initialValue: todo.DueDate.ToString("yyyy-MM-dd"));

                if (!string.IsNullOrWhiteSpace(result) && DateTime.TryParse(result, out DateTime newDate))
                {
                    todo.DueDate = newDate;
                    _toDoSvc.AddOrUpdateToDo(todo);
                    RefreshData();
                }
            }
        }

        // Project selector method
        private async Task ShowProjectSelector()
        {
            try
            {
                var projectNames = new List<string> { "No Project" }; // Always include "No Project"

                // Add existing projects
                foreach (var project in Projects)
                {
                    projectNames.Add(project.Name ?? "Unnamed Project");
                }

                if (projectNames.Count == 1) // Only "No Project" exists
                {
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("No Projects",
                            "No projects available. Create a project first.", "OK");
                    }
                    return;
                }

                // Show action sheet
                if (Application.Current?.MainPage != null)
                {
                    var selectedProjectName = await Application.Current.MainPage.DisplayActionSheet(
                        "Select Project",
                        "Cancel",
                        null,
                        projectNames.ToArray());

                    if (selectedProjectName != null && selectedProjectName != "Cancel")
                    {
                        if (selectedProjectName == "No Project")
                        {
                            SelectedProjectForNewToDo = null;
                        }
                        else
                        {
                            // Find the selected project
                            SelectedProjectForNewToDo = Projects.FirstOrDefault(p => p.Name == selectedProjectName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing project selector: {ex.Message}");
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error",
                        "Failed to show project selector. Please try again.", "OK");
                }
            }
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
                    NewToDoPriority = 1; // This will also update the button colors
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
                // Set the project display name
                if (todo.ProjectId.HasValue)
                {
                    var project = _toDoSvc.GetProjectById(todo.ProjectId);
                    todo.ProjectDisplayName = $"Project: {project?.Name ?? "Unknown"}";
                }
                else
                {
                    todo.ProjectDisplayName = "";
                }

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
            if (!projectId.HasValue) return "";
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