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
using RobotLibs.PCA9685;

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
            services.AddSingleton<GpioController>(_ => new GpioController(PinNumberingScheme.Logical));
            services.AddSingleton<PCA9685>(_ => new PCA9685());

            return services;
        }
    }
}
