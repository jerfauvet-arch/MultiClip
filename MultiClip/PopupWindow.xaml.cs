using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace MultiClip;

public partial class PopupWindow : Window
{
    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X; public int Y; }

    private readonly ClipboardManager _manager;
    private List<ClipEntry> _filteredEntries = new();

    private bool _isClosing = false;

    public PopupWindow(ClipboardManager manager)
    {
        InitializeComponent();
        _manager = manager;

        Deactivated += (s, e) =>
        {
            if (!_isClosing)
            {
                _isClosing = true;
                Close();
            }
        };
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RefreshList();
        SearchBox.Focus();

        // Animation d'apparition
        var fadeIn = (System.Windows.Media.Animation.Storyboard)Resources["FadeIn"];
        fadeIn?.Begin(this);
    }

    /// <summary>
    /// Affiche la popup à la position du curseur (décalée pour rester à l'écran).
    /// </summary>
    public void ShowAtCursor()
    {
        GetCursorPos(out var point);
        var screen = SystemParameters.WorkArea;

        // Calcul position pour rester dans l'écran
        double left = point.X;
        double top = point.Y - 20;

        if (left + Width > screen.Right) left = screen.Right - Width - 10;
        if (top + 500 > screen.Bottom) top = screen.Bottom - 510;

        Left = Math.Max(screen.Left, left);
        Top = Math.Max(screen.Top, top);

        Show();
        Activate();
    }

    private void RefreshList(string? filter = null)
    {
        _filteredEntries = string.IsNullOrWhiteSpace(filter)
            ? _manager.History.ToList()
            : _manager.History
                .Where(e => e.Content.Contains(filter, StringComparison.OrdinalIgnoreCase))
                .ToList();

        ClipList.ItemsSource = _filteredEntries;
        CountLabel.Text = $"({_filteredEntries.Count} élément{(_filteredEntries.Count > 1 ? "s" : "")})";

        if (_filteredEntries.Count > 0)
            ClipList.SelectedIndex = 0;
    }

    private void PasteSelected()
    {
        if (ClipList.SelectedItem is not ClipEntry entry) return;
        if (_isClosing) return;

        _isClosing = true;

        // Mettre le texte dans le presse-papiers
        System.Windows.Clipboard.SetText(entry.Content);

        // Fermer la fenêtre et attendre que le focus revienne à l'app précédente
        Close();

        // Délai court puis simuler Ctrl+V
        var timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(150)
        };
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            new InputSimulator().PasteText();
        };
        timer.Start();
    }

    // ── Événements UI ────────────────────────────────────────────

    private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        => RefreshList(SearchBox.Text);

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Escape:
                Close();
                break;
            case Key.Enter:
                PasteSelected();
                break;
            case Key.Down:
                ClipList.Focus();
                if (ClipList.SelectedIndex < ClipList.Items.Count - 1)
                    ClipList.SelectedIndex++;
                e.Handled = true;
                break;
            case Key.Up:
                ClipList.Focus();
                if (ClipList.SelectedIndex > 0)
                    ClipList.SelectedIndex--;
                e.Handled = true;
                break;
        }
    }

    private void ClipList_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) PasteSelected();
        if (e.Key == Key.Escape) Close();
    }

    private void ClipList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => PasteSelected();

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        _isClosing = true;
        Close();
    }
}