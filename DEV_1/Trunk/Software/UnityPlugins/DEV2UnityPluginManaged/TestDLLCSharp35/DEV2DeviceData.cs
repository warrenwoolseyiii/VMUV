
namespace VMUVUnityPlugin_NET35_v100
{
    class DEV2DeviceData
    {
        const ushort rawDataLengthInBytes = 18;
        const ushort rawDataLengthInInts = rawDataLengthInBytes / 2;
        private ushort[] rawDataInCnts = new ushort[rawDataLengthInInts];

        public DEV2DeviceData()
        {

        }

        public DEV2DeviceData(byte[] bytes)
        {
            SetRawData(bytes);
        }

        public void SetRawData(byte[] bytes)
        {
            rawDataInCnts = ByteWiseUtilities.ConvertBytesToUShortBigE(bytes, 0);
        }

        public byte[] GetRawDataInBytes()
        {
            return ByteWiseUtilities.ConvertUShortToBytesBigE(rawDataInCnts);
        }

        public ushort[] GetRawDataInCnts()
        {
            return rawDataInCnts;
        }
    }
}
