namespace RobotController.Settings
{
    public interface ISettings
    {
        int SerialPortBaudRate { get; set; }
        string SerialPortName { get; set; }
        int M1_PWM_Pin { get; set; }
        int M1_DIR_Pin { get; set; }
        int M2_PWM_Pin { get; set; }
        int M2_DIR_Pin { get; set; }

        string SH { get; set; }
        string SL { get; set; }

        int PCA9685_BusID { get; set; }
        int PCA9685_Address { get; set; }

        int ADS1115_BusID { get; set; }
        int ADS1115_Address { get; set; }
        
        void Load();
        void Save();
    }
}