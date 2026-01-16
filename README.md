# NodeConfigurator - OPC-UA Node Management Web Application

Eine moderne ASP.NET Core 8 MVC Web-Anwendung für die Verwaltung und Konfiguration von OPC-UA Server-Variablen.

## 🎯 Features

### Kernfunktionen
- **OPC-UA Server-Verbindung** mit flexibler Authentifizierung und Sicherheitseinstellungen
- **Hierarchische Baum-Ansicht** aller Server-Knoten mit Lazy Loading für optimale Performance
- **Variablen-Auswahl** mit Checkbox-Unterstützung für einfache Mehrfachauswahl
- **Details-Panel** mit allen Eigenschaften des ausgewählten Knotens
- **Werte lesen und schreiben** für OPC-UA Variablen
- **Konfigurationsverwaltung** - Speichern und Laden von Variablensets als JSON

### Web-Interface Features
- **Moderne Browser-basierte UI** mit Bootstrap 5
- **Responsive Design** - funktioniert auf Desktop, Tablet und Mobile
- **Session-Management** für mehrere gleichzeitige Benutzer
- **Real-time Updates** via AJAX
- **jsTree Integration** für hierarchische Navigation
- **Deutsche Lokalisierung** der gesamten Benutzeroberfläche

### Export-Funktionen
- Export als JSON (strukturiert mit Metadaten)
- Export als XML (standardisiertes Format)
- Export als CSV (Excel-kompatibel)

## 🚀 Installation

### Voraussetzungen
- .NET 8 SDK oder höher
- Moderner Webbrowser (Chrome, Firefox, Edge, Safari)
- Optional: OPC-UA Test-Server für Entwicklung

### Installation und Start

```bash
# Repository klonen
git clone https://github.com/random744/NodeConfigurator.git
cd NodeConfigurator

# In das Web-Projektverzeichnis wechseln
cd NodeConfigurator.Web

# NuGet-Pakete wiederherstellen
dotnet restore

# Anwendung bauen
dotnet build

# Anwendung starten
dotnet run
```

Die Anwendung ist dann unter **https://localhost:5001** oder **http://localhost:5000** erreichbar.

## 📖 Verwendung

### 1. Mit OPC-UA Server verbinden
1. Öffnen Sie die Anwendung im Browser: `https://localhost:5001`
2. Navigieren Sie zu **Verbinden** in der Navigationsleiste
3. Geben Sie die Server-URL ein (z.B. `opc.tcp://localhost:4840`)
4. Optional: Konfigurieren Sie erweiterte Einstellungen
   - Authentifizierung (Username/Passwort)
   - Sicherheitseinstellungen (SecurityMode, SecurityPolicy)
   - Zertifikatsverwaltung
   - Timeout-Einstellungen
5. Klicken Sie auf **Verbinden**
6. Bei erfolgreicher Verbindung erscheint ein grüner "Verbunden"-Badge

### 2. Knoten durchsuchen
1. Navigieren Sie zu **Durchsuchen**
2. Der Baum zeigt alle verfügbaren Knoten hierarchisch an
3. Klicken Sie auf Knoten zum Erweitern
4. Nutzen Sie die Suchfunktion für schnelles Finden
5. Wählen Sie Variablen mit Checkboxen aus

### 3. Variablen verwalten
- Ausgewählte Variablen erscheinen im rechten Panel "Ausgewählte Variablen"
- Klicken Sie auf einen Knoten für Details im Details-Panel
- Klicken Sie "Wert lesen" um den aktuellen Wert anzuzeigen
- Entfernen Sie Variablen mit dem X-Button

### 4. Konfiguration exportieren
1. Navigieren Sie zu **Export**
2. Wählen Sie das gewünschte Format (JSON, XML, CSV)
3. Die Datei wird automatisch heruntergeladen

### 5. Konfiguration importieren
1. Navigieren Sie zu **Import**
2. Wählen Sie eine zuvor exportierte JSON-Datei
3. Die Variablen werden zur aktuellen Auswahl hinzugefügt

## 🛠️ Technologie-Stack

