using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.DTO.DTOModels
{
    public class ExternalDeviceMotorValues : IDTOModel<ExternalDeviceMotorValues>
    {
        public EDTOType EDTOType => throw new NotImplementedException();

        public static ExternalDeviceMotorValues FromBytes(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}
