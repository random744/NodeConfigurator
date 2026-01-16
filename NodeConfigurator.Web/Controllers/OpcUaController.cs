using Microsoft.AspNetCore.Mvc;
using NodeConfigurator.Web.Models;
using NodeConfigurator.Web.Models.ViewModels;
using NodeConfigurator.Web.Services;
using Opc.Ua;

namespace NodeConfigurator.Web.Controllers
{
    public class OpcUaController : Controller
    {
        private readonly IOpcUaClientService _opcService;
        private readonly SessionManagerService _sessionManager;

        public OpcUaController(IOpcUaClientService opcService, SessionManagerService sessionManager)
        {
            _opcService = opcService;
            _sessionManager = sessionManager;
        }

        [HttpGet]
        public IActionResult Connect()
        {
            var viewModel = new ConnectionViewModel
            {
                ServerUrl = "opc.tcp://localhost:53530/OPCUA/SimulationServer",
                RecentUrls = new List<string>
                {
                    "opc.tcp://localhost:53530/OPCUA/SimulationServer",
                    "opc.tcp://localhost:4840",
                    "opc.tcp://localhost:48010"
                }
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Connect(ConnectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var config = new ServerConnectionConfig
            {
                Url = model.ServerUrl,
                Username = model.UseAuthentication ? model.Username : string.Empty,
                Password = model.UseAuthentication ? model.Password : string.Empty,
                SecurityMode = model.SecurityMode,
                SecurityPolicy = model.SecurityPolicy,
                AutoAcceptCertificates = model.AutoAcceptCertificates,
                Timeout = model.Timeout,
                SessionTimeout = model.SessionTimeout
            };

            var success = await _opcService.ConnectAsync(config);

            if (success)
            {
                _sessionManager.RegisterConnection(HttpContext.Session.Id);
                TempData["SuccessMessage"] = "Erfolgreich mit OPC-UA Server verbunden!";
                return RedirectToAction("Browse");
            }
            else
            {
                ModelState.AddModelError("", "Verbindung zum Server fehlgeschlagen.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Disconnect()
        {
            await _opcService.DisconnectAsync();
            _sessionManager.UnregisterConnection(HttpContext.Session.Id);
            TempData["InfoMessage"] = "Verbindung getrennt.";
            return RedirectToAction("Connect");
        }

        [HttpGet]
        public IActionResult Browse()
        {
            if (!_sessionManager.IsConnected(HttpContext.Session.Id))
            {
                TempData["ErrorMessage"] = "Bitte zuerst mit einem Server verbinden.";
                return RedirectToAction("Connect");
            }

            var viewModel = new BrowseViewModel
            {
                SelectedNodes = _sessionManager.GetSelectedNodes(HttpContext.Session.Id)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetNodes(string nodeId = "")
        {
            if (!_opcService.IsConnected)
            {
                return Json(new { success = false, message = "Nicht verbunden" });
            }

            try
            {
                var startNodeId = string.IsNullOrEmpty(nodeId) 
                    ? ObjectIds.ObjectsFolder 
                    : NodeId.Parse(nodeId);

                var references = await _opcService.BrowseAsync(startNodeId);
                
                var nodes = references.Select(r => new NodeViewModel
                {
                    NodeId = r.NodeId.ToString(),
                    DisplayName = r.DisplayName.Text ?? r.BrowseName.Name,
                    BrowseName = r.BrowseName.Name,
                    NodeClass = r.NodeClass.ToString(),
                    HasChildren = r.NodeClass != NodeClass.Variable,
                    CanBeSelected = r.NodeClass == NodeClass.Variable
                }).ToList();

                return Json(new { success = true, nodes });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult SelectNode([FromBody] SelectedNode node)
        {
            _sessionManager.AddSelectedNode(HttpContext.Session.Id, node);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult UnselectNode([FromBody] string nodeId)
        {
            _sessionManager.RemoveSelectedNode(HttpContext.Session.Id, nodeId);
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> ReadValue(string nodeId)
        {
            if (!_opcService.IsConnected)
            {
                return Json(new { success = false, message = "Nicht verbunden" });
            }

            try
            {
                var value = await _opcService.ReadValueAsync(NodeId.Parse(nodeId));
                return Json(new 
                { 
                    success = true, 
                    value = value.Value?.ToString() ?? "null",
                    statusCode = value.StatusCode.ToString(),
                    timestamp = value.SourceTimestamp.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
