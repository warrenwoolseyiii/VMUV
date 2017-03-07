using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace DEV_1ClientConsole
{
    static class HardwareInterface
    {
        private static bool deviceIsEnumerated = false;
        private static HidDevice device = null;

        public static bool GetEnumerationStatus()
        {
            return deviceIsEnumerated;
        }

        public static async Task EnumerateDevice()
        {
            if (!GetEnumerationStatus())
            {
                try
                {
                    var deviceInfo = await DeviceInformation.FindAllAsync(USBSelector.GetSelector());

                    if (deviceInfo.Count > 0)
                        device = await HidDevice.FromIdAsync(deviceInfo.ElementAt(0).Id, Windows.Storage.FileAccessMode.ReadWrite);
                }
                catch (Exception e)
                {
                    ExceptionHandler.TakeActionOnException(e);
                }

                if (device != null)
                {
                    deviceIsEnumerated = true;
                    device.InputReportReceived += new TypedEventHandler<HidDevice, HidInputReportReceivedEventArgs>(USBInterruptTransferHandler);
                    Console.WriteLine("Enumeration Complete!");
                }
            }
        }

        private static void USBInterruptTransferHandler(HidDevice sender, HidInputReportReceivedEventArgs args)
        {
            // TODO: For now we only have one report so we can get away with not parsing the type or numeric control
            HidInputReport rpt = args.Report;
            IBuffer buff = rpt.Data;
            DataReader dr = DataReader.FromBuffer(buff);
            byte[] bytes = new byte[rpt.Data.Length];
            dr.ReadBytes(bytes);
            USBPacketContainer packet = new USBPacketContainer(bytes);
            USBPacketManager.ParseNextPacket(packet);
        }
    }
}
