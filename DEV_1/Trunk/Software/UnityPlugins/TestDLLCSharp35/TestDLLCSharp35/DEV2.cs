using System;
using System.Diagnostics;
using System.IO.Pipes;
using UnityEngine;
using System.IO;

namespace TestDLLCSharp35
{
    public class DEV2
    {
        private DEV2DeviceData dataPing, dataPong;
        private NamedPipeClientStream clientPipe;
        private DEV2VRMotionFusion motionFusion;
        Process clientProcess;
        byte[] dataBytes;
        private bool pingActive, asyncReadComplete, initSuccess, clientExeLaunched;
        private int dataCnt;
        private string dev2ClientLocation = Path.Combine(Environment.CurrentDirectory, "Assets\\Plugins\\DEV_1ClientConsole");

        public void OnUpdate(Vector3 lHand, Vector3 rHand)
        {
            if (initSuccess)
            {
                if (asyncReadComplete)
                    ReadAsync();

                motionFusion.CalculateTranslationAndStaffe(GetDataInCnts(), lHand, rHand);
            }
            else
            {
                if (!clientExeLaunched)
                {
                    clientExeLaunched = LaunchDEV2Client();
                }
                else
                {
                    initSuccess = ConnectToDEV2Client();
                    ReadAsync();
                }
            }
        }

        public void OnAppQuit()
        {
            if (initSuccess)
            {
                while (!asyncReadComplete) ;
            }
        }

        public void OnStart()
        {
            initSuccess = false;
            clientExeLaunched = false;
            pingActive = true;
            asyncReadComplete = false;
            dataCnt = -1;
            dataBytes = new byte[18];
            motionFusion = new DEV2VRMotionFusion();
            dataPing = new DEV2DeviceData();
            dataPong = new DEV2DeviceData();
            clientPipe = new NamedPipeClientStream(".", "DEV_1Pipe", PipeDirection.In, PipeOptions.None);
            LaunchDEV2Client();

            if (clientExeLaunched)
            {
                initSuccess = ConnectToDEV2Client();
                if (initSuccess)
                    ReadAsync();
            }
        }

        private bool LaunchDEV2Client()
        {
            try
            {
                clientProcess = new Process();
                clientProcess.StartInfo.FileName = dev2ClientLocation;
                clientProcess.Start();
                clientExeLaunched = true;
                return true;
            }
            catch (Exception e0)
            {
                DEV2ExceptionHandler eHandle = new DEV2ExceptionHandler();
                eHandle.TakeActionOnException();
                return false;
            }
        }

        private bool ConnectToDEV2Client()
        {
            try
            {
                if (!clientPipe.IsConnected)
                    clientPipe.Connect();
                return true;
            }
            catch (Exception e0)
            {
                DEV2ExceptionHandler eHandle = new DEV2ExceptionHandler(e0);
                eHandle.TakeActionOnException();
                return false;
            }
        }

        private void ReadAsync()
        {
            try
            {
                if (clientPipe.IsConnected)
                {
                    asyncReadComplete = false;
                    clientPipe.BeginRead(dataBytes, 0, 18, AsyncReadCallBack, null);
                }
            }
            catch (Exception e0)
            {
                DEV2ExceptionHandler eHandle = new DEV2ExceptionHandler(e0);
                eHandle.TakeActionOnException();
            }
        }

        private void AsyncReadCallBack(IAsyncResult ar)
        {
            dataCnt = 0;

            try
            {
                dataCnt = clientPipe.EndRead(ar);
            }
            catch (Exception e0)
            {
                DEV2ExceptionHandler eHandle = new DEV2ExceptionHandler(e0);
                eHandle.TakeActionOnException();
            }

            if (dataCnt > 0)
            {
                SetDataInPingPong(dataBytes);
                asyncReadComplete = true;
            }
        }

        private void SetDataInPingPong(byte[] data)
        {
            if (pingActive)
            {
                dataPing.SetRawDataInBytes(data);
                pingActive = false;
            }
            else
            {
                dataPong.SetRawDataInBytes(data);
                pingActive = true;
            }
        }

        private Int16[] GetDataInCnts()
        {
            if (pingActive)
                return dataPong.GetRawDataInCnts();
            else
                return dataPing.GetRawDataInCnts();
        }

        public string GetDataInString()
        {
            if (pingActive)
                return dataPong.ToStringRawDisplayFormat();
            else
                return dataPing.ToStringRawDisplayFormat();
        }

        public float GetTranslationFromDEV2()
        {
            return motionFusion.GetTranslation();
        }

        public float GetStraffeFromDEV2()
        {
            return motionFusion.GetStraffe();
        }
    }
}
