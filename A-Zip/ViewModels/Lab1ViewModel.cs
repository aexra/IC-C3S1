using A_Zip.Helpers;
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

        reader.Close();
        stream.Close();

        IsFileSelected = true;
    }

    public async Task Zip()
    {
        if (SelectedFile == null) return;

        var file = await FilePickerHelper.CreateFile(SelectedFile.DisplayName + $".8z", new Dictionary<string, IList<string>>() { { "8-Zip archive", new List<string>() { ".8z" } } });

        if (file == null) return;

        var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true);
        var writer = new StreamWriter(stream);


    }

    public async Task Unzip()
    {
        if (SelectedFile == null || SelectedFile.FileType != "8z") return;

        var file = await FilePickerHelper.CreateFile(SelectedFile.DisplayName + $".8z", new Dictionary<string, IList<string>>() { { "8-Zip archive", new List<string>() { ".8z" } } });

        if (file == null) return;

        var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true);
        var writer = new StreamWriter(stream);


    }

    public Lab1ViewModel()
    {
        IsResultDone = false;
        IsFileSelected = false;
    }
}
