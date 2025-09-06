using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IRCameraView;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RecordingPage : Page
{
    private DispatcherTimer _timer;
    private StorageFolder _folder;

    private double _ballX = 0, _ballY = 0;

    private IRController _irController;

    public RecordingPage()
    {
        InitializeComponent();


        SelectFolder();
    }

    private void AnimationTimer_Tick(object sender, object e)
    {
        Canvas.SetTop(Ball, _ballY++);
        Canvas.SetLeft(Ball, _ballX++);
    }

    private void StartRecording()
    {
        _irController = new IRController(IRFrameFilter.Illuminated, FrameReady, 0);

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
        _timer.Tick += AnimationTimer_Tick;
        _timer.Start();
    }

    private void SelectFolder()
    {
        try
        {
            var folderPicker = new FolderPicker();

            folderPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");

            var window = WindowNative.GetWindowHandle(MainWindow.Window);
            InitializeWithWindow.Initialize(folderPicker, window);
            
            DispatcherQueue.TryEnqueue(async () =>
            {
                _folder = await folderPicker.PickSingleFolderAsync();
                StartRecording();
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Folder selection failed: {ex.Message}");
        }
    }

    private void FrameReady(SoftwareBitmap bitmap)
    {
        _ballY += 10;

        SaveBitmap(bitmap).Wait();
    }

    private async Task SaveBitmap(SoftwareBitmap bitmap)
    {
        try
        {
            if (_folder == null) return;

            StorageFile file = await _folder.CreateFileAsync($"{_ballX}-{_ballY}.jpg", CreationCollisionOption.GenerateUniqueName);

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);

                encoder.SetSoftwareBitmap(bitmap);

                var propertySet = new BitmapPropertySet();
                var qualityValue = new BitmapTypedValue(0.9, PropertyType.Single);
                propertySet.Add("ImageQuality", qualityValue);

                await encoder.FlushAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to save image: {ex.Message}");
        }
    }
}
