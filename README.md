# NodeConfigurator - OPC-UA Variable Manager

Eine moderne WPF-Anwendung für die Verwaltung und Konfiguration von OPC-UA Server-Variablen.

## 🎯 Features

### Kernfunktionen
- **OPC-UA Server-Verbindung** mit flexibler Authentifizierung und Sicherheitseinstellungen
- **Hierarchische Baum-Ansicht** aller Server-Knoten mit Lazy Loading für optimale Performance
- **Variablen-Auswahl** mit Checkbox-Unterstützung für einfache Mehrfachauswahl
- **Details-Panel** mit allen Eigenschaften des ausgewählten Knotens
- **Werte lesen und schreiben** für OPC-UA Variablen
- **Konfigurationsverwaltung** - Speichern und Laden von Variablensets als JSON

### Benutzeroberfläche
- **Moderne Material Design UI** mit ansprechenden Farben und Animationen
- **Responsive Layout** mit verstellbaren Panels (GridSplitter)
- **Such- und Filterfunktionen** für schnelles Finden von Knoten
- **Status-Indikator** mit farblicher Kennzeichnung des Verbindungsstatus
- **Fortschrittsanzeige** während längerer Operationen
- **Icons für Node-Typen** (Objekte, Variablen, Methoden)
- **DataType-Badges** für Variablen

### Export-Funktionen
- Export als JSON
- Export als XML
- Export als CSV

## 🚀 Installation

### Voraussetzungen
- .NET 8 SDK oder höher
- Windows 10/11 (für WPF)
- Visual Studio 2022 oder höher (empfohlen) oder Rider

### Installation
```bash
# Repository klonen
git clone https://github.com/random744/NodeConfigurator.git
cd NodeConfigurator

# NuGet-Pakete wiederherstellen
dotnet restore

# Anwendung bauen
dotnet build

# Anwendung starten
dotnet run --project NodeConfigurator/NodeConfigurator.csproj
```

## 📖 Verwendung

### 1. Mit OPC-UA Server verbinden
1. Geben Sie die Server-URL in die Toolbar ein (z.B. `opc.tcp://localhost:4840`)
2. Klicken Sie auf **⚙️ Einstellungen** für erweiterte Verbindungsoptionen:
   - Authentifizierung (Username/Passwort)
   - Sicherheitseinstellungen (SecurityMode, SecurityPolicy)
   - Zertifikatsverwaltung
   - Timeout-Einstellungen
3. Klicken Sie auf **🔌 Verbinden**
4. Bei erfolgreicher Verbindung wird der Status-Indikator grün

### 2. Knoten durchsuchen
- Der Baum zeigt alle verfügbaren Knoten hierarchisch an
- Klicken Sie auf den Pfeil zum Erweitern von Knoten
- Nutzen Sie die Filteroptionen (Variablen, Objekte, Methoden)
- Verwenden Sie die Suchfunktion für schnelles Finden

### 3. Variablen auswählen
- Aktivieren Sie die Checkbox bei Variablen zum Auswählen
- Ausgewählte Variablen erscheinen im rechten Panel
- Entfernen Sie Variablen mit dem ❌-Button

### 4. Konfiguration speichern
1. Menü: **Datei** → **Konfiguration speichern...**
2. Wählen Sie einen Speicherort
3. Die Konfiguration wird als JSON-Datei gespeichert

### 5. Konfiguration laden
1. Menü: **Datei** → **Konfiguration laden...**
2. Wählen Sie eine JSON-Konfigurationsdatei
3. Alle gespeicherten Variablen werden wiederhergestellt

## 🛠️ Technologie-Stack

- **.NET 8** - Moderne .NET-Plattform
- **WPF (Windows Presentation Foundation)** - Rich Desktop UI Framework
- **MVVM Pattern** - Saubere Architektur mit Model-View-ViewModel
- **OPC Foundation .NET Standard** - Offizielle OPC-UA Implementierung
  - OPCFoundation.NetStandard.Opc.Ua (1.5.374.54)
  - OPCFoundation.NetStandard.Opc.Ua.Client (1.5.374.54)

