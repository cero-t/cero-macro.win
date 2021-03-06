﻿using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.DirectX.DirectInput;
using vJoyInterfaceWrap;

namespace WindowsFormsApp1
{
    class APIs
    {
        readonly vJoy vJoyIF = new vJoy();
        readonly List<JoystickWatcher> watchers = new List<JoystickWatcher>();
        readonly Dictionary<string, StateStorage> storages = new Dictionary<string, StateStorage>();
        readonly MacroRunner macroRunner;

        public APIs()
        {
            macroRunner = new MacroRunner(vJoyIF);
        }

        public List<Joystick> GetJoySticks()
        {
            var result = new List<Joystick>();

            DeviceList list = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
            foreach (DeviceInstance dev in list)
            {
                var joystick = new Joystick()
                {
                    Id = dev.InstanceGuid.ToString(),
                    Name = dev.InstanceName
                };
                result.Add(joystick);
            }

            return result;
        }

        public void StartRecord(params string[] guids)
        {
            // ToDo: 他のタスクに対するブロックとか入れたい。
            watchers.Clear();

            foreach (string guid in guids)
            {
                if (storages.ContainsKey(guid) == false)
                {
                    var storage = new StateStorage(VJoyHelper.GetInstance(guid));
                    var watcher = JoystickWatcher.Get(guid);
                    watcher.AddEventHandler(state => storage.Add(state));

                    watchers.Add(watcher);
                    storages[guid] = storage;
                }
                else
                {
                    watchers.Add(JoystickWatcher.Get(guid));
                    storages[guid].Clear();
                }
            }

            Barrier barrier = new Barrier(watchers.Count);
            watchers.ForEach(watcher => watcher.Start(barrier));
        }

        public Dictionary<string, List<InputState>> StopRecord()
        {
            Dictionary<string, List<InputState>> result = new Dictionary<string, List<InputState>>();
            foreach (JoystickWatcher watcher in watchers)
            {
                watcher.Stop();
            }

            foreach(KeyValuePair<string, StateStorage> pair in storages)
            {
                // ToDo: RawStateをStateに変換するのはStorageの仕事ではない。
                var states = pair.Value.GetStates();
                states.AddRange(pair.Value.GetRawStates());
                result[pair.Key] = states;
            }

            return result;
        }

        // ToDo: これはマクロの開始時と終了時で良いのでは？
        public void InitvJoy()
        {
            AcquireDevice(1);
            AcquireDevice(2);
            vJoyIF.ResetAll();
        }

        public void RelasevJoy()
        {
            vJoyIF.ResetAll();
            RelaseDevice(1);
            RelaseDevice(2);
        }

        private void AcquireDevice(uint vJoyId)
        {
            if (vJoyIF.GetVJDStatus(vJoyId) == VjdStat.VJD_STAT_OWN)
            {
                return;
            }
            else if (vJoyIF.GetVJDStatus(vJoyId) == VjdStat.VJD_STAT_FREE)
            {
                if (vJoyIF.AcquireVJD(vJoyId))
                {
                    Console.WriteLine("vJoy(" + vJoyId + ") aquired");
                }
                else
                {
                    Console.WriteLine("vJoy(" + vJoyId + ") couldn't be aquired!");
                }
            }
        }

        private void RelaseDevice(uint vJoyId)
        {
            if (vJoyIF.GetVJDStatus(vJoyId) == VjdStat.VJD_STAT_OWN)
            {
                vJoyIF.RelinquishVJD(vJoyId);
            }
        }

        public void Direction(uint vJoyId, int direction)
        {
            int x = (direction - 1) % 3; // 0:left 1:center 2:right
            int y = (direction - 1) / 3; // 0:down 1:center 2:up
            y = 2 - y; // 0:up 1:center 2:down

            vJoyIF.SetAxis(0x3FFF * x, vJoyId, HID_USAGES.HID_USAGE_X);
            vJoyIF.SetAxis(0x3FFF * y, vJoyId, HID_USAGES.HID_USAGE_Y);

            return;
        }

        public void Push(uint vJoyId, uint button)
        {
            vJoyIF.SetBtn(true, vJoyId, button);
        }

        public void Release(uint vJoyId, uint button)
        {
            vJoyIF.SetBtn(false, vJoyId, button);
        }

        // ToDo: マクロの実行中は他のタスクに対するブロックとか入れたい。
        public void RunMacro(uint vJoyId, string[] macroLines)
        {
            macroRunner.RunMacro(vJoyId, macroLines);
        }

        public void RunMacro(string[] macroLines1, string[] macroLines2)
        {
            macroRunner.RunMacro(macroLines1, macroLines2);
        }

        public void RegisterMacro(uint vJoyId, string[] macroLines, string startCommand, string guid)
        {
            watchers.Clear();

        }

        public void RegisterMacro(string[] macroLines1, string[] macroLines2, string startCommand, string guid)
        {
            watchers.Clear();

            var watcher = JoystickWatcher.Get(guid);
            watcher.AddEventHandler(state =>
            {
                if (state.ToString() == startCommand)
                {
                    macroRunner.RunMacro(macroLines1, macroLines2);
                    // 待ち受けをやめる
                }
            });
            watcher.Start();
        }

        public void StopMacro()
        {
            macroRunner.Stop();
        }
    }

    class Joystick
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
