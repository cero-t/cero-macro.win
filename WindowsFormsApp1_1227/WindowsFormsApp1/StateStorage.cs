using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.DirectX.DirectInput;

namespace WindowsFormsApp1
{
    class StateStorage
    {
        readonly List<TimedState> list = new List<TimedState>();
        readonly Stopwatch sw = Stopwatch.StartNew();

        readonly int numOfButtons;
        readonly int numOfAxes;

        readonly long fps = 59800;
        readonly long oneSec = 1000000000000;

        public StateStorage(int numOfButtons, int numOfAxes)
        {
            this.numOfButtons = numOfButtons;
            this.numOfAxes = numOfAxes;
        }

        public void Add(JoystickState state)
        {
            list.Add(new TimedState()
            {
                JoyStickState = state,
                NanoTime = sw.ElapsedTicks * 1000000000 / Stopwatch.Frequency,
                Time = sw.Elapsed
            });
        }

        public void Clear()
        {
            list.Clear();
        }

        public List<InputState> GetStates()
        {
            var result = new List<InputState>();

            if (list.Count == 0) return result;
 
            long origin = list[0].NanoTime;
            long leftOver = oneSec / fps / 2;

            for (int i = 0; i < list.Count; i ++)
            {
                TimedState timedState = list[i];
                InputState inputState = ToInputState(timedState.JoyStickState);

                if (i != list.Count - 1)
                {
                    long duration = list[i + 1].NanoTime - timedState.NanoTime + leftOver;

                    int frames = (int)(duration * fps / oneSec);
                    if (frames == 0)
                    {
                        leftOver = duration;
                        continue;
                    }

                    inputState.Duration = frames;
                    result.Add(inputState);

                    leftOver = duration % (oneSec / fps);
                }
                // ignore last input because it should be the none input
            }

            return result;
        }

        private InputState ToInputState(JoystickState joystickState)
        {
            int direction;

            int pov = joystickState.GetPointOfView()[0];
            if (pov != -1)
            {
                int[] povRotation = { 8, 9, 6, 3, 2, 1, 4, 7 };
                direction = povRotation[pov / 4500];
            }
            else if ((joystickState.X != 0x7FFF && joystickState.X != 0x7FFE)
                || (joystickState.Y != 0x7FFF && joystickState.Y != 0x7FFE))
            {
                int x = joystickState.X / 0x7FFE + 1; // 1 to 3
                int y = (2 - joystickState.Y / 0x7FFE) * 3; // 0,3,6 from bottom
                direction = x + y;
            }
            else
            {
                direction = 5;
            }

            List<int> pushed = new List<int>();
            byte[] buttons = joystickState.GetButtons();
            for (int i = 0; i < numOfButtons; i ++)
            {
                if (buttons[i] > 0)
                {
                    pushed.Add(i + 1);
                }
            }

            // XInput like Hori RAP, QANBA DRONE
            if (numOfAxes > 2)
            {
                if (joystickState.Z < 0x0400)
                {
                    pushed.Add(numOfButtons + 1);
                }
                else if (joystickState.Z > 0xFC00)
                {
                    pushed.Add(numOfButtons + 2);
                }
            }

            return new InputState()
            {
                Direction = direction,
                Buttons = pushed.ToArray()
            };
        }
    }

    class InputState
    {
        public int Direction;
        public int[] Buttons;
        public int Duration;
    }

    class TimedState
    {
        public JoystickState JoyStickState;
        public long NanoTime;
        public TimeSpan Time;
    }
}
