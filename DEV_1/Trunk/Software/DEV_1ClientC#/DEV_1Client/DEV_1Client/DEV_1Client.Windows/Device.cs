using Windows.Devices.HumanInterfaceDevice;

namespace DEV_1Client
{
    class Device
    {
        private static ushort vendorId, productId, usagePage, usage;

        public Device()
        {
            vendorId = 0x6969;
            productId = 0x0001;
            usagePage = 0xFFFF;
            usage = 0x00FF;
        }

        public string GetSelector()
        {
            return (HidDevice.GetDeviceSelector(usagePage, usage, vendorId, productId));
        }
    }
}
