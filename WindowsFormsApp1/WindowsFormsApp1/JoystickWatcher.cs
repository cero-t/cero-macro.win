using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.DirectX.DirectInput;

namespace WindowsFormsApp1
{
    class JoystickWatcher
    {
        private static readonly Dictionary<string, JoystickWatcher> instances = new Dictionary<string, JoystickWatcher>();

        readonly Device device;
        readonly List<Action<JoystickState>> eventHandlers = new List<Action<JoystickState>>();

        EventWaitHandle handle = null;
        Thread thread;
        bool running;

        public static JoystickWatcher Get(string guid)
        {
            if (instances.ContainsKey(guid) == false)
            {
                var device = new Device(Guid.Parse(guid));
                device.SetDataFormat(DeviceDataFormat.Joystick);
                instances[guid] = new JoystickWatcher(device);
            }

            return instances[guid];
        }

        public static JoystickWatcher[] GetAll()
        {
            JoystickWatcher[] watchers = new JoystickWatcher[instances.Count];
            instances.Values.CopyTo(watchers, 0);

            return watchers;
        }

        private JoystickWatcher(Device device)
        {
            this.device = device;
        }

        public void AddEventHandler(Action<JoystickState> action)
        {
            eventHandlers.Add(action);
        }

        public void RemoveEventHandler(Action<JoystickState> action)
        {
            eventHandlers.Remove(action);
        }

        public void Start(Barrier barrier = null)
        {
            running = true;

            if (handle == null)
            {
                handle = new EventWaitHandle(true, EventResetMode.AutoReset);
                device.SetEventNotification(handle);
            }
            device.Acquire();

            thread = new Thread(() => {
                if (barrier != null)
                {
                    barrier.SignalAndWait();
                }
                Watch();
            });
            thread.Start();
        }

        public void Stop()
        {
            if (thread.IsAlive)
            {
                thread.Abort();
            }

            device.Unacquire();

            running = false;
        }

        private void Watch()
        {
            JoystickState state = device.CurrentJoystickState;
            eventHandlers.ForEach(handler => handler.Invoke(state));

            try
            {
                while (running)
                {
                    handle.WaitOne();
                    state = device.CurrentJoystickState;
                    eventHandlers.ForEach(handler => handler.Invoke(state));
                }
            }
            catch (ThreadAbortException)
            {
                while (running)
                {
                    handle.WaitOne();
                    state = device.CurrentJoystickState;
                    eventHandlers.ForEach(handler => handler.Invoke(state));
                }
            }
        }
    }
}
