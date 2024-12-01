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

        // Constructor único con ambas dependencias
        public RedEntrenadoresController(ApplicationDbcontext context, ILogger<RedEntrenadoresController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            // Obtener el nombre de usuario logueado desde la sesión
            var currentUserName = HttpContext.Session.GetString("CuentaUsuario");

            var usuarios = await _context.Usuarios
                                          .Where(u => u.CuentaUsuario != currentUserName)
                                          .ToListAsync();

			var CuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");

			ViewData["CuentaUsuario"] = CuentaUsuario;
            ViewData["MostrarBotones"] = false;
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

    }
}
