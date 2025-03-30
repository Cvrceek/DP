using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace RobotLibs.DTO.DTOModels
{
    public class ExternalDeviceHolderValues : IDTOModel<ExternalDeviceHolderValues>
    {
        //1+4 = 5 bytes
        public EDTOType EDTOType
        {
            get { return EDTOType.ExternalDeviceHolderValues;}
        }
        public int Position { get; set; }

        public static ExternalDeviceHolderValues FromBytes(byte[] data)
        {
            return new ExternalDeviceHolderValues
            {
                Position = BitConverter.ToInt32(data, 1),
            };
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.Add((byte)EDTOType);
            bytes.AddRange(BitConverter.GetBytes(Position));
            return bytes.ToArray();
        }
    }
}
