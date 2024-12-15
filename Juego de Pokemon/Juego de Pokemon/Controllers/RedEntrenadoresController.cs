using Microsoft.AspNetCore.Mvc;
using Juego_de_Pokemon.Data;
using Juego_de_Pokemon.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Juego_de_Pokemon.Controllers
{

    public class RedEntrenadoresController : Controller
    {
        private readonly ApplicationDbcontext _context;
        private readonly ILogger<RedEntrenadoresController> _logger;

        public RedEntrenadoresController(ApplicationDbcontext context, ILogger<RedEntrenadoresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
			var CuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");

			var usuarios = await _context.Usuarios
                                          .Where(u => u.CuentaUsuario != CuentaUsuario && u.RolId == 1)
                                          .ToListAsync();

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

			ViewData["MensajesNoLeidos"] = mensajesNoLeidos;
			ViewData["RetosPendientes"] = retosPendientes;
			ViewData["CuentaUsuario"] = CuentaUsuario;
            ViewData["MostrarMenu"] = true;
            return View(usuarios);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarMensaje(int destinatarioId, string contenido)
        {
            // Validaciones
            if (destinatarioId == 0 || string.IsNullOrEmpty(contenido))
            {
                _logger.LogWarning("Los parámetros enviados no son válidos.");
                return BadRequest("Los parámetros no son válidos.");
            }

            var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
            var remitente = await _context.Usuarios.FirstOrDefaultAsync(u => u.CuentaUsuario == cuentaUsuario);

            if (remitente == null)
            {
                _logger.LogWarning("Usuario no encontrado.");
                return BadRequest("Usuario no encontrado.");
            }

            if (string.IsNullOrEmpty(contenido))
            {
                _logger.LogWarning("Contenido vacío.");
                return BadRequest("El contenido del mensaje no puede estar vacío.");
            }

            Mensaje mensaje = new Mensaje
            {
                RemitenteId = remitente.Id,
                DestinatarioId = destinatarioId,
                Contenido = contenido,
                FechaEnvio = DateTime.Now,
                Estado = "Enviado"
            };

            _context.Mensajes.Add(mensaje);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Mensaje guardado correctamente.");
            return RedirectToAction("Index");
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> GuardarReto(int RetadoId)
		{
			_logger.LogInformation("RetadoId recibido: " + RetadoId);

			var usuarioRetado = await _context.Usuarios.FindAsync(RetadoId);
			if (usuarioRetado == null)
			{
				_logger.LogWarning("El usuario retado no existe.");
				return BadRequest("El usuario retado no existe.");
			}

			var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
			var retador = await _context.Usuarios.FirstOrDefaultAsync(u => u.CuentaUsuario == cuentaUsuario);

			if (retador == null)
			{
				_logger.LogWarning("Usuario no encontrado.");
				return BadRequest("Usuario no encontrado.");
			}

			Reto reto = new Reto
			{
				RetadorId = retador.Id,
				RetadoId = RetadoId,
				FechaReto = DateTime.Now,
				Estado = "Pendiente"
			};

			_context.Retos.Add(reto);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Reto enviado correctamente.");
			return RedirectToAction("Index");
		}


	}
}
