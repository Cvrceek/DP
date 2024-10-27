using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Windows.Documents;

namespace WpfApp1
{

    //pochopení logiky vychází z "Mobile Robots: Navigation, Control and Remote Sensing" od Gerald Cook a Feitian Zhang.
    public class MotorsValues
    {
        public MotorsValues() { }
        public MotorsValues(int speed, int direction)
        {
            //provotni nastaveni orientace, dle hodnoty rychlosti
            OrientationRight = OrientationLeft = (byte)(speed >= 0 ? 1 : 0);


            decimal speedRight = (Math.Abs(speed) * (100 - direction)) / 100.0m;
            decimal speedLeft = (Math.Abs(speed) * (100 + direction)) / 100.0m;

            if (speedLeft > 100 || speedRight > 100)
            {
                var ratio = 100 / Math.Max(speedLeft, speedRight);

                speedLeft = speedLeft * ratio;
                speedRight = speedRight * ratio;
            }
          
            SpeedLeft = mapToPowerRange(speedLeft);
            SpeedRight = mapToPowerRange(speedRight);
        }
        public byte SpeedRight = 0;
        public byte SpeedLeft = 0;
        public byte OrientationRight = 0;
        public byte OrientationLeft = 0;

        public byte[] ConvertToBytes()
        {
            return new byte[] { OrientationRight, SpeedRight, OrientationLeft, SpeedLeft };
        }
        /// <summary>
        /// metoda pro prevod vypocitanych rychlosit, do pouzitelneho vykonu
        /// motory potrebuji min vykon, aby byl proud dostatecny na otoceni
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minPower"></param>
        /// <returns></returns>
        private byte mapToPowerRange(decimal value, int minPower = 15)
        {
            return (byte)Math.Round(value / 100 * (100 - minPower) + minPower); 
        }
    }

    public class DevicesManager
    {
        public event EventHandler<MotorsValues> MotorsValuesChanged;



        GamePad gamePad;


        private int direcitionX = 0, direcitionY = 0;




        public DevicesManager()
        {
            gamePad = new GamePad();
            SetListeners();
            _ = gamePad.Run();
        }



        private void SetListeners()
        {
            #region direction, speed
            
            //direction
            gamePad.RightAxis_X_Changed += (s, e) =>
            {
                direcitionX = e;
                //Debug.WriteLine("Right_X: " + e);

                MotorsValuesChanged?.Invoke(this, new MotorsValues(direcitionY, direcitionX));

            };

            //speed
            gamePad.LeftAxis_Y_Changed += (s, e) =>
            {
                direcitionY = e;
                //Debug.WriteLine("Left_Y: " + e);
                MotorsValuesChanged?.Invoke(this, new MotorsValues(direcitionY, direcitionX));
            };


            //aktuálně nezajímá
            gamePad.RightAxis_Y_Changed += (s, e) =>
            {
                //Debug.WriteLine("Right_Y: " + e);
                //SendDataToContrib("Right_Y: " + e);

            };

            gamePad.LeftAxis_X_Changed += (s, e) =>
            {
               // Debug.WriteLine("Left_X: " + e);

            };
            #endregion


            gamePad.LeftTrigger_Axis_Changed += (s, e) =>
            {
                Debug.WriteLine("LTrigger: " + e);
            };
            gamePad.RightTrigger_Axis_Changed += (s, e) =>
            {
                Debug.WriteLine("RTrigger: " + e);
            };
            gamePad.LeftTrigger_Btn_Changed += (s, e) =>
            {
                Debug.WriteLine("LTriggerBtn: " + e.ToString());
            };
            gamePad.RightTrigger_Btn_Changed += (s, e) =>
            {
                Debug.WriteLine("RTriggerBtn: " + e.ToString());
            };

            gamePad.LeftBtns_LeftBtn_Changed += (s, e) =>
            {
                Debug.WriteLine("LB_Left: " + e.ToString());
            };
            gamePad.LeftBtns_TopBtn_Changed += (s, e) =>
            {
                Debug.WriteLine("LB_Top: " + e.ToString());
            };
            gamePad.LeftBtns_RightBtn_Changed += (s, e) =>
            {
                Debug.WriteLine("LB_Right: " + e.ToString());
            };
            gamePad.LeftBtns_BottomBtn_Changed += (s, e) =>
            {
                Debug.WriteLine("LB_Bottom: " + e.ToString());
            };

            gamePad.RightBtns_LeftBtn_Changed += (s, e) =>
            {
                Debug.WriteLine("RB_Left: " + e.ToString());
            };
            gamePad.RightBtns_TopBtn_Changed += (s, e) =>
            {
                Debug.WriteLine("RB_Top: " + e.ToString());
            };
            gamePad.RightBtns_RightBtn_Changed += (s, e) =>
            {
                Debug.WriteLine("RB_Right: " + e.ToString());
            };
            gamePad.RightBtns_BottomBtn_Changed += (s, e) =>
            {
                Debug.WriteLine("RB_Bottom: " + e.ToString());
            };

        }



    }
}
