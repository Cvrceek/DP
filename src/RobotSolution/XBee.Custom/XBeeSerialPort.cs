using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XBee.Custom
{
    public class XBeeSerialPort : SerialPort
    {
        public event EventHandler<byte[]> XBeeDataReceived;

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

                  
                    byte lengthMSB = (byte)dataStack[1];
                    byte lengthLSB = (byte)dataStack[2];
                    int length = (lengthMSB << 8) | lengthLSB;

   
                    if (dataStack.Count < length + 3 + 1) 
                    {
              
                        return;
                    }

             
                    byte[] frameData = new byte[length];
                    for (int i = 0; i < length; i++)
                    {
                        frameData[i] = (byte)dataStack[i + 3];
                    }

         
                    byte checksum = (byte)dataStack[length + 3];

                    if (checksum != CalculateChecksum(frameData))
                    {
                        Debug.WriteLine("Chybný kontrolní součet.");
                        dataStack.RemoveRange(0, length + 4); 
                        continue;
                    }

                    XBeeDataReceived?.Invoke(this, frameData);
#if DEBUG
                    Debug.WriteLine($"Přijatý API rámec: {BitConverter.ToString(frameData)}");
                    Debug.WriteLine($"Data: {Encoding.ASCII.GetString(frameData, 5, frameData.Length - 5)}");
#endif

                    dataStack.RemoveRange(0, length + 4);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Chyba při zpracování dat: {ex.Message}");
            }


        }
        private byte CalculateChecksum(byte[] frameData)
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
