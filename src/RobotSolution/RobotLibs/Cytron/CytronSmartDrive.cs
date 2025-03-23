using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Pwm;

namespace RobotLibs.Cytron
{
    public class CytronSmartDrive : IDisposable
    {
        private readonly int pinPWM;
        private readonly int pinDIR;
        private readonly GpioController gpio;
        private readonly PwmChannel pwmChannel;

        /// <summary>
        /// Konstruktor pro motor driver CytronSmartDrive.
        /// </summary>
        /// <param name="pwm">GPIO pin pro PWM</param>
        /// <param name="dir">GPIO pin pro směr</param>
        public CytronSmartDrive(int pwm, int dir)
        {
            pinPWM = pwm;
            pinDIR = dir;
            gpio = new GpioController();

            gpio.OpenPin(pinDIR, PinMode.Output);
            gpio.Write(pinDIR, PinValue.High); 

            pwmChannel = PwmChannel.Create(0, pinPWM, 100, 0);
            pwmChannel.Start();
        }

        public void SetMotor(int speed)
        {
            if (speed >= 0)
            {
                gpio.Write(pinDIR, PinValue.High);
                pwmChannel.DutyCycle = speed / 100.0;
            }
            else
            {
                gpio.Write(pinDIR, PinValue.Low);
                pwmChannel.DutyCycle = -speed / 100.0;
            }
        }

        public void StopMotor()
        {
            gpio.Write(pinDIR, PinValue.High);
            pwmChannel.DutyCycle = 0;
        }

        public void Dispose()
        {
            pwmChannel.Stop();
            pwmChannel.Dispose();
            gpio.ClosePin(pinDIR);
            gpio.Dispose();
        }
    }
}
