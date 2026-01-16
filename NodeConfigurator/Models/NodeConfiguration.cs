using System;
using System.Collections.Generic;

namespace NodeConfigurator.Models
{
    public class NodeConfiguration
    {
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public ServerConnectionConfig? ServerConfig { get; set; }
        public List<SelectedNode> SelectedNodes { get; set; } = new();
    }
}
