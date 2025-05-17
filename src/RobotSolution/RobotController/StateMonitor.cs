using Microsoft.Extensions.DependencyInjection;
using RobotController.Settings;
using RobotLibs.CustomADS1115;
using RobotLibs.DTO.DTOModels;
using RobotLibs.XbeeCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotController
{
    public class StateMonitor
    {
        public event EventHandler StopRobot;
        private readonly ADS1115Custom ADS1115Custom;
        private readonly XBeeConnection XBeeConnection;
        private readonly ISettings settings;

        public StateMonitor()
        {
            ADS1115Custom = App.ServiceProvider.GetService<ADS1115Custom>();
            XBeeConnection = App.ServiceProvider.GetService<XBeeConnection>();
            settings = App.ServiceProvider.GetService<ISettings>();

            XBeeConnection.DeliveryStatusFrameReceived += (s, e) =>
            {
                if (!e.DeliverySuccessful)
                {
                    StopRobot?.Invoke(this, new EventArgs());
                }
            };
        }

        public async Task SendStatusAndWatchCommunication(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var voltage = ADS1115Custom.ReadVoltage(settings.ADS_Battery_Pin);
                RobotStateValues state = new RobotStateValues()
                {
                    BatteryVoltage = ADS1115Custom.ReadVoltage(settings.ADS_Battery_Pin)
                };
                XBeeConnection.SendAPIMessage(state.ToBytes());

                try
                {
                    await Task.Delay(500, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }
    }
}
