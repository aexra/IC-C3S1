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

        ViewModel.Zipper += (s) =>
        {
            var (huff, huffTable) = Huffman.Encode(s);
            var (lzssList, ws, bs) = LZSS.Encode(huff, 8, 5, (l) => System.Diagnostics.Debug.WriteLine(l));

            var shuffData = string.Join("|", huffTable.Select(kv => $"({kv.Key}||{kv.Value})"));
            var lzss = string.Join("|", lzssList.Select(x => x.coded ? $"(1<{x.start},{x.length}>)" : $"(0<{x.symbol}>)"));

            return shuffData + "\n" + lzss;
        };
        ViewModel.Unzipper += (s) =>
        {
            var shuffData = s.Split("\n")[0];
            var huffTable = new Dictionary<string, string>();
            var lzssList = new List<(bool coded, int start, int length, char symbol)>();

            foreach (Match match in Regex.Matches(shuffData, huffPattern))
            {
                huffTable.Add(match.Groups[1].Value.ToString(), match.Groups[2].Value.ToString());
            }
            foreach (Match match in Regex.Matches(string.Join("\n", s.Split("\n")[1..]), lzssPattern))
            {
                var g = match.Groups[1].Value;
                var splittedVals = g[2..^1].Split(',');

                var coded = g[0] == '1' ? true : false;
                var start = coded ? int.Parse(splittedVals[0]) : 0;
                var length = coded ? int.Parse(splittedVals[1]) : -1;
                var symbol = coded ? '0' : char.Parse(splittedVals[0]);

                lzssList.Add((coded, start, length, symbol));
            }

            return string.Join(",", lzssList.Select(x => $"{x.coded}:{x.start}:{x.length}:{x.symbol}"));
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
