namespace RobotCommander.Settings
{
    public interface ISettings
    {
        string CamID { get; set; }
        int Max_MainServo { get; set; }
        int Max_MotorsPower { get; set; }
        int Min_MainServo { get; set; }
        int Min_MotorsPower { get; set; }
        int SerialPortBaudRate { get; set; }
        string SerialPortName { get; set; }
        int Step_MainServo { get; set; }

        void Load();
        void Save();
    }
}