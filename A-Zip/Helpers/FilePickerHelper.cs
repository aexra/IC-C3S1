using Windows.Storage.Pickers;
using Windows.Storage;

namespace A_Zip.Helpers;

public static class FilePickerHelper
{
    /// <summary>
    /// Accepts file types in format "jpg|jpeg|png"
    /// </summary>
    /// <param name="fileTypes"></param>
    public static async Task<StorageFile?> PickSingleFile(string fileTypes = "jpg|jpeg|png", PickerLocationId suggestedStartLocation = PickerLocationId.Desktop, PickerViewMode viewMode = PickerViewMode.Thumbnail)
    {
        var filePicker = new FileOpenPicker();

        filePicker.ViewMode = viewMode;
        filePicker.SuggestedStartLocation = suggestedStartLocation;

        fileTypes.Split('|').ToList().ForEach(type => filePicker.FileTypeFilter.Add($".{type}"));

        var hwnd = App.MainWindow.GetWindowHandle();
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

        return await filePicker.PickSingleFileAsync();
    }
}
