﻿using System;
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
        private XBeeSerialPort serialPort;
        public event EventHandler<XbeeFrame> FrameReceived;

        private byte[] remoteXbeeAddr = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF };
        private string SH;
        private string SL;



        private const int _STARTDELIMITER_ = 0x7E;
        private readonly List<byte> dataStack = new();
        public XBeeConnection(string portName, string sl, string sh, int baudRate = 9600)
        {
            serialPort = new XBeeSerialPort(portName, baudRate);
            serialPort.XBeeDataReceived += (s, e) =>
            {
                //udělat zpracování dat (různé xbee frame types
                FrameReceived?.Invoke(this, e); 
            };

            SL = sl;
            SH = sh;
            LoadAddress();
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
            //byte frameType = 0x10; // Frame type for Transmit Request

            //int length = 14 + data.Length; // Length of the frame

            //byte[] frame = new byte[length + 3]; // +3 for start delimiter, length, and checksum
            //frame[0] = 0x7E; // Start delimiter
            //frame[1] = (byte)((length >> 8) & 0xFF); // Length MSB
            //frame[2] = (byte)(length & 0xFF); // Length LSB

            //// Frame data
            //frame[3] = frameType;
            //frame[4] = frameID;
            //Array.Copy(destinationAddress, 0, frame, 5, 12); // 64-bit destination address
            //frame[13] = 0xFF; // Network address MSB
            //frame[14] = 0xFE; // Network address LSB
            //frame[15] = 0x00; // Broadcast radius
            //frame[16] = 0x00; // Options

            //Array.Copy(data, 0, frame, 17, data.Length); // Copy data

            //// Calculate checksum

            //byte[] checkSumPart = new byte[frame.Length - 4]; 
            //Array.Copy(frame, 3, checkSumPart, 0, frame.Length - 4);

            //byte checksum = CalculateChecksum(checkSumPart);
            //frame[frame.Length - 1] = checksum;

            //byte[] frame = new byte[] { 0x7E, 0x00, 0x09, 0x01, 0x01, 0xFF, 0xFE, 0x00, 0x66, 0x66, 0x66, 0x66, 0x68 };

            byte frameType = 0x10;  // Zigbee Transmit Request
            byte frameID = 0x01;    // Frame ID for ACK
            byte[] reserved = { 0xFF, 0xFE };  // Reserved bytes (for broadcast)
            byte broadcastRadius = 0x00;  // Maximum number of hops
            byte options = 0x00;  // Disable ACKs and Route Discovery

            // Build the frame
            List<byte> frame = new List<byte>
        {
            0x7E,  // Start delimiter
        };

            // Length of the frame excluding delimiter and length
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
            for (int i = 3; i < frame.Count; i++)  // Exclude delimiter and length
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

        public void ProcessTXStatusFrame(byte[] frameData)
        {
            if (frameData[0] == 0x8B)
            {
                byte frameID = frameData[1];
                byte deliveryStatus = frameData[5];

                if (deliveryStatus == 0x00)
                {
                    Console.WriteLine($"Zpráva s Frame ID {frameID} byla úspěšně doručena.");
                }
                else
                {
                    Console.WriteLine($"Zpráva s Frame ID {frameID} nebyla doručena. Stav: {deliveryStatus:X2}");
                }
            }
        }

        public void ProcessReceivedMessage(byte[] frameData)
        {
            if (frameData == null || frameData.Length < 15)
            {
                Console.WriteLine("Neplatný rámec.");
                return;
            }

            // Zkontrolujeme, zda se jedná o RX Packet (Frame Type 0x90)
            if (frameData[0] == 0x90)
            {
                // Čteme 64-bitovou adresu zdroje
                byte[] sourceAddress = new byte[8];
                Array.Copy(frameData, 1, sourceAddress, 0, 8);

                // Přijatá RF data (data odeslaná z druhého XBee)
                int rfDataLength = frameData.Length - 12; // 12 je počet bajtů před RF daty
                byte[] rfData = new byte[rfDataLength];
                Array.Copy(frameData, 12, rfData, 0, rfDataLength);

                // Zobrazíme přijatá data
                string receivedMessage = Encoding.ASCII.GetString(rfData);
                Console.WriteLine($"Přijatá zpráva od {BitConverter.ToString(sourceAddress)}: {receivedMessage}");
            }
            else
            {
                Console.WriteLine("Přijatý rámec není RX (Receive Packet).");
            }
        }
    }

}
