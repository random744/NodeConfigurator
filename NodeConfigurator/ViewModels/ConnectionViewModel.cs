using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NodeConfigurator.Commands;
using NodeConfigurator.Models;

namespace NodeConfigurator.ViewModels
{
    public class ConnectionViewModel : ViewModelBase
    {
        private string _serverUrl = "opc.tcp://localhost:4840";
        public string ServerUrl
        {
            get => _serverUrl;
            set => SetProperty(ref _serverUrl, value);
        }
        
        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }
        
        public string Password { get; set; } = string.Empty;
        
        private bool _useAuthentication;
        public bool UseAuthentication
        {
            get => _useAuthentication;
            set => SetProperty(ref _useAuthentication, value);
        }
        
        private string _securityMode = "None";
        public string SecurityMode
        {
            get => _securityMode;
            set => SetProperty(ref _securityMode, value);
        }
        
        private string _securityPolicy = "None";
        public string SecurityPolicy
        {
            get => _securityPolicy;
            set => SetProperty(ref _securityPolicy, value);
        }
        
        private bool _autoAcceptCertificates = true;
        public bool AutoAcceptCertificates
        {
            get => _autoAcceptCertificates;
            set => SetProperty(ref _autoAcceptCertificates, value);
        }
        
        private int _timeout = 15000;
        public int Timeout
        {
            get => _timeout;
            set => SetProperty(ref _timeout, value);
        }
        
        private int _sessionTimeout = 60000;
        public int SessionTimeout
        {
            get => _sessionTimeout;
            set => SetProperty(ref _sessionTimeout, value);
        }
        
        public ObservableCollection<string> RecentUrls { get; set; } = new()
        {
            "opc.tcp://localhost:4840",
            "opc.tcp://localhost:48010",
            "opc.tcp://127.0.0.1:4840"
        };
        
        public ICommand TestConnectionCommand { get; }
        
        public ConnectionViewModel()
        {
            TestConnectionCommand = new RelayCommand(_ =>
            {
                MessageBox.Show("Verbindungstest wird implementiert...", "Test", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }
        
        public ServerConnectionConfig GetConfig()
        {
            return new ServerConnectionConfig
            {
                Url = ServerUrl,
                Username = UseAuthentication ? Username : string.Empty,
                Password = UseAuthentication ? Password : string.Empty,
                SecurityMode = SecurityMode,
                SecurityPolicy = SecurityPolicy,
                AutoAcceptCertificates = AutoAcceptCertificates,
                Timeout = Timeout,
                SessionTimeout = SessionTimeout
            };
        }
    }
}
