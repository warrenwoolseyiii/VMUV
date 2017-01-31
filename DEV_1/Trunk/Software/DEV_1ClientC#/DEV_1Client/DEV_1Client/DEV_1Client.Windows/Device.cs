using Windows.Devices.HumanInterfaceDevice;

namespace DEV_1Client
{
    class Device
    {
        private static ushort vendorId, productId, usagePage, usage;

        public Device()
        {
            vendorId = 0x6969;
            productId = 0x0002;
            usagePage = 0x0003;
            usage = 0x0002;
        }

        public string GetSelector()
        {
            return (HidDevice.GetDeviceSelector(usagePage, usage, vendorId, productId));
        }
    }
}
