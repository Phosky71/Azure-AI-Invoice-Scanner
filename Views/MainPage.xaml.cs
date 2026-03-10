using MauiOCRFacturas.ViewModels;

namespace MauiOCRFacturas.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
