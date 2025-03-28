using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace RobotLibs.DTO.DTOModels
{
    public class MainMotorsValues : IDTOModel<MainMotorsValues>
    {
        //1 + 4*2 + 2 = 11 bytes  


        public int SpeedRight = 0;
        public int SpeedLeft = 0;
        public byte OrientationRight = 0;
        public byte OrientationLeft = 0;

        public EDTOType EDTOType
        {
            get { return EDTOType.MainMotorsValues; }
        }

        public static MainMotorsValues FromBytes(byte[] data)
        {
            return new MainMotorsValues
            {

                SpeedRight = BitConverter.ToInt32(data, 1),
                SpeedLeft = BitConverter.ToInt32(data, 5),
                OrientationRight = data[9],
                OrientationLeft = data[10]
            };
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.Add((byte)EDTOType);
            bytes.AddRange(BitConverter.GetBytes(SpeedRight));
            bytes.AddRange(BitConverter.GetBytes(SpeedLeft));
            bytes.Add(OrientationRight);
            bytes.Add(OrientationLeft);
            return bytes.ToArray();
        }
    }
}
