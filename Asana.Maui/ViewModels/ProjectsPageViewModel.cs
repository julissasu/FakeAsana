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

            // Listen for task completion changes to update project summaries
            ToDoDetailViewModel.TaskCompletionChanged += RefreshPage;
        }

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

        public ObservableCollection<ProjectViewModel> Projects
        {
            get
            {
                var projects = _toDoSvc.Projects.Select(p => new ProjectViewModel { Model = p });
                return new ObservableCollection<ProjectViewModel>(projects);
            }
        }

        public ProjectViewModel? SelectedProject { get; set; }

        public int SelectedProjectId => SelectedProject?.Model?.Id ?? 0;

        public void AddProject(Project project)
        {
            _toDoSvc.AddOrUpdateProject(project);
            NotifyPropertyChanged(nameof(Projects));
        }

        public void DeleteProject()
        {
            if (SelectedProject == null) return;

            _toDoSvc.DeleteProject(SelectedProject.Model);
            NotifyPropertyChanged(nameof(Projects));
        }

        public void RefreshPage()
        {
            NotifyPropertyChanged(nameof(Projects));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}