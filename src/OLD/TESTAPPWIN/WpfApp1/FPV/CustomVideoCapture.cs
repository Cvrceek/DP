using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Settings;

namespace WpfApp1.FPV
{
    public class CustomVideoCapture : VideoCapture
    {
        public CustomVideoCapture(string deviceID) : base(GetCamIndex(deviceID)) { }

        private static int GetCamIndex(string deviceID)
        {
            var availableCams = ExternalDevices.GetCameras();
            if (availableCams.Exists(x => x.DeviceID == deviceID))
            {
                return availableCams.FindIndex(x => x.DeviceID == deviceID);
            }
            else
            {
                throw new Exception("Cam not found");
            }
        }
    }
}
