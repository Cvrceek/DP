using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using RobotCommander.Inputs;
using RobotCommander.Settings;
using RobotLibs.XbeeCustom;
using System.Threading.Tasks;

namespace RobotCommander
{
    public partial class MainWindow : Window
    {
        XBeeConnection connection;
   
        public MainWindow()
        {
            InitializeComponent();
          
            InputsCommandTransmitter transmitter = new InputsCommandTransmitter();
   

        }
    }
}