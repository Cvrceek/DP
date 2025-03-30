using System;
using System.Collections.Generic;
using System.Device.I2c;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iot.Device.Ads1115;

namespace RobotLibs.CustomADS1115
{
    public class ADS1115Custom
    {
        private readonly Ads1115 ads;
        private readonly int ads_conversionDelay = 8;
  
        public ADS1115Custom(I2cDevice i2cDevice)
        {
            ads = new Ads1115(i2cDevice, InputMultiplexer.AIN0); 
            ads.MeasuringRange = MeasuringRange.FS6144; 
        }

        public void SetGain(MeasuringRange range)
        {
            ads.MeasuringRange = range;
        }
    
        public double ReadVoltage(int channel)
        {
            InputMultiplexer mux = channel switch
            {
                0 => InputMultiplexer.AIN0,
                1 => InputMultiplexer.AIN1,
                2 => InputMultiplexer.AIN2,
                3 => InputMultiplexer.AIN3,
                _ => InputMultiplexer.AIN0
            };

            ads.InputMultiplexer = mux;

            Thread.Sleep(ads_conversionDelay);
            return ads.ReadVoltage(mux).Millivolts;
        }
    }
}
