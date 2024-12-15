using Juego_de_Pokemon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juego_de_Pokemon.Controllers
{
	public class BatallaController : Controller
	{

        private readonly ApplicationDbcontext _context;
        private readonly ILogger<BatallaController> _logger;

        public BatallaController(ApplicationDbcontext context, ILogger<BatallaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ActionResult> Index()
		{
			var CuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");

			var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == CuentaUsuario);

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

            ViewData["MostrarMenu"] = true;
            ViewData["CuentaUsuario"] = CuentaUsuario;
            ViewData["MensajesNoLeidos"] = mensajesNoLeidos;
            ViewData["RetosPendientes"] = retosPendientes;
            return View();
		}
	}
}
