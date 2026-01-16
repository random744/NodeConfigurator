namespace NodeConfigurator.Models
{
    public class SelectedNode
    {
        public string NodeId { get; set; } = string.Empty;
        public string BrowseName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public int NamespaceIndex { get; set; }
    }
}
