using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Opc.Ua;
using NodeConfigurator.Commands;
using NodeConfigurator.Models;
using NodeConfigurator.Services;
using NodeConfigurator.Views;
using System.Text.Json;
using System.IO;

namespace NodeConfigurator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IOpcUaClientService _opcService;
        private ServerConnectionConfig? _connectionConfig;

        public ObservableCollection<TreeNodeViewModel> RootNodes { get; } = new();
        public ObservableCollection<SelectedNode> SelectedVariables { get; } = new();
        public ObservableCollection<string> ServerUrls { get; } = new();

        private TreeNodeViewModel? _selectedNode;
        public TreeNodeViewModel? SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (SetProperty(ref _selectedNode, value))
                {
                    OnPropertyChanged(nameof(HasSelectedNode));
                }
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (SetProperty(ref _isConnected, value))
                {
                    OnPropertyChanged(nameof(CanConnect));
                }
            }
        }

        private string _statusText = "Nicht verbunden";
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        private string _statusMessage = "Bereit";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private Brush _statusColor = Brushes.Gray;
        public Brush StatusColor
        {
            get => _statusColor;
            set => SetProperty(ref _statusColor, value);
        }

        private int _selectedNodesCount;
        public int SelectedNodesCount
        {
            get => _selectedNodesCount;
            set => SetProperty(ref _selectedNodesCount, value);
        }

        private int _totalNodesCount;
        public int TotalNodesCount
        {
            get => _totalNodesCount;
            set => SetProperty(ref _totalNodesCount, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private bool _showVariables = true;
        public bool ShowVariables
        {
            get => _showVariables;
            set => SetProperty(ref _showVariables, value);
        }

        private bool _showObjects = true;
        public bool ShowObjects
        {
            get => _showObjects;
            set => SetProperty(ref _showObjects, value);
        }

        private bool _showMethods = true;
        public bool ShowMethods
        {
            get => _showMethods;
            set => SetProperty(ref _showMethods, value);
        }

        private string _selectedServerUrl = string.Empty;
        public string SelectedServerUrl
        {
            get => _selectedServerUrl;
            set
            {
                if (SetProperty(ref _selectedServerUrl, value))
                {
                    OnPropertyChanged(nameof(CanConnect));
                }
            }
        }

        public bool HasSelectedNode => SelectedNode != null;
        public bool CanConnect => !string.IsNullOrWhiteSpace(SelectedServerUrl) && !IsConnected;

        // Commands
        public ICommand ConnectCommand { get; }
        public ICommand DisconnectCommand { get; }
        public ICommand OpenConnectionDialogCommand { get; }
        public ICommand RefreshTreeCommand { get; }
        public ICommand LoadConfigurationCommand { get; }
        public ICommand SaveConfigurationCommand { get; }
        public ICommand ClearSelectionCommand { get; }
        public ICommand RemoveSelectedVariableCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand DeselectAllCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand DocumentationCommand { get; }
        public ICommand ExpandAllCommand { get; }
        public ICommand CollapseAllCommand { get; }
        public ICommand ReadValueCommand { get; }
        public ICommand WriteValueCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ExportJsonCommand { get; }
        public ICommand ExportXmlCommand { get; }
        public ICommand ExportCsvCommand { get; }

        public MainViewModel()
        {
            _opcService = new OpcUaClientService();
            _opcService.ConnectionStatusChanged += OnConnectionStatusChanged;

            // Initialize all commands
            ConnectCommand = new RelayCommand(async _ => await ConnectAsync(), _ => CanConnect);
            DisconnectCommand = new RelayCommand(async _ => await DisconnectAsync(), _ => IsConnected);
            OpenConnectionDialogCommand = new RelayCommand(OpenConnectionDialog);
            RefreshTreeCommand = new RelayCommand(async _ => await RefreshTreeAsync(), _ => IsConnected);
            LoadConfigurationCommand = new RelayCommand(LoadConfiguration);
            SaveConfigurationCommand = new RelayCommand(SaveConfiguration, _ => SelectedVariables.Count > 0);
            ClearSelectionCommand = new RelayCommand(_ => SelectedVariables.Clear());
            RemoveSelectedVariableCommand = new RelayCommand(RemoveSelectedVariable);
            SelectAllCommand = new RelayCommand(SelectAll, _ => IsConnected);
            DeselectAllCommand = new RelayCommand(DeselectAll, _ => IsConnected);
            ExitCommand = new RelayCommand(_ => Application.Current.Shutdown());
            AboutCommand = new RelayCommand(ShowAbout);
            DocumentationCommand = new RelayCommand(ShowDocumentation);
            ExpandAllCommand = new RelayCommand(ExpandAll, _ => IsConnected);
            CollapseAllCommand = new RelayCommand(CollapseAll, _ => IsConnected);
            ReadValueCommand = new RelayCommand(async _ => await ReadSelectedValueAsync(), _ => HasSelectedNode && IsConnected);
            WriteValueCommand = new RelayCommand(WriteValue, _ => HasSelectedNode && SelectedNode?.CanWrite == true);
            SearchCommand = new RelayCommand(Search);
            ExportJsonCommand = new RelayCommand(ExportJson);
            ExportXmlCommand = new RelayCommand(ExportXml);
            ExportCsvCommand = new RelayCommand(ExportCsv);

            // Default server URLs
            ServerUrls.Add("opc.tcp://localhost:4840");
            ServerUrls.Add("opc.tcp://localhost:48010");
            SelectedServerUrl = ServerUrls.First();

            // Initial status
            StatusText = "Nicht verbunden";
            StatusColor = Brushes.Gray;
            StatusMessage = "Bereit";

            // Setup collection changed events
            SelectedVariables.CollectionChanged += (s, e) =>
            {
                SelectedNodesCount = SelectedVariables.Count;
            };
        }

        private async Task ConnectAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Verbinde...";

                // Use connection config if available, otherwise create default
                var config = _connectionConfig ?? new ServerConnectionConfig { Url = SelectedServerUrl };

                bool success = await _opcService.ConnectAsync(config);

                if (success)
                {
                    IsConnected = true;
                    StatusText = "Verbunden";
                    StatusColor = Brushes.Green;
                    StatusMessage = $"Verbunden mit {config.Url}";
                    await LoadRootNodesAsync();
                }
                else
                {
                    StatusText = "Verbindung fehlgeschlagen";
                    StatusColor = Brushes.Red;
                    StatusMessage = "Verbindung konnte nicht hergestellt werden";
                    MessageBox.Show("Verbindung zum Server konnte nicht hergestellt werden.", "Verbindungsfehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                StatusText = "Fehler";
                StatusColor = Brushes.Red;
                StatusMessage = $"Fehler: {ex.Message}";
                MessageBox.Show($"Fehler beim Verbinden: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DisconnectAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Trenne...";
                await _opcService.DisconnectAsync();
                IsConnected = false;
                StatusText = "Getrennt";
                StatusColor = Brushes.Gray;
                StatusMessage = "Getrennt";
                RootNodes.Clear();
                TotalNodesCount = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Trennen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadRootNodesAsync()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Lade Knoten...";
                RootNodes.Clear();

                var references = await _opcService.BrowseAsync(ObjectIds.ObjectsFolder);
                
                foreach (var reference in references)
                {
                    var node = new TreeNodeViewModel(_opcService)
                    {
                        DisplayName = reference.DisplayName.Text ?? reference.BrowseName.Name,
                        NodeId = reference.NodeId.ToString(),
                        BrowseName = reference.BrowseName.Name,
                        NodeClass = reference.NodeClass,
                        HasChildren = reference.NodeClass != NodeClass.Variable
                    };
                    RootNodes.Add(node);
                }

                TotalNodesCount = RootNodes.Count;
                StatusMessage = $"{RootNodes.Count} Knoten geladen";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Fehler beim Laden: {ex.Message}";
                MessageBox.Show($"Fehler beim Laden der Knoten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RefreshTreeAsync()
        {
            await LoadRootNodesAsync();
        }

        private void OnConnectionStatusChanged(object? sender, string status)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                StatusMessage = status;
            });
        }

        private void OpenConnectionDialog(object? parameter)
        {
            var dialog = new ConnectionDialog();
            var viewModel = new ConnectionViewModel();
            
            // Set current values if available
            if (_connectionConfig != null)
            {
                viewModel.ServerUrl = _connectionConfig.Url;
                viewModel.Username = _connectionConfig.Username;
                viewModel.Password = _connectionConfig.Password;
                viewModel.UseAuthentication = !string.IsNullOrEmpty(_connectionConfig.Username);
                viewModel.SecurityMode = _connectionConfig.SecurityMode;
                viewModel.SecurityPolicy = _connectionConfig.SecurityPolicy;
                viewModel.AutoAcceptCertificates = _connectionConfig.AutoAcceptCertificates;
                viewModel.Timeout = _connectionConfig.Timeout;
                viewModel.SessionTimeout = _connectionConfig.SessionTimeout;
            }
            else
            {
                viewModel.ServerUrl = SelectedServerUrl;
            }

            dialog.DataContext = viewModel;

            if (dialog.ShowDialog() == true)
            {
                _connectionConfig = viewModel.GetConfig();
                SelectedServerUrl = _connectionConfig.Url;
                
                // Add to recent URLs if not already present
                if (!ServerUrls.Contains(_connectionConfig.Url))
                {
                    ServerUrls.Insert(0, _connectionConfig.Url);
                }
            }
        }

        private void SaveConfiguration(object? parameter)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "JSON Dateien (*.json)|*.json",
                    DefaultExt = "json",
                    FileName = "NodeConfiguration.json"
                };

                if (dialog.ShowDialog() == true)
                {
                    var config = new NodeConfiguration
                    {
                        Name = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName),
                        CreatedDate = DateTime.Now,
                        ServerConfig = _connectionConfig,
                        SelectedNodes = SelectedVariables.ToList()
                    };

                    var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(dialog.FileName, json);

                    StatusMessage = $"Konfiguration gespeichert: {dialog.FileName}";
                    MessageBox.Show("Konfiguration erfolgreich gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadConfiguration(object? parameter)
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "JSON Dateien (*.json)|*.json",
                    DefaultExt = "json"
                };

                if (dialog.ShowDialog() == true)
                {
                    var json = File.ReadAllText(dialog.FileName);
                    var config = JsonSerializer.Deserialize<NodeConfiguration>(json);

                    if (config != null)
                    {
                        SelectedVariables.Clear();
                        foreach (var node in config.SelectedNodes)
                        {
                            SelectedVariables.Add(node);
                        }

                        if (config.ServerConfig != null)
                        {
                            _connectionConfig = config.ServerConfig;
                            SelectedServerUrl = _connectionConfig.Url;
                        }

                        StatusMessage = $"Konfiguration geladen: {dialog.FileName}";
                        MessageBox.Show($"Konfiguration mit {config.SelectedNodes.Count} Knoten geladen.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveSelectedVariable(object? parameter)
        {
            if (parameter is SelectedNode node)
            {
                SelectedVariables.Remove(node);
            }
        }

        private void SelectAll(object? parameter)
        {
            MessageBox.Show("Alle auswählen wird implementiert...", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeselectAll(object? parameter)
        {
            SelectedVariables.Clear();
            MessageBox.Show("Alle abgewählt.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowAbout(object? parameter)
        {
            MessageBox.Show("NodeConfigurator - OPC-UA Variable Manager\nVersion 1.0\n\n© 2024", "Über", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowDocumentation(object? parameter)
        {
            MessageBox.Show("Dokumentation wird implementiert...", "Dokumentation", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExpandAll(object? parameter)
        {
            foreach (var node in RootNodes)
            {
                node.IsExpanded = true;
            }
        }

        private void CollapseAll(object? parameter)
        {
            foreach (var node in RootNodes)
            {
                node.IsExpanded = false;
            }
        }

        private async Task ReadSelectedValueAsync()
        {
            if (SelectedNode == null) return;

            try
            {
                var nodeId = Opc.Ua.NodeId.Parse(SelectedNode.NodeId);
                var value = await _opcService.ReadValueAsync(nodeId);
                
                if (StatusCode.IsGood(value.StatusCode))
                {
                    SelectedNode.CurrentValue = value.Value?.ToString() ?? "null";
                    MessageBox.Show($"Wert: {SelectedNode.CurrentValue}", "Wert lesen", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Fehler beim Lesen: {value.StatusCode}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void WriteValue(object? parameter)
        {
            MessageBox.Show("Wert schreiben wird implementiert...", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Search(object? parameter)
        {
            MessageBox.Show($"Suche nach: {SearchText}", "Suche", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportJson(object? parameter)
        {
            MessageBox.Show("JSON Export wird implementiert...", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportXml(object? parameter)
        {
            MessageBox.Show("XML Export wird implementiert...", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportCsv(object? parameter)
        {
            MessageBox.Show("CSV Export wird implementiert...", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
