using Juego_de_Pokemon.Data;
using Juego_de_Pokemon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Juego_de_Pokemon.Controllers
{
    public class BeforeDuelController : Controller
    {
        private readonly ApplicationDbcontext _context;

        public BeforeDuelController(ApplicationDbcontext context)
        {
            _context = context;
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

            var pokemones = await _context.Usuario_Pokemones
                .Where(up => up.UsuarioId == currentUserId)
                .Take(3)  
                .Select(up => up.Pokemon)
                .ToListAsync();

            if (pokemones.Count < 3)
            {               
                ViewData["Mensaje"] = "No hay 3 pokemones asignados";
            }

            ViewData["CuentaUsuario"] = cuentaUsuario;
            ViewData["Pokemones"] = pokemones;

            return View();
        }
    }
}
