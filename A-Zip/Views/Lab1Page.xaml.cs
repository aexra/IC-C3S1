using A_Zip.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace A_Zip.Views;

public sealed partial class Lab1Page : Page
{
    public Lab1ViewModel ViewModel
    {
        get;
    }

    public Lab1Page()
    {
        ViewModel = App.GetService<Lab1ViewModel>();
        InitializeComponent();
    }
}
