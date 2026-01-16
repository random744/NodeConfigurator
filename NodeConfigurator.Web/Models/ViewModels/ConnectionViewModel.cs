using System.ComponentModel.DataAnnotations;

namespace NodeConfigurator.Web.Models.ViewModels
{
    public class ConnectionViewModel
    {
        [Required(ErrorMessage = "Server-URL ist erforderlich")]
        public string ServerUrl { get; set; } = "opc.tcp://localhost:4840";

        public bool UseAuthentication { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string SecurityMode { get; set; } = "None";

        public string SecurityPolicy { get; set; } = "None";

        public bool AutoAcceptCertificates { get; set; } = true;

        public int Timeout { get; set; } = 15000;

        public int SessionTimeout { get; set; } = 60000;

        public List<string> RecentUrls { get; set; } = new();
    }
}
