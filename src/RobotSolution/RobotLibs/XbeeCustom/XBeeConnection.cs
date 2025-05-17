using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RobotLibs.XbeeCustom
{
    public class XBeeConnection
    {
        private readonly object serialLock = new object();
        private XBeeSerialPort serialPort;
        public event EventHandler<XbeeFrame> TXFrameReceived;
        public event EventHandler<XbeeFrame> DeliveryStatusFrameReceived;

        private byte[] remoteXbeeAddr = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF };
        private string SH;
        private string SL;



        private const int _STARTDELIMITER_ = 0x7E;
        private readonly List<byte> dataStack = new();
        public XBeeConnection(string portName, string sl, string sh, int baudRate = 9600)
        {
            serialPort = new XBeeSerialPort(portName, baudRate);
            SL = sl;
            SH = sh;
            LoadAddress();

            serialPort.XBeeDataReceived += (s, e) =>
            {
                //udělat zpracování dat (různé xbee frame types
                if (e.FrameType == 0x8B)
                    DeliveryStatusFrameReceived?.Invoke(this, e);
                if (e.FrameType == 0x90)
                    TXFrameReceived?.Invoke(this, e);
            };
        }

        private void LoadAddress()
        {
            string textAddress = SH + SL;
            var address = Enumerable.Range(0, 16)
           .Where(x => x % 2 == 0)
           .Select(x => Convert.ToByte(textAddress.Substring(x, 2), 16))
           .ToArray();

            remoteXbeeAddr = address;
        }

        // Otevření sériového portu
        public void Open()
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                    Console.WriteLine("Port otevřen.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Chyba při otevírání portu: {e.Message}");
                }
            }
        }

        // Zavření sériového portu
        public void Close()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                Console.WriteLine("Port uzavřen.");
            }
        }


        public void SendAPIMessage(byte[] data)
        {
            lock (serialLock)
            {
                byte frameType = 0x10;  // Zigbee Transmit Request
                byte frameID = 0x01;    // Frame ID for ACK
                byte[] reserved = { 0xFF, 0xFE };  // Reserved bytes (for broadcast)
                byte broadcastRadius = 0x00;  // Maximum number of hops
                byte options = 0x00;  // Disable ACKs and Route Discovery

                List<byte> frame = new List<byte>
                {
                    0x7E,  // Start delimiter
                };

                int length = 14 + data.Length;
                frame.Add((byte)(length >> 8));  // MSB of length
                frame.Add((byte)(length & 0xFF));  // LSB of length

                frame.Add(frameType);
                frame.Add(frameID);

                // 64-bit destination address
                frame.AddRange(remoteXbeeAddr);

                // Reserved
                frame.AddRange(reserved);

                frame.Add(broadcastRadius);
                frame.Add(options);

                // Data to send
                frame.AddRange(data);

                // Calculate checksum
                byte checksum = 0xFF;
                for (int i = 3; i < frame.Count; i++)
                {
                    checksum -= frame[i];
                }
                frame.Add(checksum);


                if (serialPort.IsOpen)
                {
                    try
                    {
                        serialPort.Write(frame.ToArray(), 0, frame.Count);
                        Console.WriteLine("API rámec odeslán.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Chyba při odesílání API rámce: {e.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Sériový port není otevřen.");
                }
            }
        }

        //public void ProcessTXStatusFrame(byte[] frameData)
        //{
        //    lock (serialLock)
        //    {
        //        if (frameData[0] == 0x8B)
        //        {
        //            byte frameID = frameData[1];
        //            byte deliveryStatus = frameData[5];

        //            if (deliveryStatus == 0x00)
        //            {
        //                Console.WriteLine($"Zpráva s Frame ID {frameID} byla úspěšně doručena.");
        //            }
        //            else
        //            {
        //                Console.WriteLine($"Zpráva s Frame ID {frameID} nebyla doručena. Stav: {deliveryStatus:X2}");
        //            }
        //        }
        //    }
        //}

        //public void ProcessReceivedMessage(byte[] frameData)
        //{
        //    lock (serialLock)
        //    {
        //        if (frameData == null || frameData.Length < 15)
        //        {
        //            Console.WriteLine("Neplatný rámec.");
        //            return;
        //        }

        //        // Zkontrolujeme, zda se jedná o RX Packet (Frame Type 0x90)
        //        if (frameData[0] == 0x90)
        //        {
        //            // Čteme 64-bitovou adresu zdroje
        //            byte[] sourceAddress = new byte[8];
        //            Array.Copy(frameData, 1, sourceAddress, 0, 8);

        //            // Přijatá RF data
        //            int rfDataLength = frameData.Length - 12; // 12 je počet bajtů před RF daty
        //            byte[] rfData = new byte[rfDataLength];
        //            Array.Copy(frameData, 12, rfData, 0, rfDataLength);

        //            // Zobrazíme přijatá data
        //            string receivedMessage = Encoding.ASCII.GetString(rfData);
        //            Console.WriteLine($"Přijatá zpráva od {BitConverter.ToString(sourceAddress)}: {receivedMessage}");
        //        }
        //        else
        //        {
        //            Console.WriteLine("Přijatý rámec není RX (Receive Packet).");
        //        }
        //    }
        //}
    }
}
