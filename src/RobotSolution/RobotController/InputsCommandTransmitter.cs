using Iot.Device.Pwm;
using Microsoft.Extensions.DependencyInjection;
using RobotController.Settings;
using RobotLibs.Cytron;
using RobotLibs.DTO.DTOModels;
using RobotLibs.DTO;
using RobotLibs.XbeeCustom;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController
{
    public class InputsCommandTransmitter
    {
        private readonly ISettings settings;
        private readonly GpioController gpioController;
        private readonly Pca9685 pca;
        private readonly CytronSmartDuoDrive motors;
        private readonly CytronSmartDrive externalMotor;
        private readonly XBeeConnection xBeeConnection;

        public InputsCommandTransmitter()
        {
            settings = App.ServiceProvider.GetService<ISettings>();
            gpioController = App.ServiceProvider.GetService<GpioController>();
            pca = App.ServiceProvider.GetService<Pca9685>();
            motors = new CytronSmartDuoDrive(gpioController, pca, settings.M1_PWM_Pin, settings.M1_DIR_Pin, settings.M2_PWM_Pin, settings.M2_DIR_Pin);
            externalMotor = new CytronSmartDrive(gpioController, pca, settings.ExM_PWM_Pin, settings.ExM_DIR_Pin);
            xBeeConnection = App.ServiceProvider.GetService<XBeeConnection>();

            gpioController.OpenPin(settings.LedRampRele_Pin, PinMode.Output);

        }

        public void SetEvents()
        {
            xBeeConnection.TXFrameReceived += async (s, e) =>
            {
                //hlavni motory
                if (e.RFData[0] == (byte)EDTOType.MainMotorsValues)
                {
                    var values = MainMotorsValues.FromBytes(e.RFData.ToArray());
                    motors.SetMotors(values.OrientationRight, values.OrientationLeft, values.SpeedRight, values.SpeedLeft);

                    Console.WriteLine($"RFData\nOL:{values.OrientationLeft}  OR:{values.OrientationRight}  SL:{values.SpeedLeft}   SR:{values.SpeedRight}");
                }
                //motor externi
                if (e.RFData[0] == (byte)EDTOType.ExternalDeviceMotorValues)
                {
                    var values = ExternalDeviceMotorValues.FromBytes(e.RFData.ToArray());
                    externalMotor.SetMotor(values.Speed, values.Orientation);
                }
                //drzak servo
                if (e.RFData[0] == (byte)EDTOType.ExternalDeviceHolderValues)
                {
                    var values = PWMValues.FromBytes(e.RFData.ToArray());
                    pca.SetDutyCycle(settings.ExternalHolderServo_PWM_Pin, values.Position);
                }
                //ext pwm 1
                if (e.RFData[0] == (byte)EDTOType.ExternalDevicePWM1)
                {
                    var values = PWMValues.FromBytes(e.RFData.ToArray());
                    pca.SetDutyCycle(settings.ExternalDevice_PWM_Pin1, values.Position);
                }
                //ext pwm2
                if (e.RFData[0] == (byte)EDTOType.ExternalDevicePWM2)
                {
                    var values = PWMValues.FromBytes(e.RFData.ToArray());
                    pca.SetDutyCycle(settings.ExternalDevice_PWM_Pin2, values.Position);
                }
                //ledky
                if (e.RFData[0] == (byte)EDTOType.LedRamp)
                {
                    var values = SwitchValues.FromBytes(e.RFData.ToArray());
                    gpioController.Write(settings.LedRampRele_Pin, values.ON ? PinValue.High : PinValue.Low);
                }
            };
        }

        public void Stop()
        {
            motors.SetMotors(0, 0, 0, 0);
            externalMotor.SetMotor(0, 0);
        }
    }
}
