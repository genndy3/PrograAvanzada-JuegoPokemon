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

            var mensajes = await _context.Mensajes
                .Where(m => m.DestinatarioId == currentUserId)
                .Include(m => m.Remitente)
                .ToListAsync();

            var mensajesNoLeidos = await _context.Mensajes
                .Where(m => m.DestinatarioId == currentUserId && m.Estado == "Enviado")
                .CountAsync();

            var retos = await _context.Retos
				.Where(m => m.RetadoId == currentUserId)
				.Include(m => m.Retador)
				.ToListAsync();

            var retosPendientes = await _context.Retos
                .Where(m => m.RetadoId == currentUserId && m.Estado == "Pendiente")
                .CountAsync();

            ViewData["CuentaUsuario"] = cuentaUsuario;
            ViewData["Mensajes"] = mensajes;
			ViewData["Retos"] = retos;
            ViewData["RetosPendientes"] = retosPendientes;
            ViewData["UsuarioActivo"] = currentUserId;
            ViewData["MensajesNoLeidos"] = mensajesNoLeidos;
			ViewData["MostrarBotones"] = true;

			return View(mensajes);
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

        public async Task<IActionResult> MarcarComoLeido(int usuarioId)
        {
            var mensajes = await _context.Mensajes
                .Where(m => m.DestinatarioId == usuarioId && m.Estado == "Enviado")
                .ToListAsync();

            foreach (var mensaje in mensajes)
            {
                mensaje.Estado = "Leído";
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

		[HttpPost]
		public async Task<IActionResult> BorrarMensaje(int id)
		{
			var mensaje = await _context.Mensajes.FindAsync(id);
			if (mensaje == null)
			{
				return NotFound();
			}

			_context.Mensajes.Remove(mensaje);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index", "Home");
		}

        [HttpPost]
        public async Task<IActionResult> BorrarReto(int id)
        {
            var reto = await _context.Retos.FindAsync(id);
            if (reto == null)
            {
                return NotFound();
            }

            _context.Retos.Remove(reto);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
