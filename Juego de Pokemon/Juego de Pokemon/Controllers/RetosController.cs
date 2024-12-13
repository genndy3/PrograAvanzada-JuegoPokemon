using Juego_de_Pokemon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juego_de_Pokemon.Controllers
{
	public class RetosController : Controller
	{
		private readonly ApplicationDbcontext _context;
		private readonly ILogger<RetosController> _logger;

		public RetosController(ApplicationDbcontext context, ILogger<RetosController> logger)
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

			ViewData["MensajesNoLeidos"] = mensajesNoLeidos;
			ViewData["CuentaUsuario"] = cuentaUsuario;
			ViewData["RetosPendientes"] = retosPendientes;
			ViewData["MostrarMenu"] = true;
			return View(retos);
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

			return RedirectToAction("Index", "Retos");
		}
	}
}
