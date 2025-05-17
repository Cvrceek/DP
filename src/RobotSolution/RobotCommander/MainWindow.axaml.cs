using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using RobotCommander.FPV;
using RobotCommander.Inputs;
using RobotCommander.Settings;
using RobotCommander.FPV.OpenCV;
using RobotLibs.XbeeCustom;
using System;
using System.Threading.Tasks;
using System.Threading;
namespace RobotCommander
{
    public partial class MainWindow : Window
    {
        XBeeConnection connection;
        IFPVManager FPVManager;
        ISettings settings;
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;

            var cams = ExternalDevices.GetCameras();
            settings = App.Services.GetService<ISettings>();

            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F11)
                {
                    SystemDecorations = SystemDecorations.None;
                    WindowState = WindowState.FullScreen;
                }
                if (e.Key == Key.F10)
                {
                    SystemDecorations = SystemDecorations.None;
                    WindowState = WindowState.Maximized;
                }
                if (e.Key == Key.F9)
                {
                    SystemDecorations = SystemDecorations.Full;
                    WindowState = WindowState.Normal;
                }



                if (e.Key == Key.Escape)
                {
                    Close();
                }
            };




            FPVManager = new FPVManager_OpenCV();
            FPVManager.FrameChanged += async (s, e) =>
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    _FPVImageView.Source = e;
                }, DispatcherPriority.Render);
            };


            InputsCommandTransmitter transmitter = new InputsCommandTransmitter();
   

        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            var thread = new Thread(() =>
            {
                FPVManager.Run(settings.CamID);
            });

            thread.Name = "FPV Thread";
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
    }
}