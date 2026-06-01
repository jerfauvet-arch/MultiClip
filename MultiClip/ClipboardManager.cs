using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiClip;

/// <summary>
/// Gère l'historique des éléments copiés dans le presse-papiers.
/// </summary>
public class ClipboardManager
{
    private const int MaxHistory = 20;

    // Liste des entrées (la plus récente en premier)
    private readonly List<ClipEntry> _history = new();

    public IReadOnlyList<ClipEntry> History => _history.AsReadOnly();

    public event EventHandler? HistoryChanged;

    /// <summary>
    /// Ajoute un texte à l'historique (évite les doublons consécutifs).
    /// </summary>
    public void AddEntry(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        // Supprimer si déjà présent (on le remontera en tête)
        _history.RemoveAll(e => e.Content == text);

        _history.Insert(0, new ClipEntry(text));

        // Limiter la taille
        if (_history.Count > MaxHistory)
            _history.RemoveAt(_history.Count - 1);

        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Vide l'historique.
    /// </summary>
    public void ClearHistory()
    {
        _history.Clear();
        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Retourne true si l'historique contient au moins une entrée.
    /// </summary>
    public bool HasEntries => _history.Count > 0;
}

/// <summary>
/// Représente un élément copié.
/// </summary>
public record ClipEntry(string Content)
{
    public DateTime CopiedAt { get; } = DateTime.Now;

    /// <summary>
    /// Aperçu court pour l'affichage dans la liste (max 80 caractères).
    /// </summary>
    public string Preview
    {
        get
        {
            var trimmed = Content.Replace('\n', ' ').Replace('\r', ' ').Trim();
            return trimmed.Length > 80 ? trimmed[..77] + "..." : trimmed;
        }
    }

    /// <summary>
    /// Affiche le temps relatif (il y a X min/sec).
    /// </summary>
    public string TimeAgo
    {
        get
        {
            var elapsed = DateTime.Now - CopiedAt;
            if (elapsed.TotalSeconds < 60) return "À l'instant";
            if (elapsed.TotalMinutes < 60) return $"Il y a {(int)elapsed.TotalMinutes} min";
            return $"Il y a {(int)elapsed.TotalHours}h";
        }
    }
}