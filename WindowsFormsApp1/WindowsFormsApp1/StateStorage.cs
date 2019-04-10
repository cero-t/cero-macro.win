using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.DirectX.DirectInput;

namespace WindowsFormsApp1
{
    class StateStorage
    {
        readonly VJoyHelper vJoyHelper;
        
        readonly List<TimedState> list = new List<TimedState>();
        readonly Stopwatch sw = Stopwatch.StartNew();

        readonly long fps = 59800;
        readonly long oneSec = 1000000000000;

        public StateStorage(VJoyHelper vJoyHelper)
        {
            this.vJoyHelper = vJoyHelper;
        }

        public void Add(JoystickState state)
        {
            list.Add(new TimedState()
            {
                JoyStickState = state,
                NanoTime = sw.Elapsed.Ticks * 100
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

            for (int i = 0; i < list.Count; i++)
            {
                TimedState timedState = list[i];
                InputState inputState = vJoyHelper.ToInputState(timedState.JoyStickState);

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

        public List<InputState> GetRawStates()
        {
            return list.Select((s) =>
            {
                InputState inputState = vJoyHelper.ToInputState(s.JoyStickState);
                inputState.Duration = s.NanoTime;
                return inputState;
            })
            .ToList();
        }
    }

    class InputState
    {
        public int Direction;
        public int[] Buttons;
        public long Duration;
    }

    class TimedState
    {
        public JoystickState JoyStickState;
        public long NanoTime;
    }
}
