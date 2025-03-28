using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.DTO.DTOModels
{
    public class ExternalDeviceHolderValues : IDTOModel<ExternalDeviceHolderValues>
    {
        public ExternalDeviceHolderValues()
        {
        }

        public EDTOType EDTOType => throw new NotImplementedException();

        public static ExternalDeviceHolderValues FromBytes(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}
