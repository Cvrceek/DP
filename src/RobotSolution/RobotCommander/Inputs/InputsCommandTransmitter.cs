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

            OpenConnection();
            RunGamePad();

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


            xBeeConnection.FrameReceived += (s, e) =>
            {
                if(e.FrameType == 0x8B)
                {

                }
            };
        }

        #region MainMotors
        private MainMotorsValues CalculateMainMotorsValues()
        {
            var dto = new MainMotorsValues();

            dto.OrientationLeft = dto.OrientationRight = (byte)(direcitionY > 0 ? 1 : 0);

            //otocka na miste, dle zkoušek doplnit hodnoty
            if (direcitionY == 0 && Math.Abs(direcitionX) == 100)
            {
                dto.SpeedLeft = 0;
                dto.SpeedRight = 0;
            }
            //stoji
            else if (direcitionY == 0 && direcitionX == 0)
            {
                dto.SpeedLeft = 0;
                dto.SpeedRight = 0;
            }
            else
            {
                decimal speedRight = (Math.Abs(direcitionY) * (100 - direcitionX)) / 100.0m;
                decimal speedLeft = (Math.Abs(direcitionY) * (100 + direcitionX)) / 100.0m;

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
