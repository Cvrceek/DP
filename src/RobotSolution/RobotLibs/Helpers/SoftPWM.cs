using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotLibs.Helpers
{
    //https://learn.microsoft.com/en-us/dotnet/iot/gpio/
    //https://stackoverflow.com/questions/66688530/how-can-i-create-pwm-in-c-on-raspberry-pi
    //WiringPI - SoftPWm náhrada
    public class SoftPWM
    {
        private readonly GpioController gpio;
        private readonly int pin;
        private readonly int frequency;
        private CancellationTokenSource? cts;
        private Task? pwmTask;

        public SoftPWM(int gpioPin, int frequency = 100)
        {
            pin = gpioPin;
            this.frequency = frequency;
            gpio = new GpioController(PinNumberingScheme.Logical);
            gpio.OpenPin(pin, PinMode.Output);
        }

        public void SoftPwmWrite(int dutyPercent)
        {
            dutyPercent = Math.Clamp(dutyPercent, 0, 100);

            if (cts != null)
            {
                cts.Cancel();
                pwmTask?.Wait();
            }

            cts = new CancellationTokenSource();
            var token = cts.Token;
            int period = 1000 / frequency;

            pwmTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    int high = period * dutyPercent / 100;
                    int low = period - high;

                    gpio.Write(pin, PinValue.High);
                    await Task.Delay(high, token).ContinueWith(_ => { });

                    gpio.Write(pin, PinValue.Low);
                    await Task.Delay(low, token).ContinueWith(_ => { });
                }
            }, token);
        }

        public void Stop()
        {
            cts?.Cancel();
            pwmTask?.Wait();
            gpio.Write(pin, PinValue.Low);
        }

        public void Dispose()
        {
            Stop();
            gpio.ClosePin(pin);
            gpio.Dispose();
        }
    }
}
