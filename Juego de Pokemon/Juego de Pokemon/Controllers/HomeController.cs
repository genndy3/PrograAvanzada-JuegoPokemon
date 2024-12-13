using Juego_de_Pokemon.Data;
using Juego_de_Pokemon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Juego_de_Pokemon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbcontext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbcontext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()

        {
            var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == cuentaUsuario);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUserId = user.Id;

            var mensajesNoLeidos = await _context.Mensajes
                .Where(m => m.DestinatarioId == currentUserId && m.Estado == "Enviado")
                .CountAsync();

            var retosPendientes = await _context.Retos
                .Where(m => m.RetadoId == currentUserId && m.Estado == "Pendiente")
                .CountAsync();

            ViewData["CuentaUsuario"] = cuentaUsuario;
			ViewData["RetosPendientes"] = retosPendientes;
			ViewData["UsuarioActivo"] = currentUserId;
            ViewData["MensajesNoLeidos"] = mensajesNoLeidos;
            ViewData["MostrarMenu"] = true;
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
