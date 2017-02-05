using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DEV_1Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage currentMainPage;
        private static UpdaterService updater = new UpdaterService();
        private static DeviceManager deviceMngr = new DeviceManager(updater);

        public MainPage()
        {
            this.InitializeComponent();
            currentMainPage = this;

            InitializeModules();
        }

        private void InitializeModules()
        {
            LoadDEV1Image();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            deviceMngr.EnumerateDEV_1();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private void LoadDEV1Image()
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = new Uri(this.BaseUri, "Assets/DEV_1Sample.PNG");
            DEV_1Pad.Source = bitmapImage;
        }
    }
}
