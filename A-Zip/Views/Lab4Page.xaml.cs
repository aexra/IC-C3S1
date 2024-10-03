using A_Zip.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace A_Zip.Views;

public sealed partial class Lab4Page : Page
{
    public Lab4ViewModel ViewModel
    {
        get;
    }

    public Lab4Page()
    {
        ViewModel = App.GetService<Lab4ViewModel>();
        InitializeComponent();
    }
}
