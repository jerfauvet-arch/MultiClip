using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace MultiClip;

/// <summary>
/// Fenêtre invisible qui écoute les changements du presse-papiers Windows.
/// Windows exige une fenêtre pour recevoir les messages WM_CLIPBOARDUPDATE.
/// </summary>
public class ClipboardListenerWindow : Window
{
    [DllImport("user32.dll")]
    private static extern bool AddClipboardFormatListener(IntPtr hwnd);

    [DllImport("user32.dll")]
    private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

    private const int WM_CLIPBOARDUPDATE = 0x031D;

    private readonly ClipboardManager _manager;
    private HwndSource? _source;
    private bool _ignoreNext = false; // Pour éviter la boucle lors du collage

    public ClipboardListenerWindow(ClipboardManager manager)
    {
        _manager = manager;
        Width = 0;
        Height = 0;
        WindowStyle = WindowStyle.None;
        ShowInTaskbar = false;
        Opacity = 0;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        var helper = new WindowInteropHelper(this);
        _source = HwndSource.FromHwnd(helper.Handle);
        _source.AddHook(WndProc);

        AddClipboardFormatListener(helper.Handle);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_CLIPBOARDUPDATE)
        {
            if (_ignoreNext)
            {
                _ignoreNext = false;
            }
            else
            {
                OnClipboardChanged();
            }
            handled = true;
        }
        return IntPtr.Zero;
    }

    private void OnClipboardChanged()
    {
        try
        {
            if (System.Windows.Clipboard.ContainsText())
            {
                var text = System.Windows.Clipboard.GetText();
                _manager.AddEntry(text);
            }
        }
        catch
        {
            // Le presse-papiers peut être temporairement verrouillé par une autre app
        }
    }

    /// <summary>
    /// Appeler avant de coller pour ne pas ré-enregistrer ce qu'on colle.
    /// </summary>
    public void IgnoreNextChange() => _ignoreNext = true;

    protected override void OnClosed(EventArgs e)
    {
        var helper = new WindowInteropHelper(this);
        RemoveClipboardFormatListener(helper.Handle);
        _source?.RemoveHook(WndProc);
        base.OnClosed(e);
    }
}