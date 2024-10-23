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

            var shuffData = string.Join("|", huffTable.Select(kv => $"({kv.Key}||{kv.Value})"));
            var slzss = string.Join("|", lzssList.Select(x => x.coded ? $"(1<{x.start},{x.length}>)" : $"(0<{x.symbol}>)"));

            return $"{shuffData}\n{ws},{bs}\n{slzss}";
        };
        ViewModel.Unzipper += (s) =>
        {
            var shuffData = s.Split("\n")[0..3];
            var slzss = string.Join("\n", s.Split("\n")[3..]);
            var huffTable = new Dictionary<char, string>();
            var ws = int.Parse(s.Split("\n")[4].Split(",")[0]);
            var bs = int.Parse(s.Split("\n")[4].Split(",")[1]);
            var lzssList = new List<(bool coded, int start, int length, char symbol)>();

            foreach (Match match in Regex.Matches(shuffData, huffPattern))
            {
                huffTable.Add(char.Parse(match.Groups[1].Value.ToString()), match.Groups[2].Value.ToString());
            }
            foreach (Match match in Regex.Matches(slzss, lzssPattern))
            {
                var g = match.Groups[1].Value;
                var splittedVals = g[2..^1].Split(',');

                var coded = g[0] == '1' ? true : false;
                var start = coded ? int.Parse(splittedVals[0]) : 0;
                var length = coded ? int.Parse(splittedVals[1]) : -1;
                var symbol = coded ? '0' : char.Parse(splittedVals[0]);

                lzssList.Add((coded, start, length, symbol));
            }

            var huff = LZSS.Decode(lzssList, ws, bs, null);
            var output = Huffman.Decode(huff, huffTable);

            return output;
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
