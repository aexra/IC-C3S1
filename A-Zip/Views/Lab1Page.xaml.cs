using System.Text.RegularExpressions;
using A_Zip.ViewModels;
using Aexra.Codebase.Algorithms.Coding;
using Microsoft.UI.Xaml.Controls;

namespace A_Zip.Views;

public sealed partial class Lab1Page : Page
{
    private const string huffPattern = @"\(([^|]+)\|\|([^|]+)\)";
    private const string lzssPattern = @"\(([^()]+)\)";

    public Lab1ViewModel ViewModel
    {
        get;
    }

    public Lab1Page()
    {
        ViewModel = App.GetService<Lab1ViewModel>();

        ViewModel.Zipper += (s, ws, bs) =>
        {
            var (huff, huffTable) = Huffman.Encode(s);
            var (lzssList, _, _) = LZSS.Encode(huff, ws, bs, null);

            var shuffData = string.Join("", huffTable.Select(kv => $"({kv.Key}{kv.Value})"));
            var slzss = string.Join("", lzssList.Select(x => x.coded ? $"{x.start},{x.length};" : $"{x.symbol};"));

            return $"{shuffData}%s{ws};{bs};{slzss}"[..^1];
        };
        ViewModel.Unzipper += (s) =>
        {
            var parts = s.Split("%s");

            var huffFrequencies = parts[0];
            var lzssData = parts[1];

            var size = lzssData.Split(";")[..2];
            var lzss = lzssData[2..];

            return "";

            //var huff = LZSS.Decode(lzssList, ws, bs, null);
            //var output = Huffman.Decode(huff, huffTable);

            //return output;
        };

        InitializeComponent();
    }

    private async void FilePicker_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.SelectFile();
    }

    private async void zipButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var a = int.TryParse(windowSizeBox.Text, out var ws);
        var b = int.TryParse(bufferSizeBox.Text, out var bs);

        if (!a || !b)
        {
            ShellPage.Instance.Notify("Ошибка ввода", "Размеры окна и буфера должны быть целыми числами");
            return;
        }

        await ViewModel.Zip(ws, bs);
    }

    private async void unzipButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.Unzip();
    }
}
