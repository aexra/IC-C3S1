using Windows.Storage.Pickers;
using Windows.Storage;

namespace A_Zip.Helpers;

public static class FilePickerHelper
{
    public static async Task<StorageFile?> PickSingleFile(params string[] fileTypes)
    {
        var filePicker = new FileOpenPicker();

        filePicker.ViewMode = PickerViewMode.Thumbnail;
        filePicker.SuggestedStartLocation = PickerLocationId.Desktop;

        foreach (var fileType in fileTypes)
        {
            filePicker.FileTypeFilter.Add($"{fileType}");
        }

        var hwnd = App.MainWindow.GetWindowHandle();
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

        return await filePicker.PickSingleFileAsync();
    }

    public static async Task<StorageFile?> PickSingleFile(IList<string> fileTypes, PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
    {
        var filePicker = new FileOpenPicker();

        filePicker.ViewMode = viewMode;
        filePicker.SuggestedStartLocation = suggestedStartLocation;

        foreach (var fileType in fileTypes)
        {
            filePicker.FileTypeFilter.Add($"{fileType}");
        }

        var hwnd = App.MainWindow.GetWindowHandle();
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

        return await filePicker.PickSingleFileAsync();
    }

    public static async Task<StorageFile?> CreateFile(string suggestedFileName, IDictionary<string, IList<string>> fileTypeChoices)
    {
        var savePicker = new FileSavePicker();
        savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
        savePicker.SuggestedFileName = suggestedFileName;

        if (fileTypeChoices != null)
        {
            foreach (var item in fileTypeChoices)
            {
                savePicker.FileTypeChoices.Add(item);
            };
        }

        var hwnd = App.MainWindow.GetWindowHandle();
        WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

        var file = await savePicker.PickSaveFileAsync();

        return file;
    }
}
