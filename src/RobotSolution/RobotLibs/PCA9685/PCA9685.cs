using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.PCA9685
{
    //vychazi z implementace pro c++, viz old implementace
    [Obsolete("Použít Pca9685 z Iot.Device.Pwm")]
    public class PCA9685
    {
        private const int MODE1 = 0x00;
        private const int MODE2 = 0x01;
        private const int SUBADR1 = 0x02;
        private const int SUBADR2 = 0x03;
        private const int SUBADR3 = 0x04;
        private const int ALLCALLADR = 0x05;
        private const int LED0 = 0x06;
        private const int LED0_ON_L = 0x06;
        private const int LED0_ON_H = 0x07;
        private const int LED0_OFF_L = 0x08;
        private const int LED0_OFF_H = 0x09;
        private const int LED_MULTIPLYER = 4;
        private const int ALLLED_ON_L = 0xFA;
        private const int ALLLED_ON_H = 0xFB;
        private const int ALLLED_OFF_L = 0xFC;
        private const int ALLLED_OFF_H = 0xFD;
        private const int PRE_SCALE = 0xFE;

        private const double CLOCK_FREQ = 25000000.0;

        private const int BUFFER_SIZE = 0x08;

        private readonly I2cDevice _device;
        private readonly int _i2caddr;
        private readonly int _i2cbus;
        private readonly byte[] dataBuffer = new byte[BUFFER_SIZE];

        public PCA9685(int bus = 1, int address = 0x40)
        {
            _i2caddr = address;
            _i2cbus = bus;
            _device = I2cDevice.Create(new I2cConnectionSettings(bus, address));
            Reset();
        }

        public void Reset()
        {
            Write_byte(MODE1, 0x00);
            Write_byte(MODE2, 0x04);
        }

        public void SetPWMFreq(int freq)
        {
            int prescale = (int)(CLOCK_FREQ / 4096.0 / freq) - 1;

            byte oldmode = Read_byte(MODE1);
            byte newmode = (byte)((oldmode & 0x7F) | 0x10);

            Write_byte(MODE1, newmode);
            Write_byte(PRE_SCALE, (byte)prescale);
            Write_byte(MODE1, oldmode);
            Thread.Sleep(10);
            Write_byte(MODE1, (byte)(oldmode | 0x80));
        }

        public void SetPWM(byte led, int value)
        {
            SetPWM(led, 0, value);
        }

        public void SetPWM(byte led, int on_value, int off_value)
        {
            Write_byte(LED0_ON_L + LED_MULTIPLYER * led, (byte)(on_value & 0xFF));
            Write_byte(LED0_ON_H + LED_MULTIPLYER * led, (byte)(on_value >> 8));
            Write_byte(LED0_OFF_L + LED_MULTIPLYER * led, (byte)(off_value & 0xFF));
            Write_byte(LED0_OFF_H + LED_MULTIPLYER * led, (byte)(off_value >> 8));
        }

        private byte Read_byte(int address)
        {
            byte[] buffer = new byte[1];
            _device.WriteRead(new byte[] { (byte)address }, buffer);
            return buffer[0];
        }

        private void Write_byte(int address, byte data)
        {
            _device.Write(new byte[] { (byte)address, data });
        }
    }
}

