using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SDL2.SDL;
using SDL2;
using System.Diagnostics;


namespace RobotCommander.Inputs
{
    public class GamePad : IRobotCommanderInput
    {
        #region Events
        public event EventHandler<int> SpeedChanged;
        public event EventHandler<int> DirectionChanged;
        public event EventHandler<int> ExternalDeviceSpeedChanged;
        public event EventHandler<int> ExternalDevicePWM1Changed;
        public event EventHandler<int> ExternalDevicePWM2Changed;
        public event EventHandler<int> ExternalDeviceHolderPositionChanged;
        public event EventHandler<bool> LightsChanged;
        public event EventHandler<bool> BeaconChanged;
        public event EventHandler<bool> HornChanged;
        #endregion

        private int holderPositon;
        private bool lights;
        private bool horn;
        private bool beacon;
        private int externalSpeed;


        State oldState;
        public GamePad()
        {
            oldState = new State();
            if (SDL_Init(SDL_INIT_GAMECONTROLLER) != 0)
            {
                Console.WriteLine($"SDL_Init Error: {SDL_GetError()}");
                return;
            }

            nint controller = 0;
            for (int i = 0; i < SDL_NumJoysticks(); ++i)
            {
                if (SDL_IsGameController(i) == SDL_bool.SDL_TRUE)
                {
                    controller = SDL_GameControllerOpen(i);
                    if (controller != 0)
                    {
                        Console.WriteLine($"Gamepad {SDL_GameControllerName(controller)} otevřen.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Nelze otevřít gamepad {i}: {SDL_GetError()}");
                    }
                }
            }

       


            SDL_Event e;
            bool quit = false;
            while (!quit)
            {
                while (SDL_PollEvent(out e) != 0)
                {
                    if (e.type == SDL_EventType.SDL_QUIT)
                    {
                        quit = true;
                    }
                }

                InvokeEvents(new(controller));

                SDL_Delay(10);
            }

            SDL_GameControllerClose(controller);
            SDL_Quit();
        }


        private void InvokeEvents(State newState)
        {
            if (oldState.Speed != newState.Speed)
                SpeedChanged?.Invoke(this, newState.Speed);
            if (oldState.Direction != newState.Direction)
                DirectionChanged?.Invoke(this, newState.Direction);

            if (oldState.ExternalDeviceSpeed != newState.ExternalDeviceSpeed)
                ExternalDeviceSpeedChanged?.Invoke(this, newState.ExternalDeviceSpeed);
            if (oldState.ExternalDevicePWM1 != newState.ExternalDevicePWM1)
                ExternalDevicePWM1Changed?.Invoke(this, newState.ExternalDevicePWM1);
            if (oldState.ExternalDevicePWM2 != newState.ExternalDevicePWM2)
                ExternalDevicePWM2Changed?.Invoke(this, newState.ExternalDevicePWM2);
            if (oldState.ExternalDeviceHolderPosition != newState.ExternalDeviceHolderPosition)
                ExternalDeviceHolderPositionChanged?.Invoke(this, newState.ExternalDeviceHolderPosition);
            if (oldState.Lights != newState.Lights)
                LightsChanged?.Invoke(this, newState.Lights);
            if (oldState.Beacon != newState.Beacon)
                BeaconChanged?.Invoke(this, newState.Beacon);
            if (oldState.Horn != newState.Horn)
                HornChanged?.Invoke(this, newState.Horn);

            oldState = newState;
        }
      
     
        /// <summary>
        /// Třída pro načtení dat z gamepadu
        /// </summary>
        private class State
        {
            public int Speed;
            public int Direction;
            public int ExternalDeviceSpeed;
            public int ExternalDevicePWM1;
            public int ExternalDevicePWM2;
            public int ExternalDeviceHolderPosition;
            public bool Lights;
            public bool Beacon;
            public bool Horn;
            
            private float maxValue = 32767.0f;

            public State() { }
            public State(nint controller)
            {

                Speed = NormalizeValue(SDL_GameControllerGetAxis(controller, SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY)) * -1;
                Direction = NormalizeValue(SDL_GameControllerGetAxis(controller, SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX));
#if DEBUG
                Debug.WriteLine($"Speed: {Speed}\tDirection: {Direction}");
#endif
            }
            /// <summary>
            /// Převede hodnoty na -100 <> 100
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int NormalizeValue(float value)
            {
                return (int)Math.Ceiling((value / maxValue) * 100);
            }
        }
    }
}
