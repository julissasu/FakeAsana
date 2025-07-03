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
        }

        public ToDoDetailViewModel? SelectedToDo { get; set; }

        private string query = string.Empty;
        public string Query
        {
            get { return query; }
            set
            {
                if (query != value)
                {
                    query = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ObservableCollection<ToDoDetailViewModel> ToDos
        {
            get
            {
                var toDos = _toDoSvc.ToDos.Where(t => (t?.Name?.Contains(Query) ?? false) || (t?.Description?.Contains(Query) ?? false))
                        .Select(t => new ToDoDetailViewModel(t));

                if (!IsShowCompleted)
                {
                    toDos = toDos.Where(t => !(t?.Model?.IsComplete ?? false));
                }

                return new ObservableCollection<ToDoDetailViewModel>(toDos);
            }
        }

        public int SelectedToDoId => SelectedToDo?.Model?.Id ?? 0;

        private bool isShowCompleted;
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
                    NotifyPropertyChanged(nameof(ToDos));
                }
            }
        }

        public void DeleteToDo()
        {
            if (SelectedToDo == null)
            {
                return;
            }
            ToDoServiceProxy.Current.DeleteToDo(SelectedToDo.Model);
            NotifyPropertyChanged(nameof(ToDos));
        }

        public void RefreshPage()
        {
            NotifyPropertyChanged(nameof(ToDos));
        }

        public void HandleSearchClick()
        {
            RefreshPage();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}