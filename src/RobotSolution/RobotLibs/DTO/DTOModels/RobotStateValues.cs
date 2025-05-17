using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.DTO.DTOModels
{
    public class RobotStateValues : IDTOModel<RobotStateValues>
    {
        public EDTOType EDTOType
        {
            get { return EDTOType.RobotStateValues; }
        }

        public double BatteryVoltage { get; set; }
        public int HolderServoVoltage { get; set; }

        public static RobotStateValues FromBytes(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}
