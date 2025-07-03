using Asana.Library.Models;
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

    // Handle checkbox changed event for ToDo items
    private void OnToDoCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is ToDo todo)
        {
            // The ToggleToDoCompleteCommand will handle the logic
            _viewModel.ToggleToDoCompleteCommand.Execute(todo);
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Refresh data when page appears
        _viewModel.RefreshData();
    }
}