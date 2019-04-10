using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        readonly APIs api = new APIs();
        readonly Dictionary<string, string> vjoyKeyName = new Dictionary<string, string>()
        {
            {"1", "lp"},
            {"4", "mp"},
            {"6", "hp"},
            {"2", "lk"},
            {"3", "mk"},
            {"8", "hk"},
            {"10", "start"},
            {"9", "back"},
            {"11", "record"},
            {"12", "play"},
            {"5", "lb"},
            {"7", "lt"},
        };

        readonly Dictionary<string, string> xinputKeyName = new Dictionary<string, string>()
        {
            {"3", "lp"},
            {"4", "mp"},
            {"6", "hp"},
            {"1", "lk"},
            {"2", "mk"},
            {"11", "hk"},
            {"8", "start"},
            {"7", "back"},
            {"9", "record"},
            {"10", "play"},
            {"5", "lb"},
            {"12", "lt"},
        };

        bool recording = false;
        string[] targetGuids = null;

        public Form1()
        {
            InitializeComponent();

        }

        private void startRecording_Click(object sender, EventArgs e)
        {
            if (recording)
            {
                recorded1.Clear();
                Dictionary<string, List<InputState>> inputStates = api.StopRecord();

                if (targetGuids.Length > 0)
                {
                    recorded1.Text = InputStatesToString(inputStates[targetGuids[0]]);
                }
                if (targetGuids.Length > 1)
                {
                    recorded2.Text = InputStatesToString(inputStates[targetGuids[1]]);
                }

                startRecording.Text = "Start Recording";
                recording = false;
            }
            else
            {
                targetGuids = joystickList.SelectedItems.OfType<Joystick>()
                    .Take(2)
                    .Select(j => j.Id)
                    .ToArray();

                api.StartRecord(targetGuids);
                recording = true;

                recorded1.Clear();
                recorded2.Clear();
                startRecording.Text = "Recording...";
            }
        }

        private string InputStatesToString(List<InputState> inputStates)
        {
            string result = "";
            foreach (InputState state in inputStates)
            {
                string buttons = "";
                if (state.Buttons.Length > 0)
                {
                    buttons = "," + state.Buttons
                        .Select(x => xinputKeyName[x.ToString()])
                        .Aggregate((s1, s2) => s1 + "," + s2);
                }

                result += state.Direction + buttons + ":" + state.Duration + "\r\n";
            }

            return result;
        }

        private void back_Click(object sender, EventArgs e)
        {
            Thread.Sleep(2000);

            api.Push(1, 9);

            Thread.Sleep(166);

            api.Release(1, 9);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            api.InitvJoy();

            joystickList.DataSource = api.GetJoySticks();
            joystickList.DisplayMember = "Name";
            joystickList.ValueMember = "Id";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            api.RelasevJoy();
        }

        private void runMacro_Click(object sender, EventArgs e)
        {
            if (macro1.Text.Trim().Length > 0 && macro2.Text.Trim().Length > 0)
            {
                api.RunMacro(PreProcessMacro(macro1.Text), PreProcessMacro(macro2.Text));
            }
            else if (macro1.Text.Length > 0)
            {
                api.RunMacro(1, PreProcessMacro(macro1.Text));
            }
            else if (macro2.Text.Length > 0)
            {
                api.RunMacro(2, PreProcessMacro(macro2.Text));
            }
        }

        private void stopMacro_Click(object sender, EventArgs e)
        {
            api.StopMacro();
        }

        private string[] PreProcessMacro(string macro)
        {
            List<string> result = new List<string>();

            string[] lines = macro.Trim().Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            foreach (string line in lines)
            {
                string lower = line.Trim().ToLower();
                if (lower.Length == 0)
                {
                    continue;
                }

                Match match = Regex.Match(lower, "\\d+$");
                string duration = match.Value;

                string direction = "5";
                match = Regex.Match(lower.Substring(0, match.Index), "\\d");
                if (match.Success)
                {
                    direction = match.Value;
                }

                List<string> buttons = new List<string>();

                foreach (KeyValuePair<string, string> keyNamePair in vjoyKeyName)
                {
                    if (lower.Contains(keyNamePair.Value))
                    {
                        buttons.Add(keyNamePair.Key);
                    }
                }
                result.Add(direction + ":" + string.Join(",", buttons) + ":" + duration);
            }

            return result.ToArray();
        }


        public void Test2()
        {
            var result = new List<long>(1000);

            long fps = 60000;
            long oneSec = 1000000000;

            int count = 0;
            long origin = Now();
            for (int i = 0; i < 60; i++)
            {
                long start = origin + 10 * i * oneSec / fps;
                while (start > Now())
                {
                    Thread.Sleep(TimeSpan.FromTicks(1));
                    count++;
                }

                result.Add(Now());

                long sleepTo = origin + 10 * (i + 1) * oneSec / fps;
                long sleepTime = sleepTo - Now() - 16000;
                Thread.Sleep(TimeSpan.FromMilliseconds(sleepTime / 1000));
            }

            result.ForEach(Console.WriteLine);
            Console.WriteLine(count);
        }
        private long Now()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            return stopwatch.ElapsedTicks * 1000000 / Stopwatch.Frequency;
        }


        public void Test1()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            int tick = 10 * 1000 * 1000;
            int fps = 60;

            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.WriteLine(stopwatch.Elapsed.Ticks);
            Thread.Sleep(TimeSpan.FromTicks(10 * tick / fps));
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.WriteLine(stopwatch.Elapsed.Ticks);

        }

        public void Test()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = new List<long>(1000000);

            long secInTick = 10 * 1000 * 1000;
            long fps = 60;

            int count = 0;
            long origin = stopwatch.Elapsed.Ticks;
            for (int i = 0; i < 60; i++)
            {
                long start = origin + 10 * i * secInTick / fps;
                while (start > stopwatch.Elapsed.Ticks)
                {
                    Thread.Sleep(TimeSpan.FromTicks(1));
                    count++;
                }

                result.Add(Now());

                long sleepTo = origin + 10 * (i + 1) * secInTick / fps;
                long sleepTime = sleepTo - stopwatch.Elapsed.Ticks - 200;
                if (sleepTime > 0) Thread.Sleep(TimeSpan.FromTicks(sleepTime));
            }

            result.ForEach(Console.WriteLine);
            Console.WriteLine(count);
        }
    }
}
