using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.DTO.DTOModels
{
    public class SwitchValues : IDTOModel<SwitchValues>
    {
        //1+4 = 5 bytes
        public EDTOType EDTOType { get; set; }
        public bool ON { get; set; }

        public static SwitchValues FromBytes(byte[] data)
        {
            return new SwitchValues
            {
                EDTOType = (EDTOType)BitConverter.ToInt32(data, 0),
                ON = BitConverter.ToBoolean(data, 1),
            };
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.Add((byte)EDTOType);
            bytes.AddRange(BitConverter.GetBytes(ON));
            return bytes.ToArray();
        }
    }
}
