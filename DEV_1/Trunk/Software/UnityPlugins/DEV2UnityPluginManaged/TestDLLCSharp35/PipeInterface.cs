using System;
using System.IO.Pipes;

namespace VMUVUnityPlugin_NET35_v100
{
    static class PipeInterface
    {
        private static NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", "DEV_1Pipe", PipeDirection.In, PipeOptions.None);
        private const byte sync1 = 0xFF;
        private const byte sync2 = 0x5A;
        private const byte headerLen = 5;
        private const byte checkSumLen = 2;
        private const byte overHeadLen = headerLen + checkSumLen;
        private static byte[] rxBuff = new byte[255];

        public static void ConnectToServer()
        {
            if (!IsServerConnected())
            {
                try
                {
                    clientPipe.Connect();
                    Logger.LogMessage("Connected to server!");
                }
                catch (Exception e)
                {
                    DEV2ExceptionHandler.TakeActionOnException(e);
                }
            }
        }

        public static bool IsServerConnected()
        {
            return clientPipe.IsConnected;
        }

        public static void ReadPacket()
        {
            try
            {
                if (IsServerConnected())
                {
                    if (RxStateMachine() != -1)
                        InterprocessComms.ActOnReadSuccess(ByteWiseUtilities.CopyVarLen(rxBuff, 18));
                    else
                        InterprocessComms.ActOnReadFail();
                }
            }
            catch (Exception e)
            {
                DEV2ExceptionHandler.TakeActionOnException(e);
            }
        }

        private static int RxStateMachine()
        {
            bool msgIsValid = false;
            ushort rxChkSum, calcChkSum;
            int msgIndex = 0;
            int msgLen = 0;
            int msgType = 0;

            rxChkSum = calcChkSum = 0;

            try
            {
                while (!msgIsValid)
                {
                    rxBuff[msgIndex++] = (byte)clientPipe.ReadByte();

                    if (rxBuff[0] != sync1)
                        msgIndex = 0;

                    if (msgIndex > 1 && rxBuff[1] != sync2)
                        msgIndex = 0;

                    if (msgIndex > 2 && rxBuff[2] != 0x00)
                        msgIndex = 0;

                    if (msgIndex > 3)
                        msgType = rxBuff[3];

                    if (msgIndex > 4)
                    {
                        msgLen = rxBuff[4];
                        clientPipe.Read(rxBuff, 0, msgLen);

                        for (int i = 0; i < msgLen; i++)
                            calcChkSum += rxBuff[i];

                        rxChkSum = (byte)clientPipe.ReadByte();
                        rxChkSum <<= 8;
                        rxChkSum |= (byte)clientPipe.ReadByte();

                        if (rxChkSum == calcChkSum)
                            msgIsValid = true;
                        else
                            return -1;
                    }
                }
            }
            catch (Exception e)
            {
                DEV2ExceptionHandler.TakeActionOnException(e);
            }

            return msgType;
        }
    }
}
