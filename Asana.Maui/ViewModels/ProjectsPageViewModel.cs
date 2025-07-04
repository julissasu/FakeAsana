using Asana.Library.Models;
using Asana.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Asana.Maui.ViewModels
{
    public class ProjectsPageViewModel : INotifyPropertyChanged
    {
        private ToDoServiceProxy _toDoSvc;
        private bool _showAddForm = false;

        public ProjectsPageViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;

            // Listen for ToDo completion changes to update project summaries
            ToDoDetailViewModel.TaskCompletionChanged += RefreshPage;
        }

        // Controls whether the add project form is shown
        public bool ShowAddForm
        {
            get => _showAddForm;
            set
            {
                if (_showAddForm != value)
                {
                    _showAddForm = value;
                    NotifyPropertyChanged();
                }
            }
        }

        // List of all projects displayed on the page
        public ObservableCollection<ProjectViewModel> Projects
        {
            get
            {
                var projects = _toDoSvc.Projects.Select(p => new ProjectViewModel { Model = p });
                return new ObservableCollection<ProjectViewModel>(projects);
            }
        }

        // Currently selected project
        public ProjectViewModel? SelectedProject { get; set; }

        // ID of the currently selected project (or 0 if none selected)
        public int SelectedProjectId => SelectedProject?.Model?.Id ?? 0;

        // Add a new project to the list or update an existing one
        public void AddProject(Project project)
        {
            _toDoSvc.AddOrUpdateProject(project);
            NotifyPropertyChanged(nameof(Projects));
        }

        // Delete the currently selected project
        public void DeleteProject()
        {
            if (SelectedProject == null) return;

            _toDoSvc.DeleteProject(SelectedProject.Model);
            NotifyPropertyChanged(nameof(Projects));    // Refresh the project list
        }

        // Refresh the project list
        public void RefreshPage()
        {
            NotifyPropertyChanged(nameof(Projects));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        // Notify property change for data binding
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}