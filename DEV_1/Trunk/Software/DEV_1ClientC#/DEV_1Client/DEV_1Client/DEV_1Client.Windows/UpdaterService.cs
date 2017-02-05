using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV_1Client
{
    class UpdaterService
    {
        MainPage parent = null;

        public UpdaterService(MainPage mainPage)
        {
            parent = mainPage;
        }

        public void OnDeviceEnumerationComplete()
        {
            if (parent == null)
                return;
            parent.SetEnumerationStatusText("DEV1 Enumeration Success!");
        }

        public void OnDataReceived(DeviceData data)
        {

        }
    }
}
