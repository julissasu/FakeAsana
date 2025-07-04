using Asana.Maui.ViewModels;
using Asana.Library.Models;

namespace Asana.Maui
{
	public partial class ProjectsView : ContentPage
	{
		public ProjectsView()
		{
			InitializeComponent();
			BindingContext = new ProjectsPageViewModel();
		}

		private void AddNewProjectClicked(object sender, EventArgs e)
		{
			var viewModel = BindingContext as ProjectsPageViewModel;
			if (viewModel != null)
			{
				viewModel.ShowAddForm = true;
			}
		}

		private void CloseFormClicked(object sender, EventArgs e)
		{
			var viewModel = BindingContext as ProjectsPageViewModel;
			if (viewModel != null)
			{
				viewModel.ShowAddForm = false;
				// Clear the form
				ProjectNameEntry.Text = string.Empty;
				ProjectDescriptionEntry.Text = string.Empty;
			}
		}

		private void CreateProjectClicked(object sender, EventArgs e)
		{
			var projectName = ProjectNameEntry.Text?.Trim();

			if (string.IsNullOrEmpty(projectName))
			{
				DisplayAlert("Error", "Project name is required.", "OK");
				return;
			}

			var newProject = new Project
			{
				Id = 0, // Will be assigned by service
				Name = projectName,
				Description = ProjectDescriptionEntry.Text?.Trim() ?? string.Empty
			};

			var viewModel = BindingContext as ProjectsPageViewModel;
			if (viewModel != null)
			{
				viewModel.AddProject(newProject);

				// Clear and hide form
				ProjectNameEntry.Text = string.Empty;
				ProjectDescriptionEntry.Text = string.Empty;
				viewModel.ShowAddForm = false;
			}
		}

		private void DeleteProjectClicked(object sender, EventArgs e)
		{
			(BindingContext as ProjectsPageViewModel)?.DeleteProject();
		}

		private void BackToMainClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync("//MainPage");
		}

		private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
		{
			(BindingContext as ProjectsPageViewModel)?.RefreshPage();
		}
	}
}