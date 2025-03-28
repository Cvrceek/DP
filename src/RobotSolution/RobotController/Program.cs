// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using Iot.Device.Amg88xx;
using Iot.Device.Nmea0183;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RobotController.Extensions;
using RobotController.Settings;
using RobotLibs.Cytron;
using RobotLibs.DTO;
using RobotLibs.DTO.DTOModels;
using RobotLibs.XbeeCustom;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;



internal class Program
{
    public static IServiceProvider Services { get; private set; }

    private static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureServices();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Hello, World! - Logováno pomocí Serilog");

        var sett = serviceProvider.GetRequiredService<ISettings>();

        var motors = new CytronSmartDuoDrive(sett.M1_PWM_Pin, sett.M1_DIR_Pin, sett.M2_PWM_Pin, sett.M2_DIR_Pin);
        
        XBeeConnection connection = new XBeeConnection(sett.SerialPortName, sett.SL, sett.SH, sett.SerialPortBaudRate);
        connection.Open();



        connection.FrameReceived += async (s, e) =>
        {
            if (e.RFData[0] == (byte)EDTOType.MainMotorsValues)
            {
                var values = MainMotorsValues.FromBytes(e.RFData.ToArray());
                motors.SetMotors(values.OrientationRight, values.OrientationLeft, values.SpeedRight, values.SpeedLeft);
                
                Console.WriteLine($"RFData\nOL:{values.OrientationLeft}  OR:{values.OrientationRight}  SL:{values.SpeedLeft}   SR:{values.SpeedRight}");
            }
            else
            {

            }
        };


        await Task.Delay(Timeout.Infinite);
    }

    private static ServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddRobotControllerServices();
        Services = serviceCollection.BuildServiceProvider();
        return serviceCollection.BuildServiceProvider();
    }
}