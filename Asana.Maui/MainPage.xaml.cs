using Asana.Library.Models;
using Asana.Library.Services;
using Asana.Maui.ViewModels;

namespace Asana.Maui;

public partial class MainPage : ContentPage
{
	private MainPageViewModel _viewModel;

	public MainPage()
	{
		InitializeComponent();
		_viewModel = new MainPageViewModel();
		BindingContext = _viewModel;
	}

	private void OnToDoCompleteChanged(object sender, CheckedChangedEventArgs e)
	{
		if (sender is CheckBox checkBox && checkBox.BindingContext is ToDo toDo)
		{
			// Update the ToDo with the new completion status
			ToDoServiceProxy.Current.AddOrUpdateToDo(toDo);
		}
	}
}