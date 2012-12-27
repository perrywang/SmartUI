using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Collections;

namespace SUI.Base.Win
{
    public class SUIKeyboard
    {
        public enum VK : ushort
        {
            SHIFT = 0x10,
            CONTROL = 0x11,
            MENU = 0x12,
            ESCAPE = 0x1B,
            BACK = 0x08,
            TAB = 0x09,
            RETURN = 0x0D,
            SPACE = 0x20,
            PRIOR = 0x21,
            NEXT = 0x22,
            END = 0x23,
            HOME = 0x24,
            LEFT = 0x25,
            UP = 0x26,
            RIGHT = 0x27,
            DOWN = 0x28,
            SELECT = 0x29,
            PRINT = 0x2A,
            EXECUTE = 0x2B,
            SNAPSHOT = 0x2C,
            INSERT = 0x2D,
            DELETE = 0x2E,
            HELP = 0x2F,
            NUMPAD0 = 0x60,
            NUMPAD1 = 0x61,
            NUMPAD2 = 0x62,
            NUMPAD3 = 0x63,
            NUMPAD4 = 0x64,
            NUMPAD5 = 0x65,
            NUMPAD6 = 0x66,
            NUMPAD7 = 0x67,
            NUMPAD8 = 0x68,
            NUMPAD9 = 0x69,
            MULTIPLY = 0x6A,
            ADD = 0x6B,
            SEPARATOR = 0x6C,
            SUBTRACT = 0x6D,
            DECIMAL = 0x6E,
            DIVIDE = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            OEM_1 = 0xBA,   // ',:' for US
            OEM_PLUS = 0xBB,   // '+' any country
            OEM_COMMA = 0xBC,   // ',' any country
            OEM_MINUS = 0xBD,   // '-' any country
            OEM_PERIOD = 0xBE,   // '.' any country
            OEM_2 = 0xBF,   // '/?' for US
            OEM_3 = 0xC0,   // '`~' for US
            MEDIA_NEXT_TRACK = 0xB0,
            MEDIA_PREV_TRACK = 0xB1,
            MEDIA_STOP = 0xB2,
            MEDIA_PLAY_PAUSE = 0xB3,
            LWIN = 0x5B,
            RWIN = 0x5C
        }

        private static int pause = 150;

        public static int KeystrokeRate
        {
            set
            {
                pause = value;
            }
        }
        public static void Type(string str)
        {
            Type(str, pause);
        }

        private static bool isUpperCase(char ch)
        {
            return (ch >= 'A' && ch <= 'Z');
        }

