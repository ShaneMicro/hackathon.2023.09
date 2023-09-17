using Hackathon2023_MqttClient;
using System.Runtime.InteropServices;
using static Hackathon2023_MqttClient.KeyboardDatatypes;

class KeyboardHelper
{
    [DllImport("user32.dll")]
    public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    private static extern IntPtr GetMessageExtraInfo();

    public static void Run(string command)
    {
        INPUT[] inputs = { };

        switch (command)
        {
            case "miconoff":
                inputs = CreateInputs(new KEYBOARD_KEY[] { KEYBOARD_KEY.CTRL, KEYBOARD_KEY.SHIFT, KEYBOARD_KEY.M });
                break;
            case "camonoff":
                inputs = CreateInputs(new KEYBOARD_KEY[] { KEYBOARD_KEY.CTRL, KEYBOARD_KEY.SHIFT, KEYBOARD_KEY.O });
                break;
            case "leavecall":
                inputs = CreateInputs(new KEYBOARD_KEY[] { KEYBOARD_KEY.CTRL, KEYBOARD_KEY.SHIFT, KEYBOARD_KEY.H });
                break;
            case "raisehand":
                inputs = CreateInputs(new KEYBOARD_KEY[] { KEYBOARD_KEY.CTRL, KEYBOARD_KEY.SHIFT, KEYBOARD_KEY.K });
                break;
            default:
                break;
        }

        // Send the keyboard events
        uint result = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

   

    private static INPUT[] CreateInputs(KEYBOARD_KEY[] keys)
    {
        INPUT[] inputs = new INPUT[keys.Length * 2]; //considering commands to press the button down and up
        ushort inputsIndex = 0;

        //Populating KEY DOWN commands
        for (int i = 0; i < keys.Length; i++)
        {
            INPUT input = new INPUT();
            input = new INPUT
            {
                type = 1,
                union = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        dwFlags = (uint)(KeyEventF.KeyDown),
                        wVk = (ushort)keys[i]
                    }
                }
            };
            inputs[inputsIndex] = input;
            inputsIndex++;
        }

        // Populating KEY UP commands (in reverse order they were pressed)
        for (int i = keys.Length - 1; i >= 0; i--)
        {
            INPUT input = new INPUT();
            input = new INPUT
            {
                type = 1,
                union = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        dwFlags = (uint)(KeyEventF.KeyUp),
                        wVk = (ushort)keys[i]
                    }
                }
            };
            inputs[inputsIndex] = input;
            inputsIndex++;
        }

        return inputs;
    }

   
}