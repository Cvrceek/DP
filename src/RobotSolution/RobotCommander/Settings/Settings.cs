using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RobotCommander.Settings
{
    public class Settings : ISettings
    {
        public Settings()
        {
        }
        #region Load/save
        private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "Settings", "Settings.json");

        public void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {

            }
        }
        public void Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);
                    var loadedSettings = JsonSerializer.Deserialize<Settings>(json);

                    if (loadedSettings != null)
                    {
                        foreach (PropertyInfo prop in typeof(Settings).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (prop.Name == nameof(FilePath))
                                continue;

                            object value = prop.GetValue(loadedSettings);
                            prop.SetValue(this, value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region MotorPower
        public int Min_MotorsPower { get; set; }
        public int Max_MotorsPower { get; set; }
        #endregion

        #region MainServo
        public int Min_MainServo { get; set; }
        public int Max_MainServo { get; set; }
        public int Step_MainServo { get; set; }
        #endregion

        #region SerialPort
        public string SerialPortName { get; set; }
        public int SerialPortBaudRate { get; set; }
        #endregion

        public string CamID { get; set; }

        #region RemoteXbee
        public string SH { get; set; }
        public string SL { get; set; }
        #endregion

        public byte ExternalMotorDirection { get; set; }
    }
}
