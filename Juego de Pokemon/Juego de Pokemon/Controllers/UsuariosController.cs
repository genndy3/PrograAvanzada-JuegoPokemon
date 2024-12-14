using Juego_de_Pokemon.Data;
using Juego_de_Pokemon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Juego_de_Pokemon.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbcontext _context;

        public UsuariosController(ApplicationDbcontext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Registro
        public IActionResult Registrar()
        {
            return View();
        }

        // POST: Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(Usuario usuario, string password)
        {
            if (ModelState.IsValid)
            {
                // Verifica si la cuenta ya existe
                if (_context.Usuarios.Any(u => u.CuentaUsuario == usuario.CuentaUsuario))
                {
                    ModelState.AddModelError("CuentaUsuario", "La cuenta de usuario ya existe.");
                    return View(usuario);
                }

                usuario.ContraseñaHash = HashPassword(password);

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View(usuario);
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Usuario usuario, string password)
        {
            if (ModelState.IsValid)
            {
                // Validar solo los campos necesarios: CuentaUsuario y ContraseñaHash
                var usuarioValido = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.CuentaUsuario == usuario.CuentaUsuario &&
                                              u.ContraseñaHash == HashPassword(password));

                if (usuarioValido != null)
                {
                    HttpContext.Session.SetString("CuentaUsuario", usuarioValido.CuentaUsuario);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Credenciales incorrectas.";
                }
            }

            return View(usuario);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CuentaUsuario");  // Elimina la sesión del usuario
            return RedirectToAction("Login");  // Redirige al formulario de login
        }

        public async Task<IActionResult> Listar()
        {
            var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == cuentaUsuario);

            if (user == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }
            else
            {
                var usuarios = await _context.Usuarios.ToListAsync();
                ViewData["CuentaUsuario"] = cuentaUsuario;
                return View(usuarios);
            }
        }

        public async Task<IActionResult> Editar(int? id)
        {
            var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.CuentaUsuario == cuentaUsuario);

            if (user == null)
            {
                return RedirectToAction("Login", "Usuarios");
            }

            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            ViewData["CuentaUsuario"] = cuentaUsuario;
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("Id,CuentaUsuario,ContraseñaHash,Nombre,RolId")] Usuario usuario)
        {
            var cuentaUsuario = HttpContext.Session.GetString("CuentaUsuario");
            if (id != usuario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["CuentaUsuario"] = cuentaUsuario;
                return RedirectToAction(nameof(Listar));
            }
            return View(usuario);
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound(); // Retorna un error si el usuario no existe
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Listar));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        // Método para calcular el hash de una contraseña
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Método para verificar la contraseña
        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }
    }
}
