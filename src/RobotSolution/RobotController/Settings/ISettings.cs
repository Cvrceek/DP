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

        int ExM_PWM_Pin { get; set; }
        int ExM_DIR_Pin { get; set; }

        int ExternalHolderServo_PWM_Pin { get; set; }
        int ExternalDevice_PWM_Pin1 { get; set; }
        int ExternalDevice_PWM_Pin2 { get; set; }

        int LedRampRele_Pin { get; set; }
        int ADS_Battery_Pin { get; set; }

        void Load();
        void Save();
    }
}