using Asana.Library.Models;
using Asana.Maui.ViewModels;

namespace Asana.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Set the data context for data binding to the view model
            BindingContext = new MainPageViewModel();
        }

        // Show the "Add ToDo" form
        private void AddNewClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//ToDoDetails");
        }

        // Navigate to the ToDo details page for editing
        private void EditClicked(object sender, EventArgs e)
        {
            var selectedId = (BindingContext as MainPageViewModel)?.SelectedToDoId ?? 0;
            Shell.Current.GoToAsync($"//ToDoDetails?toDoId={selectedId}");
        }

        // Delete the selected ToDo item
        private void DeleteClicked(object sender, EventArgs e)
        {
            (BindingContext as MainPageViewModel)?.DeleteToDo();
        }

        // Handle the page navigation events
        private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
        {
            (BindingContext as MainPageViewModel)?.RefreshPage();
        }

        private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
        {
        }

        // Navigate to the Projects page
        private void ProjectClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//ProjectPage");
        }

        // Handle the search button click
        private void SearchClicked(object sender, EventArgs e)
        {
            (BindingContext as MainPageViewModel)?.HandleSearchClick();
        }
    }
}