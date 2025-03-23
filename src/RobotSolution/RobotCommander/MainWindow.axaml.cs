using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using RobotCommander.Settings;
using RobotLibs.XbeeCustom;

namespace RobotCommander
{
    public partial class MainWindow : Window
    {
        XBeeConnection connection;
   
        public MainWindow()
        {
            InitializeComponent();
            var settings = App.Services.GetService<ISettings>();
          

            connection = new XBeeConnection(settings.SerialPortName, settings.SerialPortBaudRate);
            connection.Open();
        }
    }
}