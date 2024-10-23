using System.Text.RegularExpressions;
using A_Zip.ViewModels;
using Aexra.Codebase.Algorithms.Coding;
using Microsoft.UI.Xaml.Controls;

namespace A_Zip.Views;

public sealed partial class Lab1Page : Page
{
    private const string huffPattern = @"\((.\d+)\)";

    public Lab1ViewModel ViewModel
    {
        get;
    }

    public Lab1Page()
    {
        ViewModel = App.GetService<Lab1ViewModel>();

        ViewModel.Zipper += (s) =>
        {
            var ws = int.Parse(ViewModel.WindowSize);
            var bs = int.Parse(ViewModel.BufferSize);

            var (huff, huffTable) = Huffman.Encode(s, s => System.Diagnostics.Debug.WriteLine(s));
            var (lzssList, _, _) = LZSS.Encode(huff, ws, bs, null);

            System.Diagnostics.Debug.WriteLine(huff);

            var huffData = string.Join("", huffTable.Select(kv => $"({kv.Key}{kv.Value})"));
            var lzss = string.Join("", lzssList.Select(x => x.coded ? $"{x.start},{x.length};" : $"{x.symbol};"));

            return $"{huffData}%s{ws};{bs};{lzss}"[..^1];
        };
        ViewModel.Unzipper += (s) =>
        {
            var parts = s.Split("%s");

            var huffFrequencies = parts[0];

            var lzssData = parts[1];
            var lzssParts = lzssData.Split(";");
            var ws = int.Parse(lzssParts[0]);
            var bs = int.Parse(lzssParts[1]);
            var lzss = lzssParts[2..];

            // BUILDING TABLES

            var frequencyTable = new Dictionary<char, int>();
            foreach (Match match in Regex.Matches(huffFrequencies, huffPattern))
            {
                var part = match.Groups[1].Value;
                var symbol = part[0];
                var frequency = int.Parse(part[1..]);

                frequencyTable.Add(symbol, frequency);
            }

            //System.Diagnostics.Debug.WriteLine(string.Join("\n", frequencyTable.Select(kv => $"{kv.Key} for {kv.Value}")));

            var lzssList = new List<(bool coded, int start, int length, char symbol)>();
            foreach (var code in lzss)
            {
                var codeParts = code.Split(",");

                if (codeParts.Length == 1)
                {
                    lzssList.Add(new(false, 0, -1, code[0]));
                }
                else
                {
                    lzssList.Add((true, int.Parse(codeParts[0]), int.Parse(codeParts[1]), '0'));
                }
            }

            // ENGAGE DECODE

            var huff = LZSS.Decode(lzssList, ws, bs, null);

            System.Diagnostics.Debug.WriteLine(huff);

            var output = Huffman.Decode(huff, frequencyTable, s => System.Diagnostics.Debug.WriteLine(s));

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
        var a = int.TryParse(ViewModel.WindowSize, out var _);
        var b = int.TryParse(ViewModel.BufferSize, out var _);

        if (!a || !b)
        {
            ShellPage.Instance.Notify("Ошибка ввода", "Размеры окна и буфера должны быть целыми числами");
            return;
        }

        await ViewModel.Zip();
    }

    private async void unzipButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.Unzip();
    }
}
