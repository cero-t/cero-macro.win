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
        readonly long fps = 5980; // 100 times
        readonly long secInTick = 1000 * 1000 * 1000; // 100 times
        bool stopped = false;

        public MacroRunner(vJoy vJoy)
        {
            this.vJoyIF = vJoy;
        }

        public void RunMacro(uint vJoyId, string[] macroLines)
        {
            stopped = false;

            if (macroLines.Length > 0 && macroLines[0].StartsWith("#"))
            {
                InputState[] startCommands = ParseCommand(macroLines[0]);
            }
            else
            {
                new Thread(() => DoRunMacro(vJoyId, ParseMacro(macroLines))).Start();
            }
        }

        void hoge(InputState[] expectedStates)
        {
            foreach (JoystickWatcher watcher in JoystickWatcher.GetAll())
            {
                watcher.AddEventHandler((joystickState) =>
                {
//                    InputState inputState = VJoyHelper.ToInputState(joystickState, 2, 8);

                    foreach (InputState expectedState in expectedStates)
                    {
                        // ハンドラを除外する (除外しないオプションがあってもいい)
                        // DoRunMacro実行
                    }
                });
            }
        }

        public void RunMacro(string[] macroLines1, string[] macroLines2)
        {
            stopped = false;

            Barrier barrier = new Barrier(2);
            new Thread(() => DoRunMacro(1, ParseMacro(macroLines1), () => barrier.SignalAndWait())).Start();
            new Thread(() => DoRunMacro(2, ParseMacro(macroLines2), () => barrier.SignalAndWait())).Start();
        }

        public void Stop()
        {
            stopped = true;
        }

        private void DoRunMacro(uint vJoyId, List<VJoyInput> macro, Action startAction = null)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            vJoyIF.SetAxis(0x4000, vJoyId, HID_USAGES.HID_USAGE_X);
            vJoyIF.SetAxis(0x4000, vJoyId, HID_USAGES.HID_USAGE_Y);
            vJoyIF.ResetButtons(vJoyId);

            if (startAction != null)
            {
                startAction.Invoke();
            }

            long frames = 0;
            long origin = stopwatch.Elapsed.Ticks;
            foreach (VJoyInput input in macro)
            {
                if (stopped)
                {
                    return;
                }

                // precise sleep
                long start = origin + frames * secInTick / fps;
                while (start > stopwatch.Elapsed.Ticks)
                {
                    Thread.Sleep(TimeSpan.FromTicks(1));
                }

                vJoyIF.SetAxis(input.X, vJoyId, HID_USAGES.HID_USAGE_X);
                vJoyIF.SetAxis(input.Y, vJoyId, HID_USAGES.HID_USAGE_Y);
                input.Release.ForEach(button => vJoyIF.SetBtn(false, vJoyId, button));
                input.Push.ForEach(button => vJoyIF.SetBtn(true, vJoyId, button));

                frames += input.Duration;

                // rough sleep
                long sleepTo = origin + frames * secInTick / fps;
                long sleepTime = sleepTo - stopwatch.Elapsed.Ticks - 200;
                if (sleepTime > 0) Thread.Sleep(TimeSpan.FromTicks(sleepTime));
            }

            vJoyIF.SetAxis(0x4000, vJoyId, HID_USAGES.HID_USAGE_X);
            vJoyIF.SetAxis(0x4000, vJoyId, HID_USAGES.HID_USAGE_Y);
            vJoyIF.ResetButtons(vJoyId);
        }

        private List<VJoyInput> ParseMacro(string[] lines)
        {
            var result = new List<VJoyInput>();
            bool[] lastButtons = null;

            foreach (string line in lines)
            {
                if (line.StartsWith("#"))
                {
                    continue;
                }

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
            // 5::
            string[] items = line.Split(':');

            // ToDo: 20固定にしてしまっている。vJoyIFから取りたい。
            bool[] buttons = items[1]
                .Split(',')
                .Where(s => s.Length > 0)
                .Select(s => int.Parse(s))
                .Aggregate(new bool[20], (array, x) => {
                    array[x] = true;
                    return array;
                });

            return new InputState()
            {
                Direction = int.Parse(items[0]),
                Buttons = buttons,
                Duration = items.Length >= 2 ? int.Parse(items[2]) : 0,
            };
        }

        private InputState[] ParseCommand(string line)
        {
            // #startWhen:2:1,2,5
            // #startWhen:2
            // #startWhen:0:1,2,
            // #startWhen:2:1/2:5
            // #startWhen:2:1/0:5
            return line.Substring(11)
                .Split('/')
                .Select(ParseLine)
                .ToArray();
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
