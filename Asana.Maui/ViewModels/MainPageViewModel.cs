using Asana.Library.Models;
using Asana.Library.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Asana.Maui.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ToDoServiceProxy _toDoSvc;

        public MainPageViewModel()
        {
            _toDoSvc = ToDoServiceProxy.Current;
            Query = string.Empty;

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
                    NotifyPropertyChanged(nameof(ToDos)); // Refresh ToDos list when query changes
                }
            }
        }

        // Filtered list of ToDos based on the search query and completion status
        public ObservableCollection<ToDoDetailViewModel> ToDos
        {
            get
            {
                var toDos = _toDoSvc.ToDos
                    .Where(t => (t?.Name?.Contains(Query) ?? false) || (t?.Description?.Contains(Query) ?? false))
                    .Select(t => new ToDoDetailViewModel(t));

                if (!IsShowCompleted)
                {
                    toDos = toDos.Where(t => !(t?.Model?.IsComplete ?? false));
                }

                return new ObservableCollection<ToDoDetailViewModel>(toDos);
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
                    NotifyPropertyChanged(nameof(ToDos)); // Refresh list when toggle changes
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
            ToDoServiceProxy.Current.DeleteToDo(SelectedToDo.Model);
            NotifyPropertyChanged(nameof(ToDos));
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