using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.XbeeCustom
{
    public class XbeeFrame
    {
        public byte StartDelimiter { get; set; }
        public byte MSB { get; set; }
        public byte LSB { get; set; }
        public int Length { get; set; }
        public byte FrameType { get; set; }
        public List<byte> Data { get; set; }
        public byte Checksum { get; set; }
        public List<byte> RFData
        {
            get { return Data.Skip(5).ToList(); }
        }

        public bool DeliverySuccessful => FrameType == 0x8B && Data.Count >= 6 && Data[5] == 0x00;
    }
}
