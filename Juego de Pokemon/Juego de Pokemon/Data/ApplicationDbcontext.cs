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
        public DbSet<Pokemon> Pokemones { get; set; }
        public DbSet<Usuario_Pokemones> Usuario_Pokemones { get; set; }
		public DbSet<Batalla> Batalla { get; set; }

        public DbSet<Enfermeria> Enfermeria { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

			// Configuración de clave primaria compuesta
			modelBuilder.Entity<Usuario_Pokemones>()
				.HasKey(up => new { up.UsuarioId, up.PokemonId });
		}
    }
}
