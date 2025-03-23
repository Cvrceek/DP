using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace RobotCommander.Settings
{
    //https://learn.microsoft.com/en-us/windows/win32/cimwin32prov/win32-pnpentity#properties
    public static class ExternalDevices
    {
        public static List<(string Name, string DeviceID, string Manufacturer)> GetCameras()
        {
            var cameraList = new List<(string, string, string)>();

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PNPClass = 'Image' OR PNPClass = 'Camera'"))
            {
                foreach (var device in searcher.Get())
                {
                    string name = device["Caption"]?.ToString() ?? ResX.Strings.ExternalDevices_NoName;
                    string deviceId = device["DeviceID"]?.ToString() ?? ResX.Strings.ExternalDevices_NoID;
                    string manufacturer = device["Manufacturer"]?.ToString() ?? ResX.Strings.ExternalDevices_Manufacturer;
                    cameraList.Add((name, deviceId, manufacturer));
                }
            }
            
            return cameraList;
        }

        public static List<(string Port, string Name, string Manufacturer)> GetSerialPorts()
        {
            var portList = new List<(string, string, string)>();

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PNPClass='Ports'"))
            {
                foreach (var device in searcher.Get())
                {
                    string name = device["Caption"]?.ToString() ?? ResX.Strings.ExternalDevices_NoName;

                    if (!name.Contains("(COM"))
                        continue;
                    string deviceID = device["DeviceID"]?.ToString() ?? ResX.Strings.ExternalDevices_NoID;
                    string manufacturer = device["Manufacturer"]?.ToString() ?? ResX.Strings.ExternalDevices_Manufacturer;

                    string port = name.Substring(name.LastIndexOf("(COM")).Replace("(", "").Replace(")", "");
               
                    portList.Add((port, name, manufacturer));
                }
            }

            return portList;
        }

        public class Device
        {
            public string Caption { get; set; }
            public string DeviceID { get; set; }
            public string Name { get; set; }
            public string Manufacturer { get; set; }
        }
    }
}
