using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Pipes;

namespace TestPipeClient
{
    class Program
    {
        private static NamedPipeClientStream clientStream;

        static void Main(string[] args)
        {
            int cnt = 0;
            clientStream = new NamedPipeClientStream(".", "DEV_1Pipe", PipeDirection.In, PipeOptions.None);
            clientStream.Connect();

            Console.WriteLine("Connection success\n");
            while (true)
            {
                int len = 0;

                len = clientStream.ReadByte() * 256;
                len += clientStream.ReadByte();
                byte[] buff = new byte[len];
                clientStream.Read(buff, 0, len);
                Console.WriteLine("Received packet %d\n", cnt++);
            }
        }
    }
}
