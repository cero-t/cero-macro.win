using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using vJoyInterfaceWrap;

namespace WindowsFormsApp1
{
    class MacroRunner
    {
        readonly vJoy vJoyIF;
        readonly Stopwatch sw = Stopwatch.StartNew();
        readonly long fps = 59800;
        readonly long oneSec = 1000000000000;
        bool stopped = false;

        public MacroRunner(vJoy vJoy)
        {
            this.vJoyIF = vJoy;
        }

        public void RunMacro(uint vJoyId, string[] macroLines)
        {
            stopped = false;

            new Thread(() => DoRunMacro(vJoyId, ParseMacro(macroLines))).Start();
        }

        public void RunMacro(string[] macroLines1, string[] macroLines2)
        {
            stopped = false;

            Barrier barrier = new Barrier(2);
            new Thread(() => DoRunMacro(1, ParseMacro(macroLines1), barrier)).Start();
            new Thread(() => DoRunMacro(2, ParseMacro(macroLines2), barrier)).Start();
        }

        public void Stop()
        {
            stopped = true;
        }

        private void DoRunMacro(uint vJoyId, List<VJoyInput> macro, Barrier barrier = null)
        {
            vJoyIF.SetAxis(0x4000, vJoyId, HID_USAGES.HID_USAGE_X);
            vJoyIF.SetAxis(0x4000, vJoyId, HID_USAGES.HID_USAGE_Y);
            vJoyIF.ResetButtons(vJoyId);

            if (barrier != null)
            {
                barrier.SignalAndWait();
            }

            long delta = 0;
            long afterLastSleep = Now();
            foreach (VJoyInput input in macro)
            {
                if (stopped)
                {
                    return;
                }

                vJoyIF.SetAxis(input.X, vJoyId, HID_USAGES.HID_USAGE_X);
                vJoyIF.SetAxis(input.Y, vJoyId, HID_USAGES.HID_USAGE_Y);
                input.Release.ForEach(button => vJoyIF.SetBtn(false, vJoyId, button));
                input.Push.ForEach(button => vJoyIF.SetBtn(true, vJoyId, button));

                long beforeSleep = Now();
                long sleepTime = input.Duration * oneSec / fps - (beforeSleep - afterLastSleep) - delta;

                Thread.Sleep(TimeSpan.FromMilliseconds(sleepTime / 1000000));

                afterLastSleep = Now();
                delta = afterLastSleep - beforeSleep - sleepTime;
            }

            vJoyIF.SetAxis(0x4000, vJoyId, HID_USAGES.HID_USAGE_X);
            vJoyIF.SetAxis(0x4000, vJoyId, HID_USAGES.HID_USAGE_Y);
            vJoyIF.ResetButtons(vJoyId);
        }

        private long Now()
        {
            return sw.ElapsedTicks * 1000000000 / Stopwatch.Frequency;
        }

        private List<VJoyInput> ParseMacro(string[] lines)
        {
            var result = new List<VJoyInput>();
            bool[] lastButtons = null;

            foreach (string line in lines)
            {
                var state = ParseLine(line);

                int x = (state.Direction - 1) % 3; // 0:left 1:center 2:right
                int y = (state.Direction - 1) / 3; // 0:down 1:center 2:up
                y = 2 - y; // 0:up 1:center 2:down

                x = 0x4000 * x;
                y = 0x4000 * y;

                var push = new List<uint>();
                var release = new List<uint>();

                if (lastButtons == null)
                {
                    for (uint i = 0; i < state.Buttons.Length; i++)
                    {
                        if (state.Buttons[i])
                        {
                            push.Add(i);
                        }
                    }
                }
                else
                {
                    for (uint i = 0; i < state.Buttons.Length; i++)
                    {
                        if (lastButtons[i] != state.Buttons[i])
                        {
                            if (state.Buttons[i])
                            {
                                push.Add(i);
                            }
                            else
                            {
                                release.Add(i);
                            }
                        }
                    }
                }
                lastButtons = state.Buttons;

                result.Add(new VJoyInput()
                {
                    X = x,
                    Y = y,
                    Push = push,
                    Release = release,
                    Duration = state.Duration
                });
            }

            return result;
        }

        private InputState ParseLine(string line)
        {
            // direction:buttons(comma separated):duration
            // ex)
            // 6:1,2,5:12
            // 2:5:12
            // 5::12
            int posDuration = line.LastIndexOf(':');
            bool[] buttons = line.Substring(2, posDuration - 2)
                .Split(',')
                .Where(s => s.Length > 0)
                .Select(s => int.Parse(s))
                .Aggregate(new bool[20], (array, x) => {
                    array[x] = true;
                    return array;
                });


            return new InputState()
            {
                Direction = int.Parse(line.Substring(0, 1)),
                Duration = int.Parse(line.Substring(posDuration + 1)),
                Buttons = buttons
            };
        }

        class InputState
        {
            public int Direction;
            public bool[] Buttons;
            public int Duration;
        }

        class VJoyInput
        {
            public int X;
            public int Y;
            public List<uint> Push;
            public List<uint> Release;
            public int Duration;
        }
    }
}
