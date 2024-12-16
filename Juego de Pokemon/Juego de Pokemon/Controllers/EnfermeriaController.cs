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
		 ViewData["MostrarMenu"] = true;
            var pokemones = await _context.Enfermeria
                .Where(e => e.UsuarioId == usuario.Id && e.Estado == "En Proceso")
                .Include(e => e.UsuarioPokemon.Pokemon)
                .Include(e => e.UsuarioPokemon.Usuario) 
                .ToListAsync();

            return View(pokemones);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarATratamiento(int pokemonId)
        {

            var currentUserName = HttpContext.Session.GetString("CuentaUsuario");
            if (string.IsNullOrEmpty(currentUserName))
            {
                _logger.LogWarning("No se encontró el nombre de usuario en la sesión.");
                return RedirectToAction("Login", "Account");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == currentUserName);

            if (usuario == null)
            {
                _logger.LogWarning("No se encontró el usuario con la cuenta: {CuentaUsuario}", currentUserName);
                return NotFound();  // O una redirección adecuada
            }

            Enfermeria enfermeria = new Enfermeria
            {
                UsuarioId = usuario.Id,
                PokemonId = pokemonId,
                Estado = "En proceso",
            };

            _context.Enfermeria.Add(enfermeria);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Enfermeria()
        {
            var currentUserName = HttpContext.Session.GetString("CuentaUsuario");
            if (string.IsNullOrEmpty(currentUserName))
            {
                _logger.LogWarning("No se encontró el nombre de usuario en la sesión.");
                return RedirectToAction("Login", "Account");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == currentUserName);

            if (usuario == null)
            {
                _logger.LogWarning("No se encontró el usuario con la cuenta: {CuentaUsuario}", currentUserName);
                return NotFound();  // O una redirección adecuada
            }

            var pokemones = await _context.Enfermeria
                .Where (e => e.Estado == "En proceso")
                .Include(e => e.UsuarioPokemon.Pokemon)
                .Include(e => e.UsuarioPokemon.Usuario)
                .ToListAsync();

            ViewData["CuentaUsuario"] = currentUserName;
            return View(pokemones);
        }

        public async Task<IActionResult> CompletarTratamiento(int enfermeriaId, int usuarioId, int pokemonId)
        {
            var currentUserName = HttpContext.Session.GetString("CuentaUsuario");
            if (string.IsNullOrEmpty(currentUserName))
            {
                _logger.LogWarning("No se encontró el nombre de usuario en la sesión.");
                return RedirectToAction("Login", "Account");
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == currentUserName);

            var enfermeria = await _context.Enfermeria
                .FirstOrDefaultAsync(e => e.Id == enfermeriaId);

            if (enfermeria != null)
            {
                enfermeria.Estado = "Atendido";  // Cambiar estado a "Atendido"
                _context.Enfermeria.Update(enfermeria);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("No se encontró el registro de enfermería para el Pokémon con ID {PokemonId}.", pokemonId);
            }

            var pokemon = await _context.Pokemones
                .FirstOrDefaultAsync(up => up.Id == pokemonId);

            var usuarioPokemon = await _context.Usuario_Pokemones
                .FirstOrDefaultAsync(up => up.UsuarioId == usuarioId && up.PokemonId == pokemonId);

            if (usuarioPokemon != null)
            {
                usuarioPokemon.Vida = pokemon.HP;
                _context.Usuario_Pokemones.Update(usuarioPokemon);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("No se encontró el registro de Usuario_Pokemones para el Pokémon con ID {PokemonId}.", pokemonId);
            }

            ViewData["CuentaUsuario"] = currentUserName;
            return RedirectToAction("Enfermeria");
        }

    }
}
