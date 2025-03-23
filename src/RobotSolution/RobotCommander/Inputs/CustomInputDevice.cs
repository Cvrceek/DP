using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCommander.Inputs
{
    /// <summary>
    /// Předchystaná třída pro budoucí rozvoj
    /// </summary>
    public class CustomInputDevice : IRobotCommanderInput
    {
        public event EventHandler<int> SpeedChanged;
        public event EventHandler<int> DirectionChanged;
        public event EventHandler<int> ExternalDeviceSpeedChanged;
        public event EventHandler<int> ExternalDevicePWM1Changed;
        public event EventHandler<int> ExternalDevicePWM2Changed;
        public event EventHandler<int> ExternalDeviceHolderPositionChanged;
        public event EventHandler<bool> LightsChanged;
        public event EventHandler<bool> BeaconChanged;
        public event EventHandler<bool> HornChanged;
    }
}
