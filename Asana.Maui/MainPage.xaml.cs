using Asana.Maui.ViewModels;

namespace Asana.Maui;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

		// Set the ViewModel as BindingContext
		BindingContext = new MainPageViewModel();
	}
}