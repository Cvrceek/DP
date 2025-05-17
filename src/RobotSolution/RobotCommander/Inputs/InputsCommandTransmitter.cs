using Microsoft.Extensions.DependencyInjection;
using RobotCommander.Inputs;
using RobotCommander.Settings;
using RobotLibs.DTO.DTOModels;
using RobotLibs.XbeeCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotCommander.Inputs
{
    /// <summary>
    /// Trida zpracovava vstupy z ovladaciho zarine a posila je na druhe xbee
    /// </summary>
    public class InputsCommandTransmitter
    {
        private int direcitionX, direcitionY, mainMotorMinPower, mainMotorMaxPower;

        XBeeConnection xBeeConnection;
        ISettings settings;

        IInputDevice inputDevice;
        public InputsCommandTransmitter()
        {
            settings = App.Services.GetService<ISettings>();
            direcitionX = direcitionY = 0;
            mainMotorMinPower = settings.Min_MotorsPower;
            mainMotorMaxPower = settings.Max_MotorsPower;
                      
            xBeeConnection = new XBeeConnection(settings.SerialPortName, settings.SL, settings.SH, settings.SerialPortBaudRate);
            inputDevice = new GamePad();

            SetEvents();

            OpenConnection();
            
            RunGamePad();

            

            xBeeConnection.TXFrameReceived += (s, e) =>
            {
                
            };
        }

        private void SetEvents()
        {
            inputDevice.SpeedChanged += (s, e) =>
            {
                direcitionY = e;
                var values = CalculateMainMotorsValues();

                xBeeConnection.SendAPIMessage(values.ToBytes());
            };

            inputDevice.DirectionChanged += (s, e) =>
            {
                direcitionX = e;
                var values = CalculateMainMotorsValues();
                xBeeConnection.SendAPIMessage(values.ToBytes());
            };

            inputDevice.LightsChanged += (s, e) =>
            {
                var values = new SwitchValues()
                {
                    EDTOType = RobotLibs.DTO.EDTOType.LedRamp,
                    ON = e
                };
                xBeeConnection.SendAPIMessage(values.ToBytes());
            };

            inputDevice.HornChanged += (s, e) =>
            {
                var values = new SwitchValues()
                {
                    EDTOType = RobotLibs.DTO.EDTOType.Horn,
                    ON = e
                };
                xBeeConnection.SendAPIMessage(values.ToBytes());
            };

            inputDevice.BeaconChanged += (s, e) =>
            {
                var values = new SwitchValues()
                {
                    EDTOType = RobotLibs.DTO.EDTOType.Beacon,
                    ON = e
                };
                xBeeConnection.SendAPIMessage(values.ToBytes());
            };

            inputDevice.ExternalDeviceHolderPositionChanged += (s, e) =>
            {
                var values = new PWMValues()
                {
                    EDTOType = RobotLibs.DTO.EDTOType.ExternalDeviceHolderValues,
                    Position = e
                };
                xBeeConnection.SendAPIMessage(values.ToBytes());
            };

            inputDevice.ExternalDevicePWM1Changed += (s, e) =>
            {
                var values = new PWMValues()
                {
                    EDTOType = RobotLibs.DTO.EDTOType.ExternalDevicePWM1,
                    Position = e
                };
                xBeeConnection.SendAPIMessage(values.ToBytes());
            };

            inputDevice.ExternalDevicePWM2Changed += (s, e) =>
            {
                var values = new PWMValues()
                {
                    EDTOType = RobotLibs.DTO.EDTOType.ExternalDevicePWM2,
                    Position = e
                };
                xBeeConnection.SendAPIMessage(values.ToBytes());
            };

            inputDevice.ExternalDeviceSpeedChanged += (s, e) =>
            {
                var values = new ExternalDeviceMotorValues()
                {
                    Speed = e,
                    Orientation = settings.ExternalMotorDirection
                };
                xBeeConnection.SendAPIMessage(values.ToBytes());
            };
        }

        #region MainMotors
        private MainMotorsValues CalculateMainMotorsValues()
        {
            var dto = new MainMotorsValues();

            if (direcitionY > 0)
                dto.OrientationLeft = dto.OrientationRight = 0;
            else
                dto.OrientationRight = dto.OrientationLeft = 1;
            //otocka na miste, dle zkoušek doplnit hodnoty
            //if (direcitionY == 0 && Math.Abs(direcitionX) == 100)
            //{
            //    dto.SpeedLeft = 0;
            //    dto.SpeedRight = 0;
            //}
            //stoji
            if (direcitionY == 0 && direcitionX == 0)
            {
                dto.SpeedLeft = 0;
                dto.SpeedRight = 0;
            }
            else
            {
                decimal speedLeft = (Math.Abs(direcitionY) * (100 - direcitionX)) / 100.0m;
                decimal speedRight = (Math.Abs(direcitionY) * (100 + direcitionX)) / 100.0m;

                if (speedLeft > 100 || speedRight > 100)
                {
                    var ratio = 100 / Math.Max(speedLeft, speedRight);
                    speedLeft = speedLeft * ratio;
                    speedRight = speedRight * ratio;
                }
                dto.SpeedLeft = mapToPowerRange(speedLeft);
                dto.SpeedRight = mapToPowerRange(speedRight);
            }

            return dto;
        }

        private int mapToPowerRange(decimal value)
        {
            return (int)Math.Round(value / 100 * (mainMotorMaxPower - mainMotorMinPower) + mainMotorMinPower);
        }

        #endregion

        public void OpenConnection()
        {
            xBeeConnection.Open();
        }

        public void RunGamePad()
        {
            _ = Task.Run(() => { inputDevice.Run(); });
        }
     
    }
}
