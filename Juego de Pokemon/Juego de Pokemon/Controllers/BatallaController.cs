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

            var pokemonSeleccionado = HttpContext.Session.GetString("PokemonSeleccionado");
            ViewData["PokemonSeleccionado"] = pokemonSeleccionado;

            return View();
        }

        public async Task<IActionResult> BeforeDuel()
        {
            var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == cuentaUsuario);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUserId = user.Id;

            var pokemones = await _context.Usuario_Pokemones
                .Where(up => up.UsuarioId == currentUserId)
                .Select(up => up.Pokemon)
                .Take(3) 
                .ToListAsync();

            ViewData["Pokemones"] = pokemones;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SeleccionarPokemon(int pokemonId)
        {
            var pokemonSeleccionado = await _context.Pokemones
                .FirstOrDefaultAsync(p => p.Id == pokemonId);

            if (pokemonSeleccionado != null)
            {
                HttpContext.Session.SetString("PokemonSeleccionado", pokemonSeleccionado.Imagen);
            }
            return RedirectToAction("Index");
        }
    }
}
