using Asana.Library.Models;
using Asana.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Asana.Maui.ViewModels
{
    public class ProjectViewModel : INotifyPropertyChanged
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

        // Commands
        public ICommand SaveCommand { get; }

        public ProjectViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;
            
            // Initialize commands
            SaveCommand = new Command(SaveProject);
        }

        private async void SaveProject()
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(Name))
            {
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Project name is required", "OK");
                }
                return;
            }

            // Create new Project
            var newProject = new Project
            {
                Id = 0, // Service will assign ID
                Name = Name,
                Description = Description,
                CompletePercent = 0
            };

            // Save the Project
            var savedProject = _toDoSvc.AddOrUpdateProject(newProject);

            if (savedProject != null)
            {
                // Clear form
                ClearForm();

                // Show success message
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Success", "Project created successfully!", "OK");
                }
            }
        }

        private void ClearForm()
        {
            Name = string.Empty;
            Description = string.Empty;
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