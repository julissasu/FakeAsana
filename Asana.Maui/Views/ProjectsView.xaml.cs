using Asana.Maui.ViewModels;

namespace Asana.Maui
{
	public partial class ProjectsView : ContentPage
	{
		public ProjectsView()
		{
			InitializeComponent();
			BindingContext = new ProjectsPageViewModel();
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