using A_Zip.Helpers;
using A_Zip.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace A_Zip.ViewModels;

public partial class Lab1ViewModel : ObservableRecipient
{
    [ObservableProperty]
    private StorageFile _selectedFile;

    [ObservableProperty]
    private string _selectedFileRaw;

    [ObservableProperty]
    private string _resultRaw;

    [ObservableProperty]
    private bool _isResultDone;
    
    [ObservableProperty]
    private bool _isFileSelected;

    [ObservableProperty]
    private bool _isTextSelected;

    [ObservableProperty]
    private bool _isZipSelected;

    [ObservableProperty]
    private string _windowSize;

    [ObservableProperty]
    private string _bufferSize;

    public Func<string, string> Zipper;
    public Func<string, string> Unzipper;

    public async Task SelectFile()
    {
        var file = await FilePickerHelper.PickSingleFile(".txt", ".8z");

        if (file == null) return;

        SelectedFile = file;

        var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true);
        var reader = new StreamReader(stream);

        SelectedFileRaw = reader.ReadToEnd();

        //ShellPage.Instance.Notify(SelectedFileRaw.Length.ToString());

        reader.Close();
        stream.Close();

        IsFileSelected = true;

        if (file.FileType == ".8z")
        {
            IsZipSelected = true;

            // FILL SAVED PARAMETERS
            var lzss = SelectedFileRaw.Split("%s").Last().Split(";");

            WindowSize = lzss[0];
            BufferSize = lzss[1];
        }
        else IsZipSelected = false;

        if (file.FileType == ".txt") IsTextSelected = true;
        else IsTextSelected = false;
    }

    public async Task Zip()
    {
        if (SelectedFile == null) return;

        var file = await FilePickerHelper.CreateFile(SelectedFile.DisplayName + $".8z", new Dictionary<string, IList<string>>() { { "8-Zip archive", new List<string>() { ".8z" } } });

        if (file == null) return;

        var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Write, FileShare.None, 4096, true);
        var writer = new StreamWriter(stream);

        writer.WriteLine(SelectedFile.Name);

        var result = Zipper(SelectedFileRaw);

        writer.Write(result);

        writer.Close();
        stream.Close();

        ResultRaw = SelectedFile.Name + "\n" + result;
        IsResultDone = true;
    }

    public async Task Unzip()
    {
        if (SelectedFile == null || SelectedFile.FileType != ".8z") return;

        var sourceFileName = "";
        var idx = 0;
        while (true)
        {
            var c = SelectedFileRaw[idx];
            if (c == '\n') break;
            sourceFileName += c;
            idx++;
        }

        var file = await FilePickerHelper.CreateFile(string.Join(".", sourceFileName.Split('.')[..^1]), new Dictionary<string, IList<string>>() { { "Unknown Document", new List<string>() { $".{sourceFileName.Split('.').Last()[..^1]}" } } });

        if (file == null) return;

        var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Write, FileShare.None, 4096, true);
        var writer = new StreamWriter(stream);

        var result = Unzipper(SelectedFileRaw[(sourceFileName.Length + 1)..]);

        writer.Write(result);

        writer.Close();
        stream.Close();

        ResultRaw = result;
        IsResultDone = true;
    }

    public Lab1ViewModel()
    {
        IsResultDone = false;
        IsFileSelected = false;
    }
}
