using Juego_de_Pokemon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Juego_de_Pokemon.Models;

namespace Juego_de_Pokemon.Controllers
{
    public class EnfermeriaController : Controller
    {
		private readonly ApplicationDbcontext _context;
		private readonly ILogger<EnfermeriaController> _logger;

		// Constructor único con ambas dependencias
		public EnfermeriaController(ApplicationDbcontext context, ILogger<EnfermeriaController> logger)
		{
			_context = context;
			_logger = logger;
		}

        public async Task<ActionResult> Index()
        {
            // Obtener el nombre de usuario logueado desde la sesión
            var currentUserName = HttpContext.Session.GetString("CuentaUsuario");

            if (string.IsNullOrEmpty(currentUserName))
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtener el ID del usuario logueado
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == currentUserName);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Configurar ViewData para pasar datos al layout
            ViewData["CuentaUsuario"] = currentUserName;
            ViewData["MostrarBotones"] = true; // Cambia según tus necesidades
            ViewData["MostrarMenu"] = true;
            // Obtener la Pokédex del usuario
            var pokedex = await _context.Pokedex
                .FirstOrDefaultAsync(p => p.UsuarioId == usuario.Id);

            if (pokedex == null)
            {
                // Crear el modelo de vista con listas vacías
                var modelo = new EnfermeriaViewModel
                {
                    Usuario = currentUserName,
                    PokemonEnTratamiento = new List<Pokemon>(),
                    PokemonDisponibles = new List<Pokemon>()
                };

                return View(modelo);
            }

            // Recuperar Pokémon disponibles (Vida entre 1 y 100)
            var pokemonDisponibles = await _context.Pokedex_Pokemon
                .Include(pp => pp.Pokemon)
                .Where(pp => pp.PokedexId == pokedex.Id && pp.Vida > 1 && pp.Vida < 100)
                .Select(pp => pp.Pokemon)
                .ToListAsync();

            // Recuperar Pokémon en tratamiento (Vida <= 1)
            var pokemonEnTratamiento = await _context.Pokedex_Pokemon
                .Include(pp => pp.Pokemon)
                .Where(pp => pp.PokedexId == pokedex.Id && pp.Vida <= 1)
                .Select(pp => pp.Pokemon)
                .ToListAsync();

            // Recuperar P
            // Crear el modelo de vista
            var model = new EnfermeriaViewModel
            {
                Usuario = currentUserName,
                PokemonEnTratamiento = pokemonEnTratamiento,
                PokemonDisponibles = pokemonDisponibles
            };

            return View(model);
        }

        public async Task<IActionResult> Enfermeria()
        {
            // Obtener el nombre de usuario logueado desde la sesión
            var currentUserName = HttpContext.Session.GetString("CuentaUsuario");

            if (string.IsNullOrEmpty(currentUserName))
            {
                return RedirectToAction("Login", "Account");
            }

            // Obtener el ID del usuario logueado
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == currentUserName);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            // Configurar ViewData para pasar datos al layout
            ViewData["CuentaUsuario"] = currentUserName;
            ViewData["MostrarBotones"] = false; // Cambia según tus necesidades
            ViewData["MostrarMenu"] = false;
            // Recuperar todos los Pokémon pendientes
            var pokemonPendiente = await _context.Pokedex_Pokemon
                .Include(pp => pp.Pokemon)
                .Where(pp => pp.Estado != null && pp.Estado == "Pendiente")
                .Select(pp => pp.Pokemon)
                .ToListAsync();

            // Recuperar todos los Pokémon completados
            var pokemonCompletados = await _context.Pokedex_Pokemon
                .Include(pp => pp.Pokemon)
                .Where(pp => pp.Estado != null && pp.Estado == "Completado")
                .Select(pp => pp.Pokemon)
                .ToListAsync();

            // Crear el modelo de vista
            var model = new EnfermeriaViewModel
            {
                Usuario = currentUserName,
                PokemonPendiente = pokemonPendiente,
                PokemonCompletados = pokemonCompletados
            };

            return View(model);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> EnviarATratamiento(int id)
        {
            var Pokedex_Pokemon = await _context.Pokedex_Pokemon
                .FirstOrDefaultAsync(pp => pp.PokemonId == id);

            if (Pokedex_Pokemon != null)
            {
                Pokedex_Pokemon.Vida = 1; // Actualizar el estado del Pokémon
                Pokedex_Pokemon.Estado = "Pendiente";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> CompletarPokemon(int pokemonId)
        {
            // Obtener el Pokémon de la base de datos por su ID
            var Pokedex_Pokemon = await _context.Pokedex_Pokemon
                .FirstOrDefaultAsync(pp => pp.PokemonId == pokemonId);

            if (Pokedex_Pokemon != null)
            {
                // Actualizar el estado y la vida del Pokémon
                Pokedex_Pokemon.Estado = "Completado";
                Pokedex_Pokemon.Vida = 100; // Suponiendo que "Vida" es un campo de Pokémon que tiene un valor entre 0 y 100

                // Guardar los cambios
                await _context.SaveChangesAsync();
            }

            // Redirigir al usuario a la página de la enfermería para ver los cambios
            return RedirectToAction("Enfermeria");
        }

    }
}
