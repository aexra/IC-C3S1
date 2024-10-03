using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace A_Zip.ViewModels;

public partial class Lab1ViewModel : ObservableRecipient
{
    public StorageFile SelectedFile;

    public Lab1ViewModel()
    {
    }
}
