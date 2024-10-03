using A_Zip.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace A_Zip.Views;

public sealed partial class Lab5Page : Page
{
    public Lab5ViewModel ViewModel
    {
        get;
    }

    public Lab5Page()
    {
        ViewModel = App.GetService<Lab5ViewModel>();
        InitializeComponent();
    }
}
