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

        private JoystickWatcher(Device device)
        {
            this.device = device;
        }

        public void AddEventHandler(Action<JoystickState> action)
        {
            eventHandlers.Add(action);
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