- **.NET 8** - Moderne .NET-Plattform
- **ASP.NET Core MVC** - Web-Framework mit Model-View-Controller Pattern
- **Bootstrap 5** - Responsive UI Framework
- **jQuery** - JavaScript Library für DOM-Manipulation
- **jsTree** - Interactive Tree View Component
- **OPC Foundation .NET Standard** - Offizielle OPC-UA Implementierung
  - OPCFoundation.NetStandard.Opc.Ua (1.5.374.54)
  - OPCFoundation.NetStandard.Opc.Ua.Client (1.5.374.54)

## 📁 Projektstruktur

```
NodeConfigurator.Web/
├── NodeConfigurator.Web.csproj    # Projektdatei
├── Program.cs                      # Application Entry Point
├── appsettings.json                # Konfiguration
├── Controllers/                    # MVC Controllers
│   ├── HomeController.cs
│   ├── OpcUaController.cs
│   └── ConfigurationController.cs
├── Views/                          # Razor Views
│   ├── Shared/
│   │   └── _Layout.cshtml
│   ├── Home/
│   │   └── Index.cshtml
│   ├── OpcUa/
│   │   ├── Connect.cshtml
│   │   └── Browse.cshtml
│   └── Configuration/
│       ├── Export.cshtml
│       └── Import.cshtml
├── Models/                         # Datenmodelle
│   ├── NodeConfiguration.cs
│   ├── ServerConnectionConfig.cs
│   ├── SelectedNode.cs
│   └── ViewModels/
│       ├── ConnectionViewModel.cs
│       ├── BrowseViewModel.cs
│       └── NodeViewModel.cs
├── Services/                       # Business Logic
│   ├── IOpcUaClientService.cs
│   ├── OpcUaClientService.cs
│   └── SessionManagerService.cs
└── wwwroot/                        # Static Files
    ├── css/
    │   └── site.css
    └── js/
        ├── site.js
        └── opcua-browser.js
```

## 🎨 UI-Komponenten

### Farbschema
- **Primary**: #0078D4 (Microsoft Blue)
- **Success**: #107C10 (Grün)
- **Danger**: #E81123 (Rot)
- **Warning**: #FFB900 (Gelb)
- **Info**: #00BCF2 (Cyan)
- **Background**: #F3F3F3 (Hellgrau)

### Features der Benutzeroberfläche
- Responsive Navigation mit Bootstrap 5
- Verbindungsstatus-Anzeige in der Navbar
- Toast-Benachrichtigungen für Benutzer-Feedback
- Interaktiver Baum mit jsTree
- Collapsible Panels für erweiterte Einstellungen
- Icon-basierte Navigation
- Moderne Card-Layouts

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
dotnet publish -c Release -o ./publish
```

### Entwicklungsserver starten
```bash
# Mit Hot Reload für Entwicklung
dotnet watch run
```

Der Server startet automatisch neu bei Code-Änderungen.

## 🌐 Browser-Kompatibilität

- Chrome/Edge (empfohlen) - Version 90+
- Firefox - Version 88+
- Safari - Version 14+

## 📋 Architektur

### Session-Management
Die Anwendung verwendet ASP.NET Core Sessions für:
- Verbindungsstatus pro Benutzer
- Ausgewählte Variablen pro Session
- Isolation zwischen verschiedenen Benutzern

### Service-Layer
- **OpcUaClientService**: Singleton für OPC-UA Verbindungen
- **SessionManagerService**: Singleton für Session-Verwaltung
- Dependency Injection für lose Kopplung

### MVC-Pattern
- **Models**: Datenstrukturen und Business-Objekte
- **Views**: Razor-Templates für HTML-Rendering
- **Controllers**: Request-Handling und Response-Generierung

## 🔒 Sicherheit

- HTTPS-Unterstützung
- Session-basierte Authentifizierung
- XSS-Schutz durch Razor-Encoding
- CSRF-Schutz für Form-Posts
- Sichere Cookie-Konfiguration

## 📄 Lizenz

Dieses Projekt steht unter der MIT-Lizenz.

## 🤝 Mitwirken

Beiträge sind willkommen! Bitte erstellen Sie einen Pull Request oder öffnen Sie ein Issue.

## 📞 Support

Bei Fragen oder Problemen öffnen Sie bitte ein Issue auf GitHub.

---

**NodeConfigurator** - Ihre professionelle Web-Lösung für OPC-UA Node Management
