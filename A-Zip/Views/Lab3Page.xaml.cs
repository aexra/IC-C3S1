using A_Zip.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace A_Zip.Views;

public sealed partial class Lab3Page : Page
{
    public Lab3ViewModel ViewModel
    {
        get;
    }

    public Lab3Page()
    {
        ViewModel = App.GetService<Lab3ViewModel>();
        InitializeComponent();
    }
}
