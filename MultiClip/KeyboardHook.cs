using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace MultiClip;

/// <summary>
/// Accroche globale clavier pour intercepter le raccourci MultiClip.
/// Raccourci par défaut : Ctrl + Shift + V (pour ne pas bloquer le Ctrl+V normal).
/// </summary>
public class KeyboardHook
{
    // Win32 API
    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private const int VK_V = 0x56;

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    private readonly ClipboardManager _manager;
    private LowLevelKeyboardProc? _proc;
    private IntPtr _hookId = IntPtr.Zero;

    public KeyboardHook(ClipboardManager manager)
    {
        _manager = manager;
    }

    public void Start()
    {
        _proc = HookCallback;
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule!;
        _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc,
            GetModuleHandle(curModule.ModuleName), 0);
    }

    public void Stop()
    {
        if (_hookId != IntPtr.Zero)
            UnhookWindowsHookEx(_hookId);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);

            // Ctrl + Shift + V → ouvrir MultiClip
            bool isCtrlDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            bool isShiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            
            if (vkCode == VK_V && isCtrlDown && isShiftDown)
            {
                if (_manager.HasEntries)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        var popup = new PopupWindow(_manager);
                        popup.ShowAtCursor();
                    });
                    return (IntPtr)1; // Bloquer la touche
                }
            }
        }
        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }
}