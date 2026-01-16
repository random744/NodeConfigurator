using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Opc.Ua;
using NodeConfigurator.Web.Models;

namespace NodeConfigurator.Web.Services
{
    public interface IOpcUaClientService
    {
        Task<bool> ConnectAsync(ServerConnectionConfig config);
        Task DisconnectAsync();
        Task<List<ReferenceDescription>> BrowseAsync(NodeId nodeId);
        Task<DataValue> ReadValueAsync(NodeId nodeId);
        bool IsConnected { get; }
        event EventHandler<string>? ConnectionStatusChanged;
    }
}
