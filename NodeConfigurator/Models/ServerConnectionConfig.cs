namespace NodeConfigurator.Models
{
    public class ServerConnectionConfig
    {
        public string Url { get; set; } = "opc.tcp://localhost:4840";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SecurityMode { get; set; } = "None";
        public string SecurityPolicy { get; set; } = "None";
        public bool AutoAcceptCertificates { get; set; } = true;
        public int Timeout { get; set; } = 15000;
        public int SessionTimeout { get; set; } = 60000;
    }
}
