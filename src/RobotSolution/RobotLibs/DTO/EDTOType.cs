using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.DTO
{
    public enum EDTOType : byte 
    {
        MainMotorsValues = 1,
        ExternalDeviceHolderValues = 2,
        ExternalDeviceMotorValues = 3,
        RobotStateValues = 4,
        ExternalDevicePWM1 = 5,
        ExternalDevicePWM2 = 6,
        LedRamp = 7,
        Horn = 8,
        Beacon = 9
    }
}
