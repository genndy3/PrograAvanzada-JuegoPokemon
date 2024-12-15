using Juego_de_Pokemon.Data;
using Juego_de_Pokemon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juego_de_Pokemon.Controllers
{
	public class PokemonController : Controller
	{
		private readonly ApplicationDbcontext _context;

		public PokemonController(ApplicationDbcontext context)
		{
			_context = context;
		}

		// GET: Listar Pokemones
		public async Task<IActionResult> Listar()
		{
			var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
			if (string.IsNullOrEmpty(cuentaUsuario))
			{
				return RedirectToAction("Login", "Usuarios");
			}

			var pokemones = await _context.Pokemones.ToArrayAsync();
			ViewData["CuentaUsuario"] = cuentaUsuario;
			return View(pokemones);
		}

		public IActionResult Registrar()
		{
			var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
			if (string.IsNullOrEmpty(cuentaUsuario))
			{
				return RedirectToAction("Login", "Usuarios");
			}

			ViewData["CuentaUsuario"] = cuentaUsuario;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Registrar(Pokemon pokemon)
		{
			if (ModelState.IsValid)
			{
				_context.Pokemones.Add(pokemon);
				await _context.SaveChangesAsync();

				return RedirectToAction("Listar", "Pokemon");
			}

			return View(pokemon);
		}

		// GET: Editar Pokémon
		public async Task<IActionResult> Editar(int? id)
		{
			var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
			if (string.IsNullOrEmpty(cuentaUsuario))
			{
				return RedirectToAction("Login", "Usuarios");
			}

			if (id == null)
			{
				return NotFound();
			}

			var pokemon = await _context.Pokemones.FindAsync(id);
			if (pokemon == null)
			{
				return NotFound();
			}

			ViewData["CuentaUsuario"] = cuentaUsuario;
			return View(pokemon);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Editar(int id, Pokemon pokemon)
		{
			if (id != pokemon.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(pokemon);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!_context.Pokemones.Any(e => e.Id == pokemon.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}

				return RedirectToAction("Listar", "Pokemon");
			}

			return View(pokemon);
		}

		// POST: Confirmar eliminación de Pokémon
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Eliminar(int id)
		{
			var pokemon = await _context.Pokemones.FindAsync(id);
			if (pokemon == null)
			{
				return NotFound();
			}

			_context.Pokemones.Remove(pokemon);
			await _context.SaveChangesAsync();

			return RedirectToAction("Listar", "Pokemon");
		}
	}
}