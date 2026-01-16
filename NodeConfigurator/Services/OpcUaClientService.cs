using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using NodeConfigurator.Models;

namespace NodeConfigurator.Services
{
    public class OpcUaClientService : IOpcUaClientService
    {
        private Session? _session;
        private ApplicationConfiguration? _configuration;

        public bool IsConnected => _session?.Connected ?? false;

        public event EventHandler<string>? ConnectionStatusChanged;

        public async Task<bool> ConnectAsync(ServerConnectionConfig config)
        {
            try
            {
                // Create application configuration
                _configuration = new ApplicationConfiguration
                {
                    ApplicationName = "NodeConfigurator",
                    ApplicationType = ApplicationType.Client,
                    SecurityConfiguration = new SecurityConfiguration
                    {
                        ApplicationCertificate = new CertificateIdentifier(),
                        AutoAcceptUntrustedCertificates = config.AutoAcceptCertificates,
                        RejectSHA1SignedCertificates = false,
                        MinimumCertificateKeySize = 2048
                    },
                    TransportQuotas = new TransportQuotas
                    {
                        OperationTimeout = config.Timeout,
                        MaxStringLength = 1048576,
                        MaxByteStringLength = 1048576,
                        MaxArrayLength = 65535,
                        MaxMessageSize = 4194304,
                        MaxBufferSize = 65535
                    },
                    ClientConfiguration = new ClientConfiguration
                    {
                        DefaultSessionTimeout = config.SessionTimeout
                    }
                };

                await _configuration.Validate(ApplicationType.Client);

                // Certificate validation
                bool certValidator = true;
                _configuration.CertificateValidator.CertificateValidation += (sender, e) =>
                {
                    if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted && config.AutoAcceptCertificates)
                    {
                        e.Accept = true;
                    }
                };

                // Create endpoint description
                var endpointDescription = CoreClientUtils.SelectEndpoint(config.Url, false, config.Timeout);

                // Create endpoint configuration
                var endpointConfiguration = EndpointConfiguration.Create(_configuration);
                endpointConfiguration.OperationTimeout = config.Timeout;

                // Create configured endpoint
                var configuredEndpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                // Create user identity
                UserIdentity userIdentity;
                if (!string.IsNullOrEmpty(config.Username))
                {
                    userIdentity = new UserIdentity(config.Username, config.Password);
                }
                else
                {
                    userIdentity = new UserIdentity();
                }

                // Create session
                _session = await Session.Create(
                    _configuration,
                    configuredEndpoint,
                    false,
                    "NodeConfigurator Session",
                    (uint)config.SessionTimeout,
                    userIdentity,
                    null
                );

                if (_session != null && _session.Connected)
                {
                    ConnectionStatusChanged?.Invoke(this, "Verbunden");
                    return true;
                }

                ConnectionStatusChanged?.Invoke(this, "Verbindung fehlgeschlagen");
                return false;
            }
            catch (Exception ex)
            {
                ConnectionStatusChanged?.Invoke(this, $"Fehler: {ex.Message}");
                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            if (_session != null)
            {
                await Task.Run(() => _session.Close());
                _session.Dispose();
                _session = null;
            }
            ConnectionStatusChanged?.Invoke(this, "Getrennt");
        }

        public async Task<List<ReferenceDescription>> BrowseAsync(NodeId nodeId)
        {
            if (_session == null || !_session.Connected)
            {
                return new List<ReferenceDescription>();
            }

            try
            {
                var browseDescription = new BrowseDescription
                {
                    NodeId = nodeId,
                    BrowseDirection = BrowseDirection.Forward,
                    ReferenceTypeId = ReferenceTypeIds.HierarchicalReferences,
                    IncludeSubtypes = true,
                    NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method),
                    ResultMask = (uint)BrowseResultMask.All
                };

                var browseDescriptions = new BrowseDescriptionCollection { browseDescription };

                BrowseResultCollection results;
                DiagnosticInfoCollection diagnostics;

                _session.Browse(
                    null,
                    null,
                    0,
                    browseDescriptions,
                    out results,
                    out diagnostics
                );

                ClientBase.ValidateResponse(results, browseDescriptions);
                ClientBase.ValidateDiagnosticInfos(diagnostics, browseDescriptions);

                var references = new List<ReferenceDescription>();

                if (results.Count > 0 && StatusCode.IsGood(results[0].StatusCode))
                {
                    references.AddRange(results[0].References);

                    // Handle continuation point
                    byte[] continuationPoint = results[0].ContinuationPoint;
                    while (continuationPoint != null && continuationPoint.Length > 0)
                    {
                        ByteStringCollection continuationPoints = new ByteStringCollection { continuationPoint };
                        _session.BrowseNext(null, false, continuationPoints, out results, out diagnostics);
                        ClientBase.ValidateResponse(results, continuationPoints);
                        ClientBase.ValidateDiagnosticInfos(diagnostics, continuationPoints);

                        if (results.Count > 0 && StatusCode.IsGood(results[0].StatusCode))
                        {
                            references.AddRange(results[0].References);
                            continuationPoint = results[0].ContinuationPoint;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                return references;
            }
            catch (Exception)
            {
                return new List<ReferenceDescription>();
            }
        }

        public async Task<DataValue> ReadValueAsync(NodeId nodeId)
        {
            if (_session == null || !_session.Connected)
            {
                return new DataValue(StatusCodes.BadNotConnected);
            }

            try
            {
                return await Task.Run(() => _session.ReadValue(nodeId));
            }
            catch (Exception)
            {
                return new DataValue(StatusCodes.Bad);
            }
        }
    }
}
