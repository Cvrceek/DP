using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.XbeeCustom
{
    public class XBeeSerialPort : SerialPort
    {
        public event EventHandler<XbeeFrame> XBeeDataReceived;
        private List<byte> dataStack = new List<byte>();
        public XBeeSerialPort(string portName, int boudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, Handshake handshake = Handshake.None) : base(portName, boudRate)
        {
            DataBits = dataBits;
            Parity = parity;
            StopBits = stopBits;
            Handshake = handshake;
            Encoding = Encoding.ASCII;
            SetupEventsHandlers();
        }

        private void SetupEventsHandlers()
        {
            DataReceived += this.OnDataReceived;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                byte[] buffer = new byte[this.BytesToRead];
                this.Read(buffer, 0, buffer.Length);

                dataStack.AddRange(buffer);

                while (dataStack.Count > 0)
                {

                    int startIndex = dataStack.IndexOf(0x7E);
                    if (startIndex == -1)
                    {
                        return;
                    }

                    if (startIndex > 0)
                    {
                        dataStack.RemoveRange(0, startIndex);
                    }

                    if (dataStack.Count < 3)
                    {
                        return;
                    }


                    byte lengthMSB = dataStack[1];
                    byte lengthLSB = dataStack[2];
                    int length = lengthMSB << 8 | lengthLSB;

                    if (dataStack.Count < length + 4)
                    {
                        return;
                    }


                    List<byte> frameData = dataStack.Skip(3).Take(length).ToList();
                  


                    byte checksum = dataStack[length + 3];

                    if (checksum != CalculateChecksum(frameData))
                    {
                        Console.WriteLine("Chybný kontrolní součet.");
                        dataStack.RemoveRange(0, length + 4);
                        continue;
                    }
               
                    var frame = new XbeeFrame
                    {
                        StartDelimiter = dataStack[0],
                        MSB = dataStack[1],
                        LSB = dataStack[2],
                        Length = length,
                        Data = frameData,
                        FrameType = dataStack[3],
                        Checksum = dataStack[3 + length]
                    };

                    XBeeDataReceived?.Invoke(this, frame);

                    
                    dataStack.RemoveRange(0, length + 4);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Chyba při zpracování dat: {ex.Message}");
            }
        }
        private byte CalculateChecksum(List<byte> frameData)
        {
            int sum = 0;
            foreach (byte b in frameData)
            {
                sum += b;
            }
            return (byte)(0xFF - (sum & 0xFF));
        }
    }
}
