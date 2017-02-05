using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace DEV_1Client
{
    class DeviceManager
    {
        private static Device dev_1 = new Device();
        private static DeviceData currentDeviceData = new DeviceData();
        private static UpdaterService updaterService = null;
        private static HidDevice hidDevice = null;
        private static bool deviceIsEnum = false;
        private static bool enumInProgress = false;

        public DeviceManager(UpdaterService updater)
        {
            updaterService = updater;
        }

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
                hidDevice.InputReportReceived += new TypedEventHandler<HidDevice, 
                    HidInputReportReceivedEventArgs>(this.InterruptHandler);
                UpdateAfterEvent(UpdaterEvents.event_device_enumeration_complete);
            }

            enumInProgress = false;
        }

        private void InterruptHandler(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            HidInputReport inputReport = args.Report;
            IBuffer buffer = inputReport.Data;
            DataReader dr = DataReader.FromBuffer(buffer);
            byte[] bytes = new byte[inputReport.Data.Length];
            dr.ReadBytes(bytes);
            currentDeviceData.SetRawDataInCnts(bytes);
            UpdateAfterEvent(UpdaterEvents.event_data_received);
        }

        private void UpdateAfterEvent(UpdaterEvents evt)
        {
            if (updaterService == null)
                return;

            try
            {
                switch (evt)
                {
                    case UpdaterEvents.event_device_enumeration_complete:
                        updaterService.OnDeviceEnumerationComplete();
                        break;
                    case UpdaterEvents.event_data_received:
                        updaterService.OnDataReceived(currentDeviceData);
                        break;
                }
            }
            catch (Exception e0)
            {
                // TODO: pass this back to the caller if it fails
            }
        }

        enum UpdaterEvents
        {
            event_device_enumeration_complete,
            event_data_received
        };
    }
}
