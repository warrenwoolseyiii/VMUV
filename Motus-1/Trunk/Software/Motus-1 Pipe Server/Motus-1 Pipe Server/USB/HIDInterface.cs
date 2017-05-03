using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.Storage.Streams;
using Motus_1_Pipe_Server.Logging;
using Motus_1_Pipe_Server.Utilities;
using Motus_1_Pipe_Server.DataStorage;

namespace Motus_1_Pipe_Server.USB
{
    static class HIDInterface
    {
        private static bool deviceIsEnumerated = false;
        private static bool deviceIsPresent = false;
        private static HidDevice device = null;
        private static TraceLogger hidLogger = new TraceLogger(128);
        private static string moduleName = "HIDInterface.cs";

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
            string methodName = "FindDevice";

            deviceIsPresent = false;

            try
            {
                var deviceInfo = await DeviceInformation.FindAllAsync(USBSelector.GetSelector());

                if (deviceInfo.Count > 0)
                {
                    deviceIsPresent = true;
                    hidLogger.QueueMessage(hidLogger.BuildMessage(moduleName, methodName, "Motus-1 is present!"));
                }
            }
            catch (Exception e0)
            {
                string message = "An exception of type " + e0.GetType().ToString() + " occurred." +
                    " Exception occurred at : " + Environment.StackTrace + ". Exception message is : " + e0.Message;
                hidLogger.QueueMessage(hidLogger.BuildMessage(moduleName, methodName, message));
            }
        }

        public static async Task PollDevice()
        {
            string methodName = "PollDevice";
            try
            {
                var deviceInfo = await DeviceInformation.FindAllAsync(USBSelector.GetSelector());

                if (deviceInfo.Count == 0)
                {
                    deviceIsPresent = false;
                    hidLogger.QueueMessage(hidLogger.BuildMessage(moduleName, methodName, "Motus-1 has been disconnected"));
                }
            }
            catch (Exception e0)
            {
                string message = "An exception of type " + e0.GetType().ToString() + " occurred." +
                    " Exception occurred at : " + Environment.StackTrace + ". Exception message is : " + e0.Message;
                hidLogger.QueueMessage(hidLogger.BuildMessage(moduleName, methodName, message));
            }
        }

        public static async Task EnumerateDevice()
        {
            string methodName = "EnumerateDevice";
            deviceIsEnumerated = false;

            if (!DeviceIsEnumerated() && DeviceIsPresent())
            {
                try
                {
                    var deviceInfo = await DeviceInformation.FindAllAsync(USBSelector.GetSelector());
                    device = await HidDevice.FromIdAsync(deviceInfo.ElementAt(0).Id, Windows.Storage.FileAccessMode.ReadWrite);
                }
                catch (Exception e0)
                {
                    string message = "An exception of type " + e0.GetType().ToString() + " occurred." +
                        " Exception occurred at : " + Environment.StackTrace + ". Exception message is : " + e0.Message;
                    hidLogger.QueueMessage(hidLogger.BuildMessage(moduleName, methodName, message));
                }

                if (device != null)
                {
                    deviceIsEnumerated = true;
                    device.InputReportReceived += new TypedEventHandler<HidDevice, HidInputReportReceivedEventArgs>(USBInterruptTransferHandler);
                    hidLogger.QueueMessage(hidLogger.BuildMessage(moduleName, methodName, "Motus-1 enumeration success!"));
                }
                else
                {
                    hidLogger.QueueMessage(hidLogger.BuildMessage(moduleName, methodName, "Motus-1 enumeration failure."));
                }
            }
        }

        public static void DisposeDevice()
        {
            deviceIsEnumerated = false;
            device.Dispose();
        }

        private static byte[] GetHidReport(HidInputReportReceivedEventArgs args)
        {
            // TODO: For now we only have one report so we can get away with not parsing the type or numeric control
            HidInputReport rpt = args.Report;
            IBuffer buff = rpt.Data;
            DataReader dr = DataReader.FromBuffer(buff);
            byte[] bytes = new byte[rpt.Data.Length];
            dr.ReadBytes(bytes);
            DataStorageTable.SetCurrentData(bytes);

            return bytes;
        }

        private static void USBInterruptTransferHandler(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            byte[] bytes = GetHidReport(args);

            if (Logger.IsLoggingRawData())
            {
                ByteUtilities.SetRawDataInCnts(bytes);
                Logger.LogRawData(ByteUtilities.ToCsvFormat());
            }
        }

        public static TraceLoggerMessage[] GetTraceMessages()
        {
            return hidLogger.GetAllMessages();
        }

        public static bool HasTraceMessages()
        {
            return hidLogger.HasMessages();
        }
    }
}
