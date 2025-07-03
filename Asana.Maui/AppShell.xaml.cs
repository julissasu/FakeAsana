namespace Asana.Maui
{
	public partial class AppShell : Shell
	{
		public AppShell()
		{
			InitializeComponent();

			// Register routes for navigation
			Routing.RegisterRoute("ToDoDetailView", typeof(ToDoDetailView));
			Routing.RegisterRoute("ProjectsView", typeof(ProjectsView));
		}
	}
}