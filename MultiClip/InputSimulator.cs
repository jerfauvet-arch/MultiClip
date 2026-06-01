using System.Runtime.InteropServices;

namespace MultiClip;

/// <summary>
/// Simule des frappes clavier via l'API Windows SendInput.
/// Utilisé pour coller le texte dans la fenêtre active après sélection.
/// </summary>
public class InputSimulator
{
    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    private const int INPUT_KEYBOARD = 1;
    private const ushort VK_CONTROL = 0x11;
    private const ushort VK_V = 0x56;
    private const uint KEYEVENTF_KEYUP = 0x0002;

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public int type;
        public KEYBDINPUT ki;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] padding;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    /// <summary>
    /// Colle le contenu du presse-papiers dans la fenêtre active (Ctrl+V).
    /// </summary>
    public void PasteText()
    {
        var inputs = new[]
        {
            // Ctrl DOWN
            new INPUT { type = INPUT_KEYBOARD, ki = new KEYBDINPUT { wVk = VK_CONTROL } },
            // V DOWN
            new INPUT { type = INPUT_KEYBOARD, ki = new KEYBDINPUT { wVk = VK_V } },
            // V UP
            new INPUT { type = INPUT_KEYBOARD, ki = new KEYBDINPUT { wVk = VK_V, dwFlags = KEYEVENTF_KEYUP } },
            // Ctrl UP
            new INPUT { type = INPUT_KEYBOARD, ki = new KEYBDINPUT { wVk = VK_CONTROL, dwFlags = KEYEVENTF_KEYUP } },
        };

        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
    }
}