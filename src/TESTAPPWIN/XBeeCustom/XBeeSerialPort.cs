using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace XBee.Custom
{
    public class XBeeSerialPort : SerialPort
    {
        public event EventHandler<byte[]> XBeeDataReceived;

        private List<byte> dataStack = new List<byte>();

        public XBeeSerialPort() { }
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
                //nacteni novych dat
                // Načteme všechna nová data jako binární data
                byte[] buffer = new byte[this.BytesToRead];
                this.Read(buffer, 0, buffer.Length);

                // Přidáme nová data do zásobníku
                dataStack.AddRange(buffer);

                while (dataStack.Count > 0)
                {
                    // Hledáme start delimiter (0x7E)
                    int startIndex = dataStack.IndexOf(0x7E);
                    if (startIndex == -1)
                    {
                        // Nenalezen start delimiter, počkáme na další data
                        return;
                    }

                    // Odstraníme všechno před start delimiterem
                    if (startIndex > 0)
                    {
                        dataStack.RemoveRange(0, startIndex);
                    }

                    // Ověříme, zda máme dostatek dat k přečtení délky rámce
                    if (dataStack.Count < 3)
                    {
                        // Čekáme na další data (musíme mít alespoň 3 bajty pro start delimiter a délku)
                        return;
                    }

                    // Přečteme délku rámce
                    byte lengthMSB = (byte)dataStack[1];
                    byte lengthLSB = (byte)dataStack[2];
                    int length = (lengthMSB << 8) | lengthLSB;

                    // Ověříme, zda máme dostatek dat k přečtení celého rámce
                    if (dataStack.Count < length + 3 + 1) // 3 bajty pro delimiter a délku + 1 byte pro checksum
                    {
                        // Čekáme na další data (rámec ještě není kompletní)
                        return;
                    }

                    // Přečteme frame data
                    byte[] frameData = new byte[length];
                    for (int i = 0; i < length; i++)
                    {
                        frameData[i] = (byte)dataStack[i + 3];
                    }

                    // Přečteme kontrolní součet
                    byte checksum = (byte)dataStack[length + 3];

                    // Zkontrolujeme správnost kontrolního součtu
                    if (checksum != CalculateChecksum(frameData))
                    {
                        Debug.WriteLine("Chybný kontrolní součet.");
                        dataStack.RemoveRange(0, length + 4); // Odstraníme špatný rámec a pokračujeme
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
