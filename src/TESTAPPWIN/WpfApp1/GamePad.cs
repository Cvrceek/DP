using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace WpfApp1
{
    public class GamePad
    {
        #region Events
        public event EventHandler<bool> Connection_Changed;

        public event EventHandler<int> LeftAxis_X_Changed;
        public event EventHandler<int> LeftAxis_Y_Changed;
                                 
        public event EventHandler<int> RightAxis_X_Changed;
        public event EventHandler<int> RightAxis_Y_Changed; 
                                 
        public event EventHandler<int> LeftTrigger_Axis_Changed; 
        public event EventHandler<int> RightTrigger_Axis_Changed; 

        public event EventHandler<bool> LeftTrigger_Btn_Changed;
        public event EventHandler<bool> RightTrigger_Btn_Changed;
                                 
        public event EventHandler<bool> LeftBtns_LeftBtn_Changed;
        public event EventHandler<bool> LeftBtns_TopBtn_Changed;
        public event EventHandler<bool> LeftBtns_RightBtn_Changed;
        public event EventHandler<bool> LeftBtns_BottomBtn_Changed;
                                 
        public event EventHandler<bool> RightBtns_LeftBtn_Changed;
        public event EventHandler<bool> RightBtns_TopBtn_Changed;
        public event EventHandler<bool> RightBtns_RightBtn_Changed;
        public event EventHandler<bool> RightBtns_BottomBtn_Changed;
        #endregion

        private const int centerAxisTolerance = 6;

        private GamePadState state;
        private DirectInput directInput;
        private bool isGamepadConnected;

        public GamePad()
        {
            state = new GamePadState();
            directInput = new DirectInput();
            isGamepadConnected = false;
        }

        public List<DeviceInstance> AvailableGamePads
        {
            get
            {
                return directInput
                    .GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)
                    .Concat(directInput.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AllDevices))
                    .Concat(directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                    .ToList();
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task Run()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var joystick = new Joystick(directInput, AvailableGamePads[0].InstanceGuid);
            joystick.Acquire();

            while (isConnected(0))
            {
                joystick.Poll();
                CompareStatesAndInvokeEvents(new GamePadState(joystick.GetCurrentState()));
                await Task.Delay(250);
            }
        }

        private bool isConnected(int deviceIndex)
        {
            var connected = directInput.IsDeviceAttached(AvailableGamePads[deviceIndex].InstanceGuid);
            if (isGamepadConnected != connected)
                Connection_Changed?.Invoke(this, connected);
            return connected;
        }

        private void CompareStatesAndInvokeEvents(GamePadState actualState)
        {
            //axis
          
            if (Math.Abs(state.LeftAxis_Y - actualState.LeftAxis_Y) >= centerAxisTolerance)
                LeftAxis_Y_Changed?.Invoke(this, ((2 * actualState.LeftAxis_Y) - 100) * -1);
            if (state.RightAxis_X != actualState.RightAxis_X)
                RightAxis_X_Changed?.Invoke(this, (2 * actualState.RightAxis_X) - 100);


            if (Math.Abs(state.LeftAxis_X - actualState.LeftAxis_X) >= centerAxisTolerance)
                LeftAxis_X_Changed?.Invoke(this, 2 * actualState.LeftAxis_X - 100);
            if (Math.Abs(state.RightAxis_Y - actualState.RightAxis_Y) >= centerAxisTolerance)
                RightAxis_Y_Changed?.Invoke(this, 2 * actualState.RightAxis_Y - 100);

            //triggers
            if (Math.Abs(state.LeftTrigger_Axis - actualState.LeftTrigger_Axis) >= centerAxisTolerance)
                LeftTrigger_Axis_Changed?.Invoke(this, actualState.LeftTrigger_Axis);
            if (Math.Abs(state.RightTrigger_Axis - actualState.RightTrigger_Axis) >= centerAxisTolerance)
                RightTrigger_Axis_Changed?.Invoke(this, actualState.RightTrigger_Axis);
            if (state.LeftTrigger_Btn != actualState.LeftTrigger_Btn)
                LeftTrigger_Btn_Changed?.Invoke(this, actualState.LeftTrigger_Btn);
            if (state.RightTrigger_Btn != actualState.RightTrigger_Btn)
                RightTrigger_Btn_Changed?.Invoke(this, actualState.RightTrigger_Btn);

            //leftBTNS
            if (state.LeftBtns_LeftBtn != actualState.LeftBtns_LeftBtn)
                LeftBtns_LeftBtn_Changed?.Invoke(this, actualState.LeftBtns_LeftBtn);
            if (state.LeftBtns_TopBtn != actualState.LeftBtns_TopBtn)
                LeftBtns_TopBtn_Changed?.Invoke(this, actualState.LeftBtns_TopBtn);
            if (state.LeftBtns_RightBtn != actualState.LeftBtns_RightBtn)
                LeftBtns_RightBtn_Changed?.Invoke(this, actualState.LeftBtns_RightBtn);
            if (state.LeftBtns_BottomBtn != actualState.LeftBtns_BottomBtn)
                LeftBtns_BottomBtn_Changed?.Invoke(this, actualState.LeftBtns_BottomBtn);

            //rightBTNS
            if (state.RightBtns_LeftBtn != actualState.RightBtns_LeftBtn)
                RightBtns_LeftBtn_Changed?.Invoke(this, actualState.RightBtns_LeftBtn);
            if (state.RightBtns_TopBtn != actualState.RightBtns_TopBtn)
                RightBtns_TopBtn_Changed?.Invoke(this, actualState.RightBtns_TopBtn);
            if (state.RightBtns_RightBtn != actualState.RightBtns_RightBtn)
                RightBtns_RightBtn_Changed?.Invoke(this, actualState.RightBtns_RightBtn);
            if (state.RightBtns_BottomBtn != actualState.RightBtns_BottomBtn)
                RightBtns_BottomBtn_Changed?.Invoke(this, actualState.RightBtns_BottomBtn);

            state = actualState;
        }

        #region GamePadState
        /// <summary>
        /// Pomocna trida pro stav gamepadu
        /// </summary>
        private class GamePadState
        {
            //hodnoty pro Axis od 0 do 100, center = 50
            public int LeftAxis_X { get; set; }
            public int LeftAxis_Y { get; set; }

            public int RightAxis_X { get; set; }
            public int RightAxis_Y { get; set; }

            public int LeftTrigger_Axis { get; set; }
            public int RightTrigger_Axis { get; set; }

            public bool LeftTrigger_Btn { get; set; }
            public bool RightTrigger_Btn { get; set; }

            public bool LeftBtns_LeftBtn { get; set; }
            public bool LeftBtns_TopBtn { get; set; }
            public bool LeftBtns_RightBtn { get; set; }
            public bool LeftBtns_BottomBtn { get; set; }

            public bool RightBtns_LeftBtn { get; set; }
            public bool RightBtns_TopBtn { get; set; }
            public bool RightBtns_RightBtn { get; set; }
            public bool RightBtns_BottomBtn { get; set; }

            public GamePadState()
            {
                RightAxis_X = 50;
                RightAxis_Y = 50;
                LeftAxis_X = 50;
                LeftAxis_Y = 50;
            }

            public GamePadState(JoystickState state)
            {
                var tempRightAxis_X = GetNormailizedValue(state.Z);
                RightAxis_X = tempRightAxis_X >= 50 - centerAxisTolerance && tempRightAxis_X <= 50 + centerAxisTolerance ? 50 : tempRightAxis_X;

                var tempRightAxis_Y = GetNormailizedValue(state.RotationZ);
                RightAxis_Y = tempRightAxis_Y >= 50 - centerAxisTolerance && tempRightAxis_Y <= 50 + centerAxisTolerance ? 50 : tempRightAxis_Y;

                var tempLeftAxis_X = GetNormailizedValue(state.X);
                LeftAxis_X = tempLeftAxis_X >= 50 - centerAxisTolerance && tempLeftAxis_X <= 50 + centerAxisTolerance ? 50 : tempLeftAxis_X;

                var tempLeftAxis_Y = GetNormailizedValue(state.Y);
                LeftAxis_Y = tempLeftAxis_Y >= 50 - centerAxisTolerance && tempLeftAxis_Y <= 50 + centerAxisTolerance ? 50 : tempLeftAxis_Y;

                LeftTrigger_Axis = GetNormailizedValue(state.RotationX);
                RightTrigger_Axis = GetNormailizedValue(state.RotationY);

                LeftTrigger_Btn = state.Buttons[4];
                RightTrigger_Btn= state.Buttons[5];

                RightBtns_LeftBtn = state.Buttons[0];
                RightBtns_TopBtn = state.Buttons[3];
                RightBtns_RightBtn = state.Buttons[2];
                RightBtns_BottomBtn = state.Buttons[1];

                LeftBtns_LeftBtn = state.PointOfViewControllers[0] == 27000;
                LeftBtns_TopBtn = state.PointOfViewControllers[0] == 0;
                LeftBtns_RightBtn = state.PointOfViewControllers[0] == 9000;
                LeftBtns_BottomBtn = state.PointOfViewControllers[0] == 18000;
            }

            private int GetNormailizedValue(int value)
            {
                return (value * 100) / 65535;
            }
        }
        #endregion

    }



}
