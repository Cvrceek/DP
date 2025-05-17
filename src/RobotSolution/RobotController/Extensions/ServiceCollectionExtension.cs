using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RobotController.Settings;
using Serilog.Formatting.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog.Extensions.Logging;
using System.Device.Gpio;
using Iot.Device.Pwm;
using System.Device.I2c;
using RobotLibs.CustomADS1115;
using RobotLibs.XbeeCustom;

namespace RobotController.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRobotControllerServices(this IServiceCollection services)
        {
            services.AddSingleton<ISettings>(provider =>
            {
                var settings = new Settings.Settings();
                settings.Load();
                return settings;
            });

            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "Logs.json");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(new JsonFormatter(), filePath, rollingInterval: RollingInterval.Infinite, retainedFileCountLimit: 1)
                .MinimumLevel.Debug()
                .CreateLogger();

            services.AddLogging(builder => builder.AddProvider(new SerilogLoggerProvider(Log.Logger)));
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddSingleton<GpioController>(_ => new GpioController());
            services.AddSingleton<Pca9685>(provider =>
            {
                var settings = provider.GetRequiredService<ISettings>();
                var i2c = I2cDevice.Create(new I2cConnectionSettings(settings.PCA9685_BusID, settings.PCA9685_Address));
                var pca = new Pca9685(i2c, 50);
                return pca; 
            });

            services.AddSingleton<ADS1115Custom>(provider =>
            {
                var settings = provider.GetRequiredService<ISettings>();
                var i2c = I2cDevice.Create(new I2cConnectionSettings(settings.ADS1115_BusID, settings.ADS1115_Address));
                return new ADS1115Custom(i2c);
            });

            services.AddSingleton<XBeeConnection>(provider =>
            {
                var settings = provider.GetRequiredService<ISettings>();
                var xbee = new XBeeConnection(settings.SerialPortName, settings.SL, settings.SH, settings.SerialPortBaudRate);
                return xbee;
            });

            return services;
        }
    }
}
