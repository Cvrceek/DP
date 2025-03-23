// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using Iot.Device.Nmea0183;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RobotController.Extensions;
using RobotController.Settings;
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
        XBeeConnection connection = new XBeeConnection(sett.SerialPortName, sett.SerialPortBaudRate);
        connection.Open();

        _ = Task.Run(() => connection.StartListening());


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