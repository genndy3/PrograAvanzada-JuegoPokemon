using Juego_de_Pokemon.Data;
using Juego_de_Pokemon.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Juego_de_Pokemon.Controllers
{
	public class MensajesController : Controller
	{
		private readonly ApplicationDbcontext _context;
		private readonly ILogger<MensajesController> _logger;

		public MensajesController(ApplicationDbcontext context, ILogger<MensajesController> logger)
		{
			_context = context;
			_logger = logger;
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

			var retosPendientes = await _context.Retos
				.Where(m => m.RetadoId == currentUserId && m.Estado == "Pendiente")
				.CountAsync();

			ViewData["MensajesNoLeidos"] = mensajesNoLeidos;
			ViewData["RetosPendientes"] = retosPendientes;
			ViewData["CuentaUsuario"] = cuentaUsuario;
			ViewData["MostrarMenu"] = true;
			return View(mensajes);
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

            return RedirectToAction("Index", "Mensajes");
        }
    }
}
