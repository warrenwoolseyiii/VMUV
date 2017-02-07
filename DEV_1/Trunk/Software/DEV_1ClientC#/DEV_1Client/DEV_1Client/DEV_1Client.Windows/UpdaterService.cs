using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace DEV_1Client
{
    class UpdaterService
    {
        MainPage parent = null;

        public UpdaterService(MainPage mainPage)
        {
            parent = mainPage;
        }

        public void OnDeviceEnumerationComplete()
        {
            if (parent == null)
                return;
            parent.SetEnumerationStatusText("DEV1 Enumeration Success!");
        }

        public void OnDataReceived(DeviceData data)
        {
            if (parent == null)
                return;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,() =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                {
                    Int16[] rawData = data.GetRawDataInCnts();
                    parent.SetRawDataDisplay(data.ToStringRawDisplayFormat());
                    parent.SetPadHeatMapsInRGB(PadCountsToRGB(rawData[0]));
                }
            );

        }

        private byte[] PadCountsToRGB(Int16 counts)
        {
            // 0 is blue
            // 2048 is green
            // 4096 is red
            byte[] rgb = new byte[3];
            byte temp = (byte)((counts + 1) / 16);

            if (temp <= 128 && temp > 0)
            {
                byte mod = (byte)(temp % 128);
                if (mod != 0)
                {
                    rgb[1] = mod;
                    rgb[2] = (byte)(128 - mod);
                }
                else
                {
                    rgb[1] = 128;
                    rgb[2] = 0;
                }

                rgb[0] = 0;
            }
            else if (temp == 0)
            {
                rgb[1] = 0;
                rgb[2] = 128;
                rgb[0] = 0;
            }
            else
            {
                byte mod = 0;
                temp -= 128;
                mod = (byte)(temp % 128);

                if (mod != 0)
                {
                    rgb[0] = mod;
                    rgb[1] = (byte)(128 - mod);
                }
                else
                {
                    rgb[0] = 128;
                    rgb[1] = 0;
                }

                rgb[2] = 0;
            }

            return rgb;
        }
    }
}
