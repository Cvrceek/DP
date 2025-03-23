using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RobotCommander.Comand;
using RobotCommander.Extensions;
using RobotCommander.Settings;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;
using System;
using System.IO;

namespace RobotCommander
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        

        public override void OnFrameworkInitializationCompleted()
        {

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRobotCommanderServices();
            Services = serviceCollection.BuildServiceProvider();

            GamePad gp = new GamePad();


            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

   

            base.OnFrameworkInitializationCompleted();


        }
    }
}