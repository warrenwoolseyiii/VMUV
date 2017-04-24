using System;
using System.Threading.Tasks;
using System.Threading;

namespace Motus_1_Pipe_Server
{
    class Program
    {
        private static HardwareStates hwState = HardwareStates.find_device;

        static void Main(string[] args)
        {
            Initialize();

            while (true)
            {
                Motus1HardwareMain();
                Thread.Sleep(2);
            }
        }

        static void Initialize()
        {
            string startTime = DateTime.Now.ToString("h:mm:ss tt");
            Logger.CreateLogFile();
            Logger.LogMessage("Motus-1 Pipe Server started at " + startTime);
        }

        static void Motus1HardwareMain()
        {
            switch (hwState)
            {
                case HardwareStates.find_device:
                    Thread.Sleep(1000);
                    FindDevice();

                    if (HIDInterface.DeviceIsPresent())
                        hwState = HardwareStates.enumerate_device;
                    break;
                case HardwareStates.enumerate_device:
                    EnumerateDevice();

                    if (HIDInterface.DeviceIsEnumerated())
                        hwState = HardwareStates.device_enumerated;
                    else
                        hwState = HardwareStates.find_device;
                    break;
                case HardwareStates.device_enumerated:
                    Thread.Sleep(1000);
                    PollDevice();

                    if (!HIDInterface.DeviceIsPresent())
                    {
                        HIDInterface.DisposeDevice();
                        hwState = HardwareStates.find_device;
                    }
                    break;
            }
        }

        static void FindDevice()
        {
            Task.Run(async () =>
            {
                await HIDInterface.FindDevice();
            }).GetAwaiter().GetResult();
        }

        static void PollDevice()
        {
            Task.Run(async () =>
            {
                await HIDInterface.PollDevice();
            }).GetAwaiter().GetResult();
        }

        static void EnumerateDevice()
        {
            Task.Run(async () =>
            {
                await HIDInterface.EnumerateDevice();
            }).GetAwaiter().GetResult();
        }

        static void TakeDown()
        {
            string endTime = DateTime.Now.ToString("h:mm:ss tt");
            Logger.LogMessage("Motus-1 Pipe Server ended at " + endTime);
        }

        public enum HardwareStates
        {
            find_device,
            enumerate_device,
            device_enumerated
        }
    }
}
