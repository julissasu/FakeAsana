// ProjectsView.xaml.cs
using Asana.Maui.ViewModels;

namespace Asana.Maui.View;

public partial class ProjectsView : ContentPage
{
	private ProjectsViewModel _viewModel;

	public ProjectsView()
	{
		InitializeComponent();
		_viewModel = new ProjectsViewModel();
		BindingContext = _viewModel;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		// Refresh data when page appears
		_viewModel.RefreshProjects();
	}
}