using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DEV_1Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage currentMainPage;
        private static UpdaterService updater = null;
        private static DeviceManager deviceMngr = null;

        public MainPage()
        {
            this.InitializeComponent();
            currentMainPage = this;
            Device dev_1 = new Device();
            updater = new UpdaterService(currentMainPage);
            deviceMngr = new DeviceManager(updater);
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

        public void SetEnumerationStatusText(String s)
        {
            if (s != null)
                ConnectionStatus.Text = s;
        }

        public void SetRawDataDisplay(String s)
        {
            if (s != null)
                RawDataDisplay.Text = s;
        }

        public void SetPadHeatMapsInRGB(byte[] intensity)
        {
            Pad0HeatMap.Fill = new SolidColorBrush(Color.FromArgb(intensity[0], 66, 165, 211));
            Pad1HeatMap.Fill = new SolidColorBrush(Color.FromArgb(intensity[1], 66, 165, 211));
            Pad2HeatMap.Fill = new SolidColorBrush(Color.FromArgb(intensity[2], 66, 165, 211));
            Pad3HeatMap.Fill = new SolidColorBrush(Color.FromArgb(intensity[3], 66, 165, 211));
            Pad4HeatMap.Fill = new SolidColorBrush(Color.FromArgb(intensity[4], 66, 165, 211));
            Pad5HeatMap.Fill = new SolidColorBrush(Color.FromArgb(intensity[5], 66, 165, 211));
            Pad6HeatMap.Fill = new SolidColorBrush(Color.FromArgb(intensity[6], 66, 165, 211));
            Pad7HeatMap.Fill = new SolidColorBrush(Color.FromArgb(intensity[7], 66, 165, 211));
            Pad8HeatMap.Fill = new SolidColorBrush(Color.FromArgb(intensity[8], 66, 165, 211));
        }

        public void UpdateWindow()
        {

        }
    }
}
