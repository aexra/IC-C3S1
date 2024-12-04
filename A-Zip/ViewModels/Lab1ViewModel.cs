using A_Zip.Helpers;
using A_Zip.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;
using Windows.UI.StartScreen;

namespace A_Zip.ViewModels;

public partial class Lab1ViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool _isLoading;

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
        IsLoading = true;

        var file = await FilePickerHelper.PickSingleFile(".txt", ".8z", ".bmp");

        if (file == null) return;

        SelectedFile = file;

        if (file.FileType == ".8z")
        {
            var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.None, 4096, true);
            var reader = new StreamReader(stream);

            SelectedFileRaw = reader.ReadToEnd();

            ShellPage.Instance.Notify(SelectedFileRaw.Length.ToString());

            reader.Close();
            stream.Close();

            IsZipSelected = true;
            IsTextSelected = false;

            // FILL SAVED PARAMETERS
            var lzss = SelectedFileRaw.Split("%s").Last().Split(";");

            WindowSize = lzss[0];
            BufferSize = lzss[1];
        }
        else
        {
            var bytes = await file.ReadBytesAsync();
            SelectedFileRaw = Convert.ToBase64String(bytes);

            IsZipSelected = false;
            IsTextSelected = true;
        }

        IsFileSelected = true;
        IsLoading = false;
    }

    public async Task Zip()
    {
        IsLoading = true;

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

        IsLoading = false;
    }

    public async Task Unzip()
    {
        IsLoading = true;

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



        //var stream = new FileStream(file.Path, FileMode.Open, FileAccess.Write, FileShare.None, 4096, true);
        //var writer = new StreamWriter(stream);

        var result = Unzipper(SelectedFileRaw[(sourceFileName.Length + 1)..]);

        //writer.Write(result);

        //writer.Close();
        //stream.Close();

        var bytes = SafeConvertToByteArray(result);

        if (sourceFileName.Split(".")[^1][..^1] == "bmp")
            bytes = ValidateBmp(bytes);

        File.WriteAllBytes(file.Path, bytes);
        
        ResultRaw = result;
        IsResultDone = true;

        IsLoading = false;
    }

    public static byte[] SafeConvertToByteArray(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Array.Empty<byte>(); // Возвращаем пустой массив, если строка пуста

        input = input.Trim(); // Убираем лишние пробелы

        // Попробуем обработать строку, уменьшая ее до последнего корректного символа
        while (input.Length > 0)
        {
            try
            {
                return Convert.FromBase64String(input);
            }
            catch (FormatException)
            {
                // Если строка невалидна, обрезаем 1 символ и пробуем снова
                input = input.Substring(0, input.Length - 1);
            }
        }

        // Если ничего не удалось преобразовать, возвращаем пустой массив
        return Array.Empty<byte>();
    }

    public static byte[] ValidateBmp(byte[] bmpData)
    {
        // Проверяем, чтобы массив не был пустым и имел минимум 54 байта (стандартный заголовок BMP)
        if (bmpData == null || bmpData.Length < 54)
            throw new ArgumentException("Недопустимый файл BMP: слишком маленький размер.");

        // Получаем ожидаемый размер файла из заголовка BMP (4 байта, начиная с 2-го индекса)
        var fileSize = BitConverter.ToInt32(bmpData, 2);

        // Если фактический размер меньше указанного в заголовке, добавляем недостающие байты
        if (bmpData.Length < fileSize)
        {
            var fixedData = new byte[fileSize];
            Array.Copy(bmpData, fixedData, bmpData.Length); // Копируем существующие данные
            // Остальные байты автоматически инициализируются нулями
            return fixedData;
        }

        // Если размер совпадает или превышает указанный, возвращаем исходный массив
        return bmpData;
    }

    public Lab1ViewModel()
    {
        IsResultDone = false;
        IsFileSelected = false;
    }
}
