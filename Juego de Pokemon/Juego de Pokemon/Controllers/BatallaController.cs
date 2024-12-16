using Juego_de_Pokemon.Data;
using Juego_de_Pokemon.Models;
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

        public async Task<ActionResult> Index(int batallaId)
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

			var batalla = await _context.Batalla
                .Include(b => b.Usuario1)
                .Include(b => b.Usuario2)
                .Include(b => b.Ganador)
                .FirstOrDefaultAsync(b => b.Id == batallaId);

			if (batalla == null)
            {
                return NotFound();
            }

			var pokemon1 = await _context.Pokemones
				.FirstOrDefaultAsync(u => u.Id == batalla.PokemonUsuario1Id);

			var pokemon2 = await _context.Pokemones
				.FirstOrDefaultAsync(u => u.Id == batalla.PokemonUsuario2Id);

            var Usuario_Pokemon1 = await _context.Usuario_Pokemones
				.FirstOrDefaultAsync(u => u.UsuarioId == batalla.Usuario1Id && u.PokemonId == batalla.PokemonUsuario1Id);

			var Usuario_Pokemon2 = await _context.Usuario_Pokemones
				.FirstOrDefaultAsync(u => u.UsuarioId == batalla.Usuario2Id && u.PokemonId == batalla.PokemonUsuario2Id);

			ViewData["MostrarMenu"] = true;
            ViewData["CuentaUsuario"] = CuentaUsuario;
            ViewData["MensajesNoLeidos"] = mensajesNoLeidos;
            ViewData["RetosPendientes"] = retosPendientes;
			ViewData["Pokemon1"] = pokemon1;
			ViewData["Pokemon2"] = pokemon2;
			ViewData["UsuarioPokemon1"] = Usuario_Pokemon1;
			ViewData["UsuarioPokemon2"] = Usuario_Pokemon2;
			return View(batalla);
        }

        public async Task<IActionResult> BeforeDuel(int idReto)
        {
            var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == cuentaUsuario);
			var reto = await _context.Retos.FindAsync(idReto);

			if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUserId = user.Id;

			var usuario_pokemones = await _context.Usuario_Pokemones
		.Where(up => up.UsuarioId == currentUserId && up.Vida >= 50)
		.Include(up => up.Pokemon)
		.ToListAsync();

			var pokemones = await _context.Usuario_Pokemones
                .Where(up => up.UsuarioId == currentUserId && up.Vida >= 50)
                .Take(3)
                .Include(up => up.Pokemon)
                .ToListAsync();

			ViewData["Pokemones"] = pokemones;
			ViewData["RetoId"] = reto.Id;
            ViewData["UsuarioPokemones"] = usuario_pokemones;

			return View();
        }

        [HttpPost]
        public async Task<IActionResult> SeleccionarPokemon(int pokemonId, int retoId)
        {

			var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
			var user = await _context.Usuarios
				.FirstOrDefaultAsync(u => u.CuentaUsuario == cuentaUsuario);

			if (user == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var currentUserId = user.Id;

			var reto = await _context.Retos.FindAsync(retoId);

            if (reto == null)
            {
                return NotFound("Reto no encontrado.");
            }

			var pokemonUsuario1 = await _context.Usuario_Pokemones
				.Where(up => up.UsuarioId == currentUserId && up.PokemonId == pokemonId)
                .FirstOrDefaultAsync();

			var pokemonesUsuario2 = await _context.Usuario_Pokemones
                .Where(up => up.UsuarioId == reto.RetadorId)
				.ToListAsync();

				// Genera un índice aleatorio
				var random = new Random();
				var indiceAleatorio = random.Next(0, pokemonesUsuario2.Count); // Índice entre 0 y Count - 1

				// Selecciona el Pokémon aleatorio
				var pokemonUsuario2 = pokemonesUsuario2[indiceAleatorio];


			Batalla batalla = new Batalla
			{
				Usuario1Id = reto.RetadoId,
				Usuario2Id = reto.RetadorId,
				PokemonUsuario1Id = pokemonUsuario1.PokemonId,
                PokemonUsuario2Id = pokemonUsuario2.PokemonId,
                GanadorId = reto.RetadoId,//temporal
                Fecha = DateTime.Now
            };

			_context.Batalla.Add(batalla);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index", new { batallaId = batalla.Id });
		}

		[HttpPost]
		public async Task<IActionResult> ActualizarVidas(int usuarioId1, int usuarioId2, int pokemonId1, int pokemonId2, int vidaPokemon1, int vidaPokemon2)
		{
			_logger.LogInformation($"usuarioId1: {usuarioId1}, usuarioId2: {usuarioId2}");
			_logger.LogInformation($"pokemonId1: {pokemonId1}, pokemonId2: {pokemonId2}");
			_logger.LogInformation($"vidaPokemon1: {vidaPokemon1}, vidaPokemon2: {vidaPokemon2}");


			var pokemonUsuario1 = await _context.Usuario_Pokemones
				.Where(up => up.UsuarioId == usuarioId1 && up.PokemonId == pokemonId1)
				.FirstOrDefaultAsync();

			var pokemonUsuario2 = await _context.Usuario_Pokemones
				.Where(up => up.UsuarioId == usuarioId2 && up.PokemonId == pokemonId2)
				.FirstOrDefaultAsync();

			if (pokemonUsuario1 != null && pokemonUsuario2 != null)
			{
				pokemonUsuario1.Vida = vidaPokemon1;
				pokemonUsuario2.Vida = vidaPokemon2;

                _context.SaveChanges();
			}

            return RedirectToAction("Index", "Home");
        }
	}
}
