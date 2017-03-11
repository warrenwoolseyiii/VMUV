
namespace DEV_1ClientConsole
{
    static class USBPacketManager
    {
        private const ushort padInputRptLen = 9;
        private const ushort accumLimit = 10;
        private static ushort[] padValueAccum = new ushort[padInputRptLen];
        private static ushort accumCntr = 0;

        public static int GetPadDataLen()
        {
            return padInputRptLen;
        }

        public static void ParseNextPacket(USBPacketContainer packet)
        {
            switch (packet.GetType())
            {
                case USBPacketContainer.Types.packet_type_pad_report:
                    HandlePadValuePacket(packet);
                    break;
            }
        }

        private static void HandlePadValuePacket(USBPacketContainer packet)
        {
            ushort[] addend = ByteWiseUtilities.ConvertBytesToUShortLittleE(packet.GetData(), 1);

            //Logger.LogMessage("Read Values\n" + ByteWiseUtilities.UShortToString(addend));
            padValueAccum = ByteWiseUtilities.AccumulateUShorts(padValueAccum, addend);
            accumCntr++;

            if (accumCntr >= accumLimit)
            {
                CurrentValueTable.SetPadData(ByteWiseUtilities.AverageUShorts(padValueAccum, accumCntr));
                padValueAccum = ByteWiseUtilities.ClearAccumulation(padValueAccum);
                accumCntr = 0;
            }
        }
    }
}
