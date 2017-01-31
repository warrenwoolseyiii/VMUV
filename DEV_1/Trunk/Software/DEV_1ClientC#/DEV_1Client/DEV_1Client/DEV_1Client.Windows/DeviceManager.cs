using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.Foundation;

namespace DEV_1Client
{
    class DeviceManager
    {
        private static Device dev_1 = new Device();
        private static HidDevice hidDevice = null;
        private static bool deviceIsEnum = false;
        private static bool enumInProgress = false;

        public bool IsDeviceEnumerated()
        {
            return deviceIsEnum;
        }

        public bool EnumerationInProgress()
        {
            return enumInProgress;
        }

        public async Task EnumerateDEV_1()
        {
            enumInProgress = true;

            if (!deviceIsEnum)
            {
                try
                {
                    var deviceInfo = await DeviceInformation.FindAllAsync(dev_1.GetSelector());

                    if (deviceInfo.Count > 0)
                        hidDevice = await HidDevice.FromIdAsync(deviceInfo.ElementAt(0).Id, FileAccessMode.ReadWrite);
                }
                catch (Exception e0)
                {
                    // TODO: Figure out how to effectivly throw an exception
                }
            }

            if (hidDevice != null)
            {
                deviceIsEnum = true;
                hidDevice.InputReportReceived += new TypedEventHandler<HidDevice, HidInputReportReceivedEventArgs>(this.InterruptHandler);
            }

            enumInProgress = false;
        }

        private void InterruptHandler(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            var data = args.Report;
        }
    }
}
