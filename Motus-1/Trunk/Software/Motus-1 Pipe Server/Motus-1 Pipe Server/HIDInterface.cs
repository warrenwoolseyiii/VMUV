using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Motus_1_Pipe_Server
{
    static class HIDInterface
    {
        private static bool deviceIsEnumerated = false;
        private static bool deviceIsPresent = false;
        private static HidDevice device = null;

        public static bool DeviceIsEnumerated()
        {
            return deviceIsEnumerated;
        }

        public static bool DeviceIsPresent()
        {
            return deviceIsPresent;
        }

        public static async Task FindDevice()
        {
            deviceIsPresent = false;

            try
            {
                var deviceInfo = await DeviceInformation.FindAllAsync(USBSelector.GetSelector());

                if (deviceInfo.Count > 0)
                {
                    deviceIsPresent = true;
                    Logger.LogMessage("Motus-1 is present!");
                }
            }
            catch (Exception e0)
            {
                Logger.LogMessage("An exception of type " + e0.GetType().ToString() + " occurred.");
                Logger.LogMessage("Exception occurred at : " + Environment.StackTrace);
                Logger.LogMessage("Exception message is : " + e0.Message);
            }
        }

        public static async Task PollDevice()
        {
            try
            {
                var deviceInfo = await DeviceInformation.FindAllAsync(USBSelector.GetSelector());

                if (deviceInfo.Count == 0)
                {
                    deviceIsPresent = false;
                    Logger.LogMessage("Motus-1 has been disconnected.");
                }
            }
            catch (Exception e0)
            {
                Logger.LogMessage("An exception of type " + e0.GetType().ToString() + " occurred.");
                Logger.LogMessage("Exception occurred at : " + Environment.StackTrace);
                Logger.LogMessage("Exception message is : " + e0.Message);
            }
        }

        public static async Task EnumerateDevice()
        {
            deviceIsEnumerated = false;

            if (!DeviceIsEnumerated() && DeviceIsPresent())
            {
                try
                {
                    Logger.LogMessage("Attempting to enumerate device...");
                    var deviceInfo = await DeviceInformation.FindAllAsync(USBSelector.GetSelector());
                    device = await HidDevice.FromIdAsync(deviceInfo.ElementAt(0).Id, Windows.Storage.FileAccessMode.ReadWrite);
                }
                catch (Exception e0)
                {
                    Logger.LogMessage("An exception of type " + e0.GetType().ToString() + " occurred.");
                    Logger.LogMessage("Exception occurred at : " + Environment.StackTrace);
                    Logger.LogMessage("Exception message is : " + e0.Message);
                }

                if (device != null)
                {
                    deviceIsEnumerated = true;
                    device.InputReportReceived += new TypedEventHandler<HidDevice, HidInputReportReceivedEventArgs>(USBInterruptTransferHandler);
                    Logger.LogMessage("Motus-1 enumeration success!");
                }
                else
                {
                    Logger.LogMessage("Motus-1 enumeration failure.");
                }
            }
        }

        public static void DisposeDevice()
        {
            deviceIsEnumerated = false;
            device.Dispose();
        }

        private static void USBInterruptTransferHandler(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            // TODO: For now we only have one report so we can get away with not parsing the type or numeric control
            HidInputReport rpt = args.Report;
            IBuffer buff = rpt.Data;
            DataReader dr = DataReader.FromBuffer(buff);
            byte[] bytes = new byte[rpt.Data.Length];
            dr.ReadBytes(bytes);

            if (Logger.IsLoggingRawData())
            {
                ByteUtilities.SetRawDataInCnts(bytes);
                Logger.LogRawData(ByteUtilities.ToCsvFormat());
            }
        }
    }
}
