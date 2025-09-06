using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IRCameraView
{
    /// <summary>
    /// The window that displays the camera feed.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        //private bool _isRecording = false;
        public static NavigationService NavigationService { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();

            GoToMainPage();
        }

        public void GoToMainPage()
        {
            MainFrame.Navigate(typeof(MainPage));
        }

        public void GoToRecordingPage()
        {
            MainFrame.Navigate(typeof(RecordingPage));
        }
    }
}
