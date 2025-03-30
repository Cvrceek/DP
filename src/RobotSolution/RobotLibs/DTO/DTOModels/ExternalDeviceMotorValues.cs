using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.DTO.DTOModels
{
    public class ExternalDeviceMotorValues : IDTOModel<ExternalDeviceMotorValues>
    {
        //1+1+4= 6bytes
        public EDTOType EDTOType
        { 
            get { return EDTOType.ExternalDeviceMotorValues; } 
        }

        public int Speed = 0;
        public byte Orientation = 0;
        public static ExternalDeviceMotorValues FromBytes(byte[] data)
        {
            return new ExternalDeviceMotorValues
            {
                Speed = BitConverter.ToInt32(data, 1),
                Orientation = data[5]
            };
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.Add((byte)EDTOType);
            bytes.AddRange(BitConverter.GetBytes(Speed));
            bytes.Add(Orientation);
            return bytes.ToArray();
        }
    }
}
