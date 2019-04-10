using System;
using System.Collections.Generic;
using Microsoft.DirectX.DirectInput;

namespace WindowsFormsApp1
{
    class VJoyHelper
    {
        int numOfAxes;
        int numOfButtons;

        public static VJoyHelper GetInstance(string guid)
        {
            var device = new Device(Guid.Parse(guid));

            return new VJoyHelper()
            {
                numOfAxes = device.Caps.NumberAxes,
                numOfButtons = device.Caps.NumberButtons,
            };
        }

        public InputState ToInputState(JoystickState joystickState)
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
            for (int i = 0; i < numOfButtons; i++)
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
}
