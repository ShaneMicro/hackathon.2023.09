using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

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
                inputs = KeyboardHelper.getTeamsMuteUnmuteMic();
                break;
            case "keyboardonoff":
                inputs = KeyboardHelper.getTeamsOpenCloseCamera();
                break;
            default:
                // code block
                break;
        }

        // Send the keyboard events
        uint result = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }

    [Flags]
    public enum KeyboardDwflags
    {
        KeyDown = 0x0000,
        ExtendedKey = 0x0001,
        KeyUp = 0x0002,
        Unicode = 0x0004,
        Scancode = 0x0008
    }

    public static INPUT[] getTeamsOpenCloseCamera()
    {
        INPUT[] inputs = new INPUT[6];

        INPUT inputCTRL = new INPUT();
        inputCTRL = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    //dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                    wVk = 0x11, //CTRL
                }
            }
        };

        INPUT inputSHIFT = new INPUT();
        inputSHIFT = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    //dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                    wVk = 0x10, //SHIFT
                }
            }
        };


        INPUT inputM = new INPUT();
        inputM = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    //dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                    dwFlags = (uint)(KeyEventF.KeyDown),
                    wVk = (ushort)'O',
                    //wVk = 0x68,
                }
            }
        };

        INPUT inputMKeyUp = new INPUT();
        inputMKeyUp = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    //wVk = (ushort)Char.ConvertToUtf32("D", 0),
                    wVk = (ushort)'O',
                    //dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                    dwFlags = (uint)KeyEventF.KeyUp,
                }
            }
        };

        INPUT inputSHIFTKeyUp = new INPUT();
        inputSHIFTKeyUp = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0x10,
                    //dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                    dwFlags = (uint)KeyEventF.KeyUp,
                }
            }
        };

        INPUT inputCTRLKeyUp = new INPUT();
        inputCTRLKeyUp = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0x11,
                    //dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                    dwFlags = (uint)KeyEventF.KeyUp,
                }
            }
        };

        inputs[0] = inputCTRL;
        inputs[1] = inputSHIFT;
        inputs[2] = inputM;
        inputs[3] = inputMKeyUp;
        inputs[4] = inputSHIFTKeyUp;
        inputs[5] = inputCTRLKeyUp;

        return inputs;
    }



    public static INPUT[] getTeamsMuteUnmuteMic()
    {
        INPUT[] inputs = new INPUT[6];

        INPUT inputCTRL = new INPUT();
        inputCTRL = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    //dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                    wVk = 0x11,
                }
            }
        };

        INPUT inputSHIFT = new INPUT();
        inputSHIFT = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    //dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                    wVk = 0x10,
                }
            }
        };


        INPUT inputM = new INPUT();
        inputM = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    //dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                    dwFlags = (uint)(KeyEventF.KeyDown),
                    wVk = (ushort)'M',
                    //wVk = 0x68,
                }
            }
        };

        INPUT inputMKeyUp = new INPUT();
        inputMKeyUp = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    //wVk = (ushort)Char.ConvertToUtf32("D", 0),
                    wVk = (ushort)'M',
                    //dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                    dwFlags = (uint)KeyEventF.KeyUp,
                }
            }
        };

        INPUT inputSHIFTKeyUp = new INPUT();
        inputSHIFTKeyUp = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0x10,
                    //dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                    dwFlags = (uint)KeyEventF.KeyUp,
                }
            }
        };

        INPUT inputCTRLKeyUp = new INPUT();
        inputCTRLKeyUp = new INPUT
        {
            type = 1,
            union = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0x11,
                    //dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                    dwFlags = (uint)KeyEventF.KeyUp,
                }
            }
        };

        inputs[0] = inputCTRL;
        inputs[1] = inputSHIFT;
        inputs[2] = inputM;
        inputs[3] = inputMKeyUp;
        inputs[4] = inputSHIFTKeyUp;
        inputs[5] = inputCTRLKeyUp;

        return inputs;
    }

    public struct INPUT
    {
        public uint type;
        public InputUnion union;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)]
        public MOUSEINPUT mi;
        [FieldOffset(0)]
        public KEYBDINPUT ki;
        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }

    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    public struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    public struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }

    [Flags]
    public enum KeyEventF
    {
        KeyDown = 0x0000,
        ExtendedKey = 0x0001,
        KeyUp = 0x0002,
        Unicode = 0x0004,
        Scancode = 0x0008
    }
}