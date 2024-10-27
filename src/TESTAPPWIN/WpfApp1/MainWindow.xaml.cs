using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XBee;
using XBee.Custom;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
   
        XBeeConnection connection;


        public MainWindow()
        {
            InitializeComponent();


            connection = new XBeeConnection("COM9");
            connection.Open();


            DevicesManager dm = new DevicesManager();
            dm.MotorsValuesChanged += (s, e) =>
            {
                Debug.WriteLine($"dir: {e.OrientationLeft} left: {e.SpeedLeft} right: {e.SpeedRight}");
                connection.SendAPIMessage(0x05, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF }, e.ConvertToBytes());
            };



            
        }
    }
}
     
