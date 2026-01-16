using Microsoft.AspNetCore.Mvc;
using NodeConfigurator.Web.Services;

namespace NodeConfigurator.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly SessionManagerService _sessionManager;

        public HomeController(SessionManagerService sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public IActionResult Index()
        {
            ViewBag.IsConnected = _sessionManager.IsConnected(HttpContext.Session.Id);
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
    }
}
