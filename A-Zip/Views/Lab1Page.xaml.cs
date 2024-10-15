using A_Zip.ViewModels;
using Aexra.Codebase.Algorithms.Coding;
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
            var (huff, huffTable) = Huffman.Encode(s);
            var (data, ws, bs) = LZSS.Encode(huff, 8, 5, (l) => System.Diagnostics.Debug.WriteLine(l));

            var shuffData = string.Join("|", huffTable.Select(kv => $"({kv.Key}||{kv.Value})"));
            var lzss = string.Join("|", data.Select(x => x.coded ? $"(1<{x.start},{x.length}>)" : $"(0<{x.symbol}>)"));

            return shuffData + "\n" + lzss;
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
