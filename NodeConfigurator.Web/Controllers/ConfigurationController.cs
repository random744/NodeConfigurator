using Microsoft.AspNetCore.Mvc;
using NodeConfigurator.Web.Models;
using NodeConfigurator.Web.Services;
using System.Text.Json;

namespace NodeConfigurator.Web.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly SessionManagerService _sessionManager;

        public ConfigurationController(SessionManagerService sessionManager)
        {
            _sessionManager = sessionManager;
        }

        [HttpGet]
        public IActionResult Export()
        {
            var selectedNodes = _sessionManager.GetSelectedNodes(HttpContext.Session.Id);
            ViewBag.SelectedNodesCount = selectedNodes.Count;
            return View();
        }

        [HttpPost]
        public IActionResult DownloadJson()
        {
            var selectedNodes = _sessionManager.GetSelectedNodes(HttpContext.Session.Id);
            
            var config = new NodeConfiguration
            {
                Name = "NodeConfiguration",
                CreatedDate = DateTime.Now,
                SelectedNodes = selectedNodes
            };

            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });

            var fileName = $"NodeConfig_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            return File(System.Text.Encoding.UTF8.GetBytes(json), "application/json", fileName);
        }

        [HttpPost]
        public IActionResult DownloadXml()
        {
            var selectedNodes = _sessionManager.GetSelectedNodes(HttpContext.Session.Id);
            
            // Simple XML generation
            var xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<NodeConfiguration>
    <Name>NodeConfiguration</Name>
    <CreatedDate>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</CreatedDate>
    <SelectedNodes>
{string.Join("\n", selectedNodes.Select(n => $@"        <Node>
            <NodeId>{n.NodeId}</NodeId>
            <DisplayName>{n.DisplayName}</DisplayName>
            <BrowseName>{n.BrowseName}</BrowseName>
            <DataType>{n.DataType}</DataType>
        </Node>"))}
    </SelectedNodes>
</NodeConfiguration>";

            var fileName = $"NodeConfig_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
            return File(System.Text.Encoding.UTF8.GetBytes(xml), "application/xml", fileName);
        }

        [HttpPost]
        public IActionResult DownloadCsv()
        {
            var selectedNodes = _sessionManager.GetSelectedNodes(HttpContext.Session.Id);
            
            var csv = "NodeId,DisplayName,BrowseName,DataType\n" +
                      string.Join("\n", selectedNodes.Select(n => 
                          $"{n.NodeId},{n.DisplayName},{n.BrowseName},{n.DataType}"));

            var fileName = $"NodeConfig_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportJson(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Bitte eine Datei auswählen.";
                return RedirectToAction("Import");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var config = await JsonSerializer.DeserializeAsync<NodeConfiguration>(stream);

                if (config?.SelectedNodes != null)
                {
                    foreach (var node in config.SelectedNodes)
                    {
                        _sessionManager.AddSelectedNode(HttpContext.Session.Id, node);
                    }

                    TempData["SuccessMessage"] = $"{config.SelectedNodes.Count} Knoten erfolgreich importiert.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Fehler beim Import: {ex.Message}";
            }

            return RedirectToAction("Import");
        }

        [HttpPost]
        public IActionResult ClearSelection()
        {
            _sessionManager.ClearSelectedNodes(HttpContext.Session.Id);
            TempData["InfoMessage"] = "Auswahl gelöscht.";
            return RedirectToAction("Export");
        }
    }
}
