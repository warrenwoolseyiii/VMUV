using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;

namespace DEV_1Client
{
    class DeviceManager
    {
        private static Device dev_1 = new Device();
        private static HidDevice hidDevice = null;
        private static bool isEnum = false;
        private static bool enumInProgress = false;

        public bool IsEnumerated()
        {
            return isEnum;
        }

        public bool EnumerationInProgress()
        {
            return enumInProgress;
        }

        public async Task EnumerateDEV_1()
        {
            enumInProgress = true;

            if (!isEnum)
            {
                try
                {
                    var deviceInfo = await DeviceInformation.FindAllAsync(dev_1.GetSelector());

                    if (deviceInfo.Count > 0)
                    {
                        hidDevice = await HidDevice.FromIdAsync(deviceInfo.ElementAt(0).Id, FileAccessMode.ReadWrite);
                    }
                }
                catch (Exception e0)
                {
                    // TODO: Figure out how to effectivly throw an exception
                }
            }

            if (hidDevice != null)
                isEnum = true;

            enumInProgress = false;
        }
    }
}
