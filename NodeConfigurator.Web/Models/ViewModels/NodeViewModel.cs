namespace NodeConfigurator.Web.Models.ViewModels
{
    public class NodeViewModel
    {
        public string NodeId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string BrowseName { get; set; } = string.Empty;
        public string NodeClass { get; set; } = string.Empty;
        public bool HasChildren { get; set; }
        public bool CanBeSelected { get; set; }
        public string DataType { get; set; } = string.Empty;
    }
}
