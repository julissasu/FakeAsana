using Asana.Maui.ViewModels;

namespace Asana.Maui.View
{
	public partial class ToDoDetailView : ContentPage
	{
		public ToDoDetailView()
		{
			InitializeComponent();
			BindingContext = new ToDoDetailViewModel();
		}
	}
}