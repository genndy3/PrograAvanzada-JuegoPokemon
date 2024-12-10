using Microsoft.EntityFrameworkCore;
using Juego_de_Pokemon.Models;
namespace Juego_de_Pokemon.Data
{
    public class ApplicationDbcontext : DbContext
    {
        public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options)
        {

        }

        //Agregar los modelos aquí (Cada modelo corresponde a una tabla en la BD)
        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Mensaje> Mensajes { get; set; }

        public DbSet<Reto> Retos { get; set; }
    }
}
