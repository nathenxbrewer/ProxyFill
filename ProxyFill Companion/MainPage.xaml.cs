namespace ProxyFill_Companion;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    
    private async void OnSelectFileClicked(object sender, EventArgs e)
    {
        var customFileType = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.my.comic.extension" } }, // UTType values
                { DevicePlatform.Android, new[] { "application/comics" } }, // MIME type
                { DevicePlatform.WinUI, new[] { ".cbr", ".cbz" } }, // file extension
                { DevicePlatform.Tizen, new[] { "*/*" } },
                { DevicePlatform.MacCatalyst, new[] { "json", "json" } }, // UTType values
            });

        PickOptions options = new()
        {
            PickerTitle = "Please select your ProxyFill file.",
            FileTypes = customFileType,
        };

        await PickAndShow(options);
    }
    
    public async Task<FileResult> PickAndShow(PickOptions options)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                var image = ImageSource.FromStream(() => stream);
            }

            return result;
        }
        catch (Exception ex)
        {
            // The user canceled or something went wrong
        }

        return null;
    }
}