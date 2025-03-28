using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    }
}
