﻿using CommunityToolkit.Mvvm.ComponentModel;
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

    public void SelectFile(StorageFile file)
    {
        SelectedFile = file;

        var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true);
        var reader = new StreamReader(stream);

        SelectedFileRaw = reader.ReadToEnd();

        reader.Close();
        stream.Close();

        IsFileSelected = true;
    }

    public Lab1ViewModel()
    {
        IsResultDone = false;
        IsFileSelected = false;
    }
}
