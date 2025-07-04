using Asana.Maui.ViewModels;
using Asana.Library.Models;

namespace Asana.Maui
{
	public partial class ProjectsView : ContentPage
	{
		public ProjectsView()
		{
			InitializeComponent();
			// Set the data context for data binding to the view model
			BindingContext = new ProjectsPageViewModel();
		}

		// Show the "Add Project" form when the button is clicked
		private void AddNewProjectClicked(object sender, EventArgs e)
		{
			var viewModel = BindingContext as ProjectsPageViewModel;
			if (viewModel != null)
			{
				viewModel.ShowAddForm = true;
			}
		}

		// Hide the "Add Project" form when the close/cancel button is clicked
		private void CloseFormClicked(object sender, EventArgs e)
		{
			var viewModel = BindingContext as ProjectsPageViewModel;
			if (viewModel != null)
			{
				viewModel.ShowAddForm = false;
				// Clear form inputs
				ProjectNameEntry.Text = string.Empty;
				ProjectDescriptionEntry.Text = string.Empty;
			}
		}

		// Validate and create a new project
		private void CreateProjectClicked(object sender, EventArgs e)
		{
			var projectName = ProjectNameEntry.Text?.Trim();

			// Validate project name
			if (string.IsNullOrEmpty(projectName))
			{
				DisplayAlert("Error", "Project name is required.", "OK");
				return;
			}

			// Create new project model
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

		// Delete the selected project 
		private void DeleteProjectClicked(object sender, EventArgs e)
		{
			(BindingContext as ProjectsPageViewModel)?.DeleteProject();
		}

		// Navigate back to the main page
		private void BackToMainClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync("//MainPage");
		}

		// Handle navigation to this page
		private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
		{
			(BindingContext as ProjectsPageViewModel)?.RefreshPage();
		}
	}
}