using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace RobotLibs.DTO.DTOModels
{
    public class PWMValues : IDTOModel<PWMValues>
    {
        //1+4 = 5 bytes
        public EDTOType EDTOType { get; set; }
        public int Position { get; set; }

        public static PWMValues FromBytes(byte[] data)
        {
            return new PWMValues
            {
                EDTOType = (EDTOType)BitConverter.ToInt32(data, 0),
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
