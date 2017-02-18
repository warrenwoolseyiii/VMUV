using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Pipes;
using System;
using DEV_1ClientConsole;

namespace TestPipeClient
{
    class DEV_1
    {
        private DeviceData data = new DeviceData();
        private NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", "DEV_1Pipe", PipeDirection.In, PipeOptions.None);
        private bool dataIsValid = true;

        public void ConnectToClientService()
        {
            try
            {
                if (!clientPipe.IsConnected)
                    clientPipe.Connect();
            }
            catch (Exception e0)
            {
                ExceptionHandler eHandle = new ExceptionHandler(e0);
                eHandle.TakeActionOnException();
            }
        }

        public void ReadPacket()
        {
            try
            {
                if (clientPipe.IsConnected)
                {
                    int len = clientPipe.ReadByte() * 256;
                    len += clientPipe.ReadByte();
                    byte[] buff = new byte[len];
                    clientPipe.Read(buff, 0, len);
                    data.SetRawDataInBytes(buff);
                    int chkSum = clientPipe.ReadByte() * 256;
                    chkSum += clientPipe.ReadByte();
                }
            }
            catch (Exception e0)
            {
                ExceptionHandler eHandle = new ExceptionHandler(e0);
                eHandle.TakeActionOnException();
            }
        }

        public bool IsPacketValid()
        {
            return dataIsValid;
        }

        public Int16[] GetDataInInts()
        {
            return data.GetRawDataInCnts();
        }

        public String GetDataInString()
        {
            return data.ToStringRawDisplayFormat();
        }
    }
}
