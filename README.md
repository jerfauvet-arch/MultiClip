# 📋 MultiClip — Gestionnaire de presse-papiers Windows

Un outil **léger** ⚡ et **gratuit** 🎉 qui enrichit votre presse-papiers Windows : copiez autant d'éléments que vous voulez, puis choisissez lequel coller.

![Version](https://img.shields.io/badge/version-1.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![Platform](https://img.shields.io/badge/platform-Windows%2010%2B-blue)

---

## ⚡ Démarrage rapide

### Prérequis
- **Windows 10** ou **11**
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (pour compiler)

### Utilisation rapide
1. **Télécharge** `MultiClip.exe` depuis [Releases](https://github.com/jerfauvet-arch/MultiClip/releases)
2. **Lance** l'application
3. **Copie** autant de texte que tu veux
4. **Appuie** sur `Ctrl + Shift + V` pour ouvrir la liste
5. **Sélectionne** ce que tu veux coller

### Compilation (optionnel)
```bash
cd MultiClip
dotnet build
dotnet run
```

### Publier un .exe standalone
```bash
dotnet publish -c Release -r win-x64 --self-contained false -o ./publish
```
→ Le fichier `publish/MultiClip.exe` est autonome ✨

---

## 🎮 Raccourcis clavier

| Action | Raccourci |
|---|---|
| **Copier** (normal) | `Ctrl + C` |
| **Ouvrir MultiClip** | `Ctrl + Shift + V` |
| **Naviguer** dans la liste | `↑ ↓` |
| **Coller** l'élément sélectionné | `Entrée` ou double-clic |
| **Fermer** sans coller | `Échap` |
| **Rechercher** dans l'historique | Taper dans la barre |

---

## 📁 Structure du projet

```
MultiClip/
├── MultiClip.sln                    # Solution Visual Studio
└── MultiClip/
    ├── MultiClip.csproj             # Config projet WPF .NET 8
    ├── App.xaml / .cs               # Application + icône tray
    ├── ClipboardManager.cs          # Logique : historique des copies
    ├── ClipboardListenerWindow.cs   # Fenêtre invisible (WM_CLIPBOARDUPDATE)
    ├── KeyboardHook.cs              # Hook global clavier (Ctrl+Shift+V)
    ├── PopupWindow.xaml / .cs       # Interface de sélection
    ├── InputSimulator.cs            # Simulation de Ctrl+V
    └── Assets/
        └── icon.ico                 # Icône
```

---

## 🗺️ Roadmap

### ✅ Phase 1 — MVP (actuel)
- [x] Capture automatique de tout ce qui est copié
- [x] Popup de sélection au raccourci `Ctrl+Shift+V`
- [x] Recherche dans l'historique
- [x] Navigation clavier complète
- [x] Icône dans la barre système

### 🔜 Phase 2 — Confort
- [ ] Support images et fichiers
- [ ] Raccourci personnalisable
- [ ] Démarrage automatique avec Windows
- [ ] Thème clair/sombre
- [ ] Épingler des éléments favoris
- [ ] Historique persistant entre redémarrages

### 🔮 Phase 3 — Avancé
- [ ] Synchronisation entre PC (cloud)
- [ ] Plugins (nettoyage de texte, traduction...)
- [ ] Version Pro avec features exclusives

---

## 🛠️ Technologie

- **Framework** : [WPF](https://github.com/dotnet/wpf) (.NET 8)
- **Langage** : C# 12
- **Interop** : Win32 API (hooks clavier, presse-papiers)
- **License** : MIT

---

## 🤝 Contribution

Les contributions sont **bienvenues** ! 🎉

1. **Fork** le projet
2. Crée une **branche** (`git checkout -b feature/amazing-feature`)
3. **Commit** tes changements (`git commit -m 'Add amazing feature'`)
4. **Push** la branche (`git push origin feature/amazing-feature`)
5. Ouvre une **Pull Request**

---

## 📧 Support

Des questions ou des bugs ? 
- Ouvre une [Issue](https://github.com/jerfauvet-arch/MultiClip/issues)
- Ou contacte-moi directement

---

## 📄 License

Ce projet est sous [MIT License](LICENSE) — utilise-le librement ! 🚀

---

**Créé avec ❤️ pour les devs qui copient beaucoup de code** 😄
