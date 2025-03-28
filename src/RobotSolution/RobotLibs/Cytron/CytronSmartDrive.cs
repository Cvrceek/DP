using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using RobotLibs.Helpers;
using Iot.Device.Pwm;
using UnitsNet;

namespace RobotLibs.Cytron
{
    public class CytronSmartDrive 
    {
        private readonly int pinPWM;
        private readonly int pinDIR;
        private readonly GpioController gpio;
        private Pca9685 pca9685;

        public CytronSmartDrive(GpioController gpioController, Pca9685 pca9685, int pwm, int dir)
        {
            pinPWM = pwm;
            pinDIR = dir;
            gpio = gpioController;
            this.pca9685 = pca9685;

            gpio.OpenPin(pinDIR, PinMode.Output);
            gpio.Write(pinDIR, PinValue.High);
        }

        public void SetMotor(int speed)
        {
            if (speed >= 0)
            {
                gpio.Write(pinDIR, PinValue.High);
                pca9685.SetDutyCycle(pinPWM, speed / 100);
            }
            else
            {
                gpio.Write(pinDIR, PinValue.Low);
                pca9685.SetDutyCycle(pinPWM, speed / 100);
            }
        }
        public void StopMotor()
        {
            gpio.Write(pinDIR, PinValue.High);
            pca9685.SetDutyCycle(pinPWM, 0);

        }
    }
}
