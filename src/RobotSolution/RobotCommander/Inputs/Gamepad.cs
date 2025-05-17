using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SDL2.SDL;
using SDL2;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;


namespace RobotCommander.Inputs
{
    public class GamePad : IInputDevice
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

        private int holderPositon = 50;
        private bool lights = false;
        private bool horn = false;
        private bool beacon = false;
        private int externalSpeed = 0;

        private nint controller;

        State oldState;
        public GamePad()
        {
            oldState = new State();
            if (SDL_Init(SDL_INIT_GAMECONTROLLER) != 0)
            {
                Console.WriteLine($"SDL_Init Error: {SDL_GetError()}");
                return;
            }

            controller = 0;
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
        }


        public void Run()
        {
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

                SDL_Delay(100);
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

            if (oldState.DPad_UP != newState.DPad_UP)
            {
                if (newState.DPad_UP && holderPositon < 100)
                {
                    holderPositon += 10;
                    ExternalDeviceHolderPositionChanged?.Invoke(this, holderPositon);
                }
            }

            if (oldState.DPad_DOWN != newState.DPad_DOWN)
            {
                if (newState.DPad_DOWN && holderPositon > 0)
                {
                    holderPositon -= 10;
                    ExternalDeviceHolderPositionChanged?.Invoke(this, holderPositon);
                }
            }

            //trojuhelnik   SDL_CONTROLLER_BUTTON_Y
            //kolecko  	SDL_CONTROLLER_BUTTON_B
            //ctverecek   	SDL_CONTROLLER_BUTTON_X
            //krizek  SDL_CONTROLLER_BUTTON_A

            if (oldState.Btn_Y != newState.Btn_Y)
            {
                if (newState.Btn_Y && externalSpeed < 100)
                {
                    externalSpeed += 10;
                    ExternalDeviceSpeedChanged?.Invoke(this, externalSpeed);
                }
            }

            if (oldState.Btn_A != newState.Btn_A)
            {
                if (newState.Btn_A && externalSpeed > 0)
                {
                    externalSpeed -= 10;
                    ExternalDeviceSpeedChanged?.Invoke(this, externalSpeed);
                }
            }

            if (oldState.Btn_B != newState.Btn_B)
            {
                if (newState.Btn_B)
                    ExternalDevicePWM1Changed?.Invoke(this, 80);
                else
                    ExternalDevicePWM1Changed?.Invoke(this, 0);
            }

            if (oldState.Btn_X != newState.Btn_X)
            {
                if (newState.Btn_X)
                {
                    if (newState.Btn_B)
                        ExternalDevicePWM1Changed?.Invoke(this, 20);
                    else
                        ExternalDevicePWM1Changed?.Invoke(this, 0);
                }
            }


            if (oldState.Btn_R != newState.Btn_R)
            {
                if (newState.Btn_R)
                {
                    lights = !lights;
                    LightsChanged?.Invoke(this, lights);
                }
            }

            oldState = newState;
        }
      
     
        /// <summary>
        /// Třída pro načtení dat z gamepadu
        /// </summary>
        private class State
        {
            public int Speed;
            public int Direction;

            public bool DPad_UP;
            public bool DPad_DOWN;
            public bool DPad_RIGHT;
            public bool DPad_LEFT;

            public bool Btn_Y;
            public bool Btn_X;
            public bool Btn_A;
            public bool Btn_B;


            public int TriggerRight;
            public int TriggerLeft;

            public bool Btn_R;
            public bool Btn_L;

           
            private float maxValue = 32767.0f;
            private int deadZone = 10;


            public State() { }
            //trojuhelnik   SDL_CONTROLLER_BUTTON_Y
            //kolecko  	SDL_CONTROLLER_BUTTON_B
            //ctverecek   	SDL_CONTROLLER_BUTTON_X
            //krizek  SDL_CONTROLLER_BUTTON_A

            //D - Pad Nahoru SDL_CONTROLLER_BUTTON_DPAD_UP
            //D - Pad Dolů SDL_CONTROLLER_BUTTON_DPAD_DOWN
            //D - Pad Vlevo SDL_CONTROLLER_BUTTON_DPAD_LEFT
            //D - Pad Vpravo SDL_CONTROLLER_BUTTON_DPAD_RIGHT

            //L2(analog) SDL_CONTROLLER_AXIS_TRIGGERLEFT 
            //R2(analog) SDL_CONTROLLER_AXIS_TRIGGERRIGHT

            //L3(stick klik) SDL_CONTROLLER_BUTTON_LEFTSTICK
            //R3(stick klik) SDL_CONTROLLER_BUTTON_RIGHTSTICK
            public State(nint controller)
            {

                Speed = NormalizeValue(SDL_GameControllerGetAxis(controller, SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY)) * -1;
                Direction = NormalizeValue(SDL_GameControllerGetAxis(controller, SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX));


                DPad_UP = SDL_GameControllerGetButton(controller, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP) == 1;
                DPad_DOWN = SDL_GameControllerGetButton(controller, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN) == 1;
                DPad_RIGHT = SDL_GameControllerGetButton(controller, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT) == 1;
                DPad_LEFT = SDL_GameControllerGetButton(controller, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT) == 1;

                Btn_A = SDL_GameControllerGetButton(controller, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A) == 1;
                Btn_B = SDL_GameControllerGetButton(controller, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B) == 1;
                Btn_X = SDL_GameControllerGetButton(controller, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X) == 1;
                Btn_Y = SDL_GameControllerGetButton(controller, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y) == 1;

                TriggerLeft = NormalizeValue(SDL_GameControllerGetAxis(controller, SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT));
                TriggerRight = NormalizeValue(SDL_GameControllerGetAxis(controller, SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT));

                Btn_L = SDL_GameControllerGetButton(controller, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK) == 1;
                Btn_R = SDL_GameControllerGetButton(controller, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK) == 1;
            }
            /// <summary>
            /// Převede hodnoty na -100 <> 100
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            private int NormalizeValue(float value, bool useDeadZone = true)
            {
                var result = (int)Math.Ceiling((value / maxValue) * 100);
                return useDeadZone ? DeadZone(result) : result;
            }

            private int DeadZone(int value)
            {
                if (Math.Abs(value) < deadZone)
                    return 0;
                else
                    return value;
            }
        }
    }
}
