using Juego_de_Pokemon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Juego_de_Pokemon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()

        {
            var CuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");

            ViewData["CuentaUsuario"] = CuentaUsuario;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