        public static void Type(string str, int pauserate)
        {
            foreach (char ch in str)
            {
                Thread.Sleep(pauserate);
                char typeCh = ch;
                IList<INPUT> inputList = new List<INPUT>();
                switch (ch)
                {
                    case ':':
                        {
                            typeCh = ';';
                            break;
                        }
                    case '<':
                        {
                            typeCh = ',';
                            break;
                        }
                    case '?':
                        {
                            typeCh = '/';
                            break;
                        }
                    case '~':
                        {
                            typeCh = '`';
                            break;
                        }
                    case '!':
                        {
                            typeCh = '1';
                            break;
                        }
                    case '@':
                        {
                            typeCh = '2';
                            break;
                        }
                    case '#':
                        {
                            typeCh = '3';
                            break;
                        }
                    case '$':
                        {
                            typeCh = '4';
                            break;
                        }
                    case '%':
                        {
                            typeCh = '5';
                            break;
                        }
                    case '^':
                        {
                            typeCh = '6';
                            break;
                        }
                    case '&':
                        {
                            typeCh = '7';
                            break;
                        }
                    case '*':
                        {
                            typeCh = '8';
                            break;
                        }
                    case '(':
                        {
                            typeCh = '9';
                            break;
                        }
                    case ')':
                        {
                            typeCh = '0';
                            break;
                        }
                    case '_':
                        {
                            typeCh = '-';
                            break;
                        }
                    case '+':
                        {
                            typeCh = '=';
                            break;
                        }
                    case '{':
                        {
                            typeCh = '[';
                            break;
                        }
                    case '}':
                        {
                            typeCh = ']';
                            break;
                        }
                    case '|':
                        {
                            typeCh = '\\';
                            break;
                        }
                    case '\"':
                        {
                            typeCh = '\'';
                            break;
                        }
                    default:
                        break;
                }
                byte vkCode = GetVirtualKeyCode(typeCh);
                ushort scanCode = 0;
                IntPtr extroInfo = IntPtr.Zero;
                if (vkCode == 0xff) //Input unicode
                {
                    vkCode = 0;
                    int character = typeCh;
                    scanCode = (ushort)character;

                    inputList.Add(ConstructINPUT(vkCode, scanCode, 4 | 0, extroInfo));
                    inputList.Add(ConstructINPUT(vkCode, scanCode, 4 | 2, extroInfo));
                }
                else
                {
                    if (ch == ':' || ch == '<' || ch == '>' || ch == '?' ||
                        ch == '~' || ch == '!' || ch == '@' || ch == '$' ||
                        ch == '%' || ch == '^' || ch == '&' || ch == '(' ||
                        ch == ')' || ch == '_' || ch == '+' || ch == '{' ||
                        ch == '}' || ch == '|' || ch == '\"' || ch == '#' || ch == '*' || isUpperCase(ch))
                        inputList.Add(ConstructINPUT((byte)VK.SHIFT, 0, 0, extroInfo));
                    inputList.Add(ConstructINPUT(vkCode, scanCode, 0, extroInfo));
                    inputList.Add(ConstructINPUT(vkCode, scanCode, 2, extroInfo));
                    if (ch == ':' || ch == '<' || ch == '>' || ch == '?' ||
                        ch == '~' || ch == '!' || ch == '@' || ch == '$' ||
                        ch == '%' || ch == '^' || ch == '&' || ch == '(' ||
                        ch == ')' || ch == '_' || ch == '+' || ch == '{' ||
                        ch == '}' || ch == '|' || ch == '\"' || ch == '#' || ch == '*' || isUpperCase(ch))
                        inputList.Add(ConstructINPUT((byte)VK.SHIFT, 0, 2, extroInfo));
                }
                INPUT[] inputs = new INPUT[inputList.Count];
                inputList.CopyTo(inputs, 0);

               SendInputs(inputs);
            }
            
        }

        public static void Type(VK key)
        {
            Type(key, pause);
        }

        public static void Type(VK key, int pauserate)
        {
            Thread.Sleep(pauserate);
            SUIWinAPIs.keybd_event((byte)key, 0, 0, 0);
            SUIWinAPIs.keybd_event((byte)key, 0, 2, 0);
        }


        public static void Press(VK key)
        {
            Thread.Sleep(pause);
            SUIWinAPIs.keybd_event((byte)key, 0, 0, 0);
        }

        public static void Press(char key)
        {
            Thread.Sleep(pause);
            SUIWinAPIs.keybd_event((byte)key, 0, 0, 0);
        }

        public static void Release(char key)
        {
            Thread.Sleep(pause);
            SUIWinAPIs.keybd_event((byte)key, 0, 2, 0);
        }

        public static void Release(VK key)
        {
            Thread.Sleep(pause);
            SUIWinAPIs.keybd_event((byte)key, 0, 2, 0);
        }

        private static byte GetVirtualKeyCode(char character)
        {
            IntPtr KeyboardLayout = SUIWinAPIs.GetKeyboardLayout(0);
            HandleRef keyboardLayout = new HandleRef(null, KeyboardLayout);
            short num = SUIWinAPIs.VkKeyScanEx(character, keyboardLayout);
            return Convert.ToByte(num & 0xff, CultureInfo.InvariantCulture);
        }

        private static INPUT ConstructINPUT(byte vk, ushort scan, int flags, IntPtr dwExtraInfo)
        {
            INPUT input = new INPUT();
            input.type = InputType.KEYBOARD;
            KEYBDINPUT kbInput = new KEYBDINPUT();
            kbInput.wVk = vk;
            kbInput.wScan = scan;
            kbInput.dwFlags = flags;
            kbInput.time = 0;
            kbInput.dwExtraInfo = dwExtraInfo;
            input.ki = kbInput;
            return input;
        }

        private static void SendInput(INPUT input)
        {
            SUIWinAPIs.SendInput(1, new INPUT[] { input }, Marshal.SizeOf(input));
        }

        private static void SendInputs(INPUT[] inputs)
        {
            if(inputs != null && inputs.Length > 0)
            {
                SUIWinAPIs.SendInput(inputs.Length, inputs, Marshal.SizeOf(inputs[0]));
            }
        }
    }
}
