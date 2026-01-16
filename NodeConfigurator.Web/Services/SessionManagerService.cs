using NodeConfigurator.Web.Models;
using System.Collections.Concurrent;

namespace NodeConfigurator.Web.Services
{
    public class SessionManagerService
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _connectedSessions = new();
        private readonly ConcurrentDictionary<string, List<SelectedNode>> _selectedNodes = new();

        public void RegisterConnection(string sessionId)
        {
            _connectedSessions.TryAdd(sessionId, new HashSet<string>());
        }

        public void UnregisterConnection(string sessionId)
        {
            _connectedSessions.TryRemove(sessionId, out _);
        }

        public bool IsConnected(string sessionId)
        {
            return _connectedSessions.ContainsKey(sessionId);
        }

        public void AddSelectedNode(string sessionId, SelectedNode node)
        {
            if (!_selectedNodes.ContainsKey(sessionId))
            {
                _selectedNodes[sessionId] = new List<SelectedNode>();
            }

            var existingNode = _selectedNodes[sessionId].FirstOrDefault(n => n.NodeId == node.NodeId);
            if (existingNode == null)
            {
                _selectedNodes[sessionId].Add(node);
            }
        }

        public void RemoveSelectedNode(string sessionId, string nodeId)
        {
            if (_selectedNodes.ContainsKey(sessionId))
            {
                _selectedNodes[sessionId].RemoveAll(n => n.NodeId == nodeId);
            }
        }

        public List<SelectedNode> GetSelectedNodes(string sessionId)
        {
            return _selectedNodes.ContainsKey(sessionId) 
                ? _selectedNodes[sessionId] 
                : new List<SelectedNode>();
        }

        public void ClearSelectedNodes(string sessionId)
        {
            if (_selectedNodes.ContainsKey(sessionId))
            {
                _selectedNodes[sessionId].Clear();
            }
        }
    }
}
