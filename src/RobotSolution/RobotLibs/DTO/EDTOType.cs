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
        RobotStateValues = 4
    }
}
