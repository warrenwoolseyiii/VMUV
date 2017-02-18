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

namespace DEV_1ClientConsole
{
    class DeviceManager
    {
        private static Device dev_1 = new Device();
        private static DeviceData currentDeviceData = new DeviceData();
        private static InterprocessComms comms = null;
        private static HidDevice hidDevice = null;
        private static bool deviceIsEnum = false;
        private static bool enumInProgress = false;

        public DeviceManager(InterprocessComms commMngr)
        {
            comms = commMngr;
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
                    ExceptionHandler eHandle = new ExceptionHandler(e0);
                    eHandle.TakeActionOnException();
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
            currentDeviceData.SetRawDataInBytes(bytes);
            UpdateAfterEvent(UpdaterEvents.event_data_received);
        }

        private void UpdateAfterEvent(UpdaterEvents evt)
        {
            switch (evt)
            {
                case UpdaterEvents.event_device_enumeration_complete:
                    Console.WriteLine("DEV_1 Enumeration Complete!\n");
                    break;
                case UpdaterEvents.event_data_received:
                    if (comms != null)
                        comms.WritePadData(currentDeviceData);
                    break;
            }
        }

        enum UpdaterEvents
        {
            event_device_enumeration_complete,
            event_data_received
        };
    }
}
