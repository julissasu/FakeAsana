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

        public ProjectsPageViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;
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