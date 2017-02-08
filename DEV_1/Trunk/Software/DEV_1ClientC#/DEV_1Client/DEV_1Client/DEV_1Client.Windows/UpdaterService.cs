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
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            {
                Int16[] rawData = data.GetRawDataInCnts();
                parent.SetRawDataDisplay(data.ToStringRawDisplayFormat());
                parent.SetPadHeatMapsInRGB(PadCountsToColorDensity(rawData));
            }
            );

        }

        private byte[] PadCountsToColorDensity(Int16 [] data)
        {
            byte[] rtn = new byte[9];
            for (int i = 0; i < data.Length; i++)
                rtn[i] = (byte)(data[i] / 16);
            return rtn;
        }
    }
}
