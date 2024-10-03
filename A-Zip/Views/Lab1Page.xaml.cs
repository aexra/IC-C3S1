using A_Zip.Helpers;
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

        ViewModel.Zipper += (s) =>
        {
            return s;
        };
        ViewModel.Unzipper += (s) =>
        {
            return s;
        };

        InitializeComponent();
    }

    private async void FilePicker_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.SelectFile();
    }

    private async void zipButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.Zip();
    }

    private async void unzipButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.Unzip();
    }
}
