using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace MultiClip;

public partial class App : Application
{
    private NotifyIcon? _trayIcon;
    private ClipboardManager? _clipboardManager;
    private KeyboardHook? _keyboardHook;
    private static Mutex? _mutex;

    protected override void OnStartup(StartupEventArgs e)
    {
        // Afficher les erreurs silencieuses
        DispatcherUnhandledException += (s, e) =>
        {
            MessageBox.Show($"Erreur : {e.Exception.Message}\n\n{e.Exception.StackTrace}",
                "MultiClip - Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        };

        // Une seule instance à la fois
        _mutex = new Mutex(true, "MultiClip_SingleInstance", out bool isNew);
        if (!isNew)
        {
            MessageBox.Show("MultiClip est déjà en cours d'exécution.", "MultiClip",
                MessageBoxButton.OK, MessageBoxImage.Information);
            Shutdown();
            return;
        }

        base.OnStartup(e);

        _clipboardManager = new ClipboardManager();
        _keyboardHook = new KeyboardHook(_clipboardManager);

        SetupTrayIcon();
        _keyboardHook.Start();

        var listener = new ClipboardListenerWindow(_clipboardManager);
        listener.Show();
        listener.Hide();
    }

    private void SetupTrayIcon()
    {
        var iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "icon.ico");
        var icon = System.IO.File.Exists(iconPath)
            ? new System.Drawing.Icon(iconPath)
            : SystemIcons.Application;

        _trayIcon = new NotifyIcon
        {
            Text = "MultiClip — Gestionnaire de presse-papiers",
            Visible = true,
            Icon = icon
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("📋 Voir l'historique", null, (s, e) => ShowHistory());
        menu.Items.Add("🗑️ Vider l'historique", null, (s, e) => ClearHistory());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("❌ Quitter", null, (s, e) => QuitApp());

        _trayIcon.ContextMenuStrip = menu;
        _trayIcon.DoubleClick += (s, e) => ShowHistory();
    }

    private void ShowHistory()
    {
        var popup = new PopupWindow(_clipboardManager!);
        popup.ShowAtCursor();
    }

    private void ClearHistory()
    {
        _clipboardManager?.ClearHistory();
        _trayIcon!.ShowBalloonTip(2000, "MultiClip", "Historique vidé ✓", ToolTipIcon.Info);
    }

    private void QuitApp()
    {
        _keyboardHook?.Stop();
        _trayIcon?.Dispose();
        Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _keyboardHook?.Stop();
        _trayIcon?.Dispose();
        base.OnExit(e);
    }
}