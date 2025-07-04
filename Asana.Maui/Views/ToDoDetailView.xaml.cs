using Asana.Library.Models;
using Asana.Maui.ViewModels;

namespace Asana.Maui
{
	// Enables navigation to this page with a query parameter
	[QueryProperty(nameof(ToDoId), "toDoId")]
	public partial class ToDoDetailView : ContentPage
	{
		public ToDoDetailView()
		{
			InitializeComponent();
		}

		// Property to hold the ToDo ID passed via navigation
		public int ToDoId { get; set; }

		// Navigates back to the main page without saving changes
		private void CancelClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync("//MainPage");
		}

		// Triggers the add or update operation in the ViewModel
		private void OkClicked(object sender, EventArgs e)
		{
			(BindingContext as ToDoDetailViewModel)?.AddOrUpdateToDo();
			Shell.Current.GoToAsync("//MainPage");
		}

		// Handles the page navigation events
		private void ContentPage_NavigatedFrom(object sender, NavigatedFromEventArgs e)
		{
		}

		// Initializes the ViewModel with the ToDo ID when the page is navigated to
		private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
		{
			BindingContext = new ToDoDetailViewModel(ToDoId);
		}
	}
}