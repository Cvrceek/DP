using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCommander.Inputs
{
    public interface IInputDevice
    {
        event EventHandler<int> SpeedChanged;
        event EventHandler<int> DirectionChanged;
        
        event EventHandler<int> ExternalDeviceSpeedChanged;
        event EventHandler<int> ExternalDevicePWM1Changed;
        event EventHandler<int> ExternalDevicePWM2Changed;
       
        event EventHandler<int> ExternalDeviceHolderPositionChanged;
        
        event EventHandler<bool> LightsChanged;
        event EventHandler<bool> BeaconChanged;
        event EventHandler<bool> HornChanged;

        public void Run();

    }
}
