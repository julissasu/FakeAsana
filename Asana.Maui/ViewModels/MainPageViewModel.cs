using Asana.Library.Models;
using Asana.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.Generic;

namespace Asana.Maui.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ToDoServiceProxy _toDoSvc;
        private ProjectServiceProxy _projectSvc;
        private readonly ObservableCollection<string> _sortOptionsList;
        private string _selectedSortOption;


        public MainPageViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;
            _projectSvc = ProjectServiceProxy.Current;
            Query = string.Empty;

            _sortOptionsList = new ObservableCollection<string>
            {
                "None",
                "Name",
                "Priority",
                "DueDate",
                "Project",
                "IsComplete"
            };

            _selectedSortOption = "None"; // Default to 'None'

            // Listen for ToDo completion changes
            ToDoDetailViewModel.TaskCompletionChanged += RefreshPage;
        }

        // Currently selected ToDo item
        public ToDoDetailViewModel? SelectedToDo { get; set; }

        private string query = string.Empty;


        // Search query for filtering ToDos
        public string Query
        {
            get
            {
                return query;
            }
            set
            {
                if (query != value)
                {
                    query = value;
                    NotifyPropertyChanged();
                    RefreshPage();
                }
            }
        }

        public ObservableCollection<string> SortOptionsList => _sortOptionsList;

        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                if (_selectedSortOption != value)
                {
                    _selectedSortOption = value;
                    NotifyPropertyChanged();
                    RefreshPage();
                }
            }
        }

        // Filtered and sorted lists of ToDos
        public ObservableCollection<ToDoDetailViewModel> ToDos
        {
            get
            {
                var toDos = _toDoSvc.ToDos
                    .Where(t => (t?.Name?.Contains(Query, StringComparison.OrdinalIgnoreCase) ?? false) || (t?.Description?.Contains(Query, StringComparison.OrdinalIgnoreCase) ?? false));

                if (!IsShowCompleted)
                {
                    toDos = toDos.Where(t => !(t?.IsComplete ?? false));
                }

                // Apply sorting
                switch (SelectedSortOption)
                {
                    case "Name":
                        toDos = toDos.OrderBy(t => t.Name);
                        break;
                    case "Priority":
                        toDos = toDos.OrderBy(t => t.Priority);
                        break;
                    case "DueDate":
                        toDos = toDos.OrderBy(t => t.DueDate);
                        break;
                    case "Project":
                        toDos = toDos.OrderBy(t => _projectSvc.GetProjectById(t.ProjectId)?.Name);
                        break;
                    case "IsComplete":
                        toDos = toDos.OrderBy(t => t.IsComplete);
                        break;
                    default:
                        // No sorting
                        break;
                }

                return new ObservableCollection<ToDoDetailViewModel>(toDos.Select(t => new ToDoDetailViewModel(t)));
            }
        }

        // ID of the currently selected ToDo item
        public int SelectedToDoId => SelectedToDo?.Model?.Id ?? 0;

        private bool isShowCompleted;

        // Toggle to show/hide completed ToDos
        public bool IsShowCompleted
        {
            get
            {
                return isShowCompleted;
            }
            set
            {
                if (isShowCompleted != value)
                {
                    isShowCompleted = value;
                    NotifyPropertyChanged();
                    RefreshPage();
                }
            }
        }

        // Delete the currently selected ToDo item
        public void DeleteToDo()
        {
            if (SelectedToDo == null)
            {
                return;
            }
            _toDoSvc.DeleteToDo(SelectedToDo.Model);
            RefreshPage();
        }

        // Refresh the ToDos list
        public void RefreshPage()
        {
            NotifyPropertyChanged(nameof(ToDos));
        }

        // Called when the search button is clicked
        public void HandleSearchClick()
        {
            RefreshPage();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        // Notify property change for data binding
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}