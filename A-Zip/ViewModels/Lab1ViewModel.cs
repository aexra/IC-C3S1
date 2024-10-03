using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace A_Zip.ViewModels;

public partial class Lab1ViewModel : ObservableRecipient
{
    [ObservableProperty]
    private StorageFile _selectedFile;

    [ObservableProperty]
    private string _selectedFileRaw;

    public void SelectFile(StorageFile file)
    {
        SelectedFile = file;

        var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true);
        var reader = new StreamReader(stream);

        SelectedFileRaw = reader.ReadToEnd();

        reader.Close();
        stream.Close();
    }

    public Lab1ViewModel()
    {
    }
}
