using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RobotController.Settings
{
    public class Settings : ISettings
    {
        public Settings()
        {
        }
        #region Load/save
        private static readonly string FilePath = Path.Combine(AppContext.BaseDirectory, "Settings",
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Settings_Win.json" : "Settings_Linux.json");

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

        #region SerialPort
        public string SerialPortName { get; set; }
        public int SerialPortBaudRate { get; set; }
        #endregion

        #region MainMotors
        public int M1_PWM_Pin { get; set; }
        public int M1_DIR_Pin { get; set; }
        public int M2_PWM_Pin { get; set; }
        public int M2_DIR_Pin { get; set; }

        #endregion

        #region RemoteXbee
        public string SH { get; set; }
        public string SL { get; set; }
        #endregion

        #region PCA9685
        public int PCA9685_BusID { get; set; }
        public int PCA9685_Address { get; set; }
        #endregion

        #region ADS1115
        public int ADS1115_BusID { get; set; }
        public int ADS1115_Address { get; set; }
        #endregion

        #region ExternalMotor
        public int ExM_PWM_Pin { get; set; }
        public int ExM_DIR_Pin { get; set; }
        #endregion

        public int ExternalHolderServo_PWM_Pin { get; set; }
        public int ExternalDevice_PWM_Pin1 { get; set; }
        public int ExternalDevice_PWM_Pin2 { get; set; }

        public int LedRampRele_Pin { get; set; }

        public int ADS_Battery_Pin { get; set; }

    }
}