## 📁 Projektstruktur

```
NodeConfigurator/
├── NodeConfigurator.sln              # Visual Studio Solution
├── NodeConfigurator/
│   ├── NodeConfigurator.csproj       # Projektdatei
│   ├── App.xaml                       # Application mit Styles
│   ├── App.xaml.cs
│   ├── Views/                         # UI Views
│   │   ├── MainWindow.xaml
│   │   ├── MainWindow.xaml.cs
│   │   ├── ConnectionDialog.xaml
│   │   └── ConnectionDialog.xaml.cs
│   ├── ViewModels/                    # MVVM ViewModels
│   │   ├── ViewModelBase.cs
│   │   ├── MainViewModel.cs
│   │   ├── TreeNodeViewModel.cs
│   │   └── ConnectionViewModel.cs
│   ├── Models/                        # Datenmodelle
│   │   ├── NodeConfiguration.cs
│   │   ├── ServerConnectionConfig.cs
│   │   └── SelectedNode.cs
│   ├── Services/                      # Business Logic
│   │   ├── IOpcUaClientService.cs
│   │   └── OpcUaClientService.cs
│   ├── Converters/                    # XAML Value Converters
│   │   ├── BoolToVisibilityConverter.cs
│   │   └── NodeClassToIconConverter.cs
│   └── Commands/                      # Command Pattern
│       └── RelayCommand.cs
├── README.md
└── .gitignore
```

## 🎨 UI-Komponenten

### Farbschema
- **Primary**: #0078D4 (Microsoft Blue)
- **Accent**: #106EBE (Dunkleres Blue)
- **Success**: #107C10 (Grün)
- **Error**: #E81123 (Rot)
- **Background**: #F3F3F3 (Hellgrau)
- **Border**: #D0D0D0 (Grau)

### Styles
- ModernButton - Primärer Button mit Hover-Effekten
- SecondaryButton - Sekundärer Button mit Outline
- DangerButton - Roter Button für Lösch-Aktionen
- ModernTextBox - Eingabefelder mit Border-Radius
- ModernComboBox - Dropdown mit angepasstem Design
- GroupBox - Gruppierte Inhalte mit Header

## 🔧 Entwicklung

### Build-Befehle
```bash
# Debug Build
dotnet build

# Release Build
dotnet build -c Release

# Tests ausführen (wenn vorhanden)
dotnet test

# Publish für Deployment
dotnet publish -c Release -r win-x64 --self-contained
```

### Erweiterungen
Das Projekt ist erweiterbar für:
- Weitere Export-Formate
- Historische Daten-Abfrage
- Alarm & Event Management
- Daten-Visualisierung (Charts)
- Batch-Operationen
- Scripting-Unterstützung

## 📋 Roadmap

- [ ] Implementierung aller Export-Funktionen (XML, CSV)
- [ ] Erweiterte Such- und Filterfunktionen
- [ ] Historische Daten-Abfrage
- [ ] Werte-Schreibfunktion mit Validierung
- [ ] Alarm & Event Subscription
- [ ] Mehrsprachige UI (Englisch, Deutsch)
- [ ] Dark Mode
- [ ] Datenvisualisierung mit Live-Charts
- [ ] Scripting mit C# oder Python
- [ ] Plugin-System

## 📄 Lizenz

Dieses Projekt steht unter der MIT-Lizenz.

## 🤝 Mitwirken

Beiträge sind willkommen! Bitte erstellen Sie einen Pull Request oder öffnen Sie ein Issue.

## 📞 Support

Bei Fragen oder Problemen öffnen Sie bitte ein Issue auf GitHub.

---

**NodeConfigurator** - Ihre Lösung für professionelles OPC-UA Node Management
