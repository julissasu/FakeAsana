namespace Asana.Maui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		// Register routes for navigation
		Routing.RegisterRoute("ToDoDetailView", typeof(View.ToDoDetailView));
		Routing.RegisterRoute("ProjectsView", typeof(View.ProjectsView));
	}
}