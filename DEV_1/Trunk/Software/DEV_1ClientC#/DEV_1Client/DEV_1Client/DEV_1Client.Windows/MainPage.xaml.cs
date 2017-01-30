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
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Enumeration;
using System.Threading.Tasks;
using Windows.Devices.HumanInterfaceDevice;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DEV_1Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            EnumerateDevices();
        }

        static async Task EnumerateDevices()
        {
            ushort vId, pId, uPage, uId;

            vId = 0x6969;
            pId = 0x0001;
            uPage = 0xFFFF;
            uId = 0x00FF;

            try
            {
                string selector = HidDevice.GetDeviceSelector(uPage, uId, vId, pId);
                var devices = await DeviceInformation.FindAllAsync(selector);
                
                if (devices.Count > 0)
                {
                    string deviceIdStr = devices.ElementAt(0).Id;
                    HidDevice device = await HidDevice.FromIdAsync(devices.ElementAt(0).Id, Windows.Storage.FileAccessMode.ReadWrite);
                }
            }
            catch (Exception e0)
            {
                var ex = e0;
            }
        }
    }
}
