
using Microsoft.Extensions.DependencyInjection;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WpfApp1.FPV;
using WpfApp1.Settings;
using XBee.Custom;



namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
   
        XBeeConnection connection;
        FPVManager FPVManager;
     

        public MainWindow()
        {
            InitializeComponent();

            _CamImgView.Source = FPVManager.LoadingCam();

            var cams = ExternalDevices.GetCameras();
            var port = ExternalDevices.GetSerialPorts();

            var settings = App.Services.GetService<RSettings>();
    
            FPVManager = new FPVManager();
            FPVManager.FrameChanged += (s, e) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    _CamImgView.Source = e;
                }, System.Windows.Threading.DispatcherPriority.Render);
            };
            FPVManager.Run(settings.CamID);



            //capture = new VideoCapture(0); // 0 pro výchozí kameru
            //frame = new Mat();

            //timer = new System.Windows.Threading.DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(30); // Nastavení intervalu pro obnovení snímků
            //timer.Tick += Timer_Tick;
            //timer.Start();

            connection = new XBeeConnection(settings.SerialPortName, settings.SerialPortBaudRate);
            connection.Open();

            connection.SendAPIMessage(0x05, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF }, new byte[] { 3, 50 });


            DevicesManager dm = new DevicesManager();
            dm.MotorsValuesChanged += (s, e) =>
            {
                //Debug.WriteLine($"dir: {e.OrientationLeft} left: {e.SpeedLeft} right: {e.SpeedRight}");
                connection.SendAPIMessage(0x05, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF }, e.ConvertToBytes());
            };

            dm.DeviceMotorStateChanged += (s, e) =>
            {
                connection.SendAPIMessage(0x05, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF }, new byte[] { 2, (byte)(e ? 100 : 0) });
            };

            dm.ServoChanged += (s, e) =>
            {
                connection.SendAPIMessage(0x05, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF }, new byte[] { 3, (byte)e });
            };
            
        }
    }
}
     
