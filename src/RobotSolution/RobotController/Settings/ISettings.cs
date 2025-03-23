namespace RobotController.Settings
{
    public interface ISettings
    {
        int SerialPortBaudRate { get; set; }
        string SerialPortName { get; set; }

        void Load();
        void Save();
    }
}