using Microsoft.AspNetCore.Mvc;
using Juego_de_Pokemon.Data;
using Microsoft.EntityFrameworkCore;
using Juego_de_Pokemon.Models;

namespace Juego_de_Pokemon.Controllers
{
    public class PokemonController : Controller
    {
        private readonly ApplicationDbcontext _context;

        public PokemonController(ApplicationDbcontext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Pokemones()
        {
            var pokemones = await _context.Pokemones.ToListAsync() ?? new List<Pokemon>();
            return View(pokemones);

            if (pokemones == null || !pokemones.Any())
            {
                Console.WriteLine("No se encontraron Pokémon en la base de datos.");
            }

            return View(pokemones);
        }


    }
}
